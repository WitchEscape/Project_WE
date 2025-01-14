using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private InputActionReference openMenuInputLeftHand, openMenuInputRightHand;

    private InventorySlot[] inventorySlots;

    public ActionBasedController leftController = null, rightController = null;

    [SerializeField] private AudioSource enableAudio = null, disableAudio = null;
    [SerializeField] private GameObject pannel; 
    [SerializeField] private bool lookAtController = false;
    private bool isActive = true;

    public bool IsInventoryOpen { get; private set; }

    private void Start()
    {
        OnValidate();

        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            return;
        }

        foreach (var itemSlot in inventorySlots)
        {
            if (itemSlot == null)
            {
                continue;
            }
            itemSlot.StartCoroutine(itemSlot.CreateStartingItemAndDisable());
        }

        //if (openMenuInputLeftHand != null && openMenuInputRightHand != null)
        //{
        //    openMenuInputLeftHand.GetInputAction().performed += x => ToggleInventoryAtController(false);
        //    openMenuInputRightHand.GetInputAction().performed += x => ToggleInventoryAtController(true);
        //}
    }

    private void OnValidate()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>();
    }

    private void OnEnable()
    {
        openMenuInputLeftHand.EnableAction();
        openMenuInputRightHand.EnableAction();
    }

    private void OnDisable()
    {
        openMenuInputLeftHand.DisableAction();
        openMenuInputRightHand.DisableAction();
    }

    public void ToggleInventoryAtController()
    {
        TurnOnInventory();
    }

    private void TurnOnInventory()
    {
        isActive = !isActive;
        ToggleInventoryItems(isActive);
        //PlayAudio(isActive);
    }

    private void PlayAudio(bool state)
    {
        if (state)
            enableAudio.Play();
        else
            disableAudio.Play();
    }

    public void ToggleInventoryItems(bool state)
    {
        IsInventoryOpen = state;
        
        if(pannel!= null)
        {
            if (state)
            {
                pannel.SetActive(true);
            }
            else
            {
                pannel.SetActive(false);
            }

            // 인벤토리 UI 활성화/비활성화
            foreach (var slot in inventorySlots)
            {
                if (slot != null)
                {
                    slot.gameObject.SetActive(state);
                }
            }
            SetPositionAndRotation();
        }
        



    }

    private void SetPositionAndRotation()
    {
        Transform cameraTransform = Camera.main.transform;
        
        Vector3 inventoryPosition = cameraTransform.position + (cameraTransform.forward * 0.5f);
        transform.position = inventoryPosition;
        
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180f, 0);
    }

    public List<SaveData.InventorySlotData> GetInventoryData()
    {
        var data = new List<SaveData.InventorySlotData>();
        
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            if (slot.CurrentSlotItem != null)
            {
                var itemData = slot.CurrentSlotItem.GetComponent<InteractableItemData>();
                if (itemData && !string.IsNullOrEmpty(itemData.prefabPath))
                {
                    // SaveableObject 컴포넌트 확인
                    var saveable = slot.CurrentSlotItem.GetComponent<SaveableObject>();
                    if (saveable != null)
                    {
                        data.Add(new SaveData.InventorySlotData
                        {
                            itemPrefabPath = itemData.prefabPath,
                            slotIndex = i,
                            itemUniqueID = saveable.UniqueID // UniqueID 저장
                        });
                    }
                }
            }
        }
        
        return data;
    }

    public void LoadInventoryData(List<SaveData.InventorySlotData> data)
    {
        if (data == null)
        {
            Debug.LogError("[InventoryManager] 인벤토리 데이터가 null입니다.");
            return;
        }

        Debug.Log($"[InventoryManager] 인벤토리 데이터 로드 시작: {data.Count}개의 아이템");

        // 인벤토리를 일시적으로 활성화
        bool wasInventoryOpen = IsInventoryOpen;
        if (!wasInventoryOpen)
        {
            ToggleInventoryItems(true);
        }

        // 모든 슬롯 초기화
        foreach (var slot in inventorySlots)
        {
            if (slot.CurrentSlotItem != null)
            {
                Debug.Log($"[InventoryManager] 슬롯 아이템 제거: {slot.CurrentSlotItem.name}");
                Destroy(slot.CurrentSlotItem.gameObject);
            }
        }

        // 저장된 아이템 로드
        foreach (var slotData in data)
        {
            Debug.Log($"[InventoryManager] 아이템 로드 시도 - Path: {slotData.itemPrefabPath}, SlotIndex: {slotData.slotIndex}");

            if (slotData.slotIndex < 0 || slotData.slotIndex >= inventorySlots.Length)
            {
                Debug.LogError($"[InventoryManager] 잘못된 슬롯 인덱스: {slotData.slotIndex}");
                continue;
            }

            var itemPrefab = Resources.Load<GameObject>(slotData.itemPrefabPath);
            if (itemPrefab == null)
            {
                Debug.LogError($"[InventoryManager] 프리팹을 찾을 수 없음: {slotData.itemPrefabPath}");
                continue;
            }

            var slot = inventorySlots[slotData.slotIndex];
            Debug.Log($"[InventoryManager] 슬롯 {slotData.slotIndex}에 아이템 생성 시작");

            // startingItem 설정
            var startingItemField = typeof(InventorySlot).GetField("startingItem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (startingItemField != null)
            {
                var item = itemPrefab.GetComponent<XRBaseInteractable>();
                if (item != null)
                {
                    // prefabPath 설정을 위해 InteractableItemData 컴포넌트 확인
                    var itemDataPrefab = itemPrefab.GetComponent<InteractableItemData>();
                    if (itemDataPrefab != null)
                    {
                        // 프리팹의 InteractableItemData에서 prefabPath 가져오기
                        string prefabPath = itemDataPrefab.prefabPath;
                        
                        // 인스턴스의 InteractableItemData에 prefabPath 설정
                        var itemDataInstance = item.GetComponent<InteractableItemData>();
                        if (itemDataInstance != null)
                        {
                            itemDataInstance.prefabPath = prefabPath;
                            Debug.Log($"[InventoryManager] prefabPath 설정: {prefabPath}");
                        }
                    }

                    startingItemField.SetValue(slot, item);
                    Debug.Log($"[InventoryManager] startingItem 설정 완료: {item.name}");
                    
                    slot.StartCoroutine(slot.CreateStartingItemAndDisable());
                }
                else
                {
                    Debug.LogError("[InventoryManager] XRBaseInteractable 컴포넌트를 찾을 수 없음");
                }
            }
            else
            {
                Debug.LogError("[InventoryManager] startingItem 필드를 찾을 수 없음");
            }
        }

        //ToggleInventoryItems(false);

        // 모든 아이템이 로드된 후 인벤토리를 이전 상태로 되돌림
        if (!wasInventoryOpen)
        {
            // 약간의 지연을 주어 아이템 설정이 완료되도록 함
            StartCoroutine(CloseInventoryAfterDelay());
        }
    }

    private IEnumerator CloseInventoryAfterDelay()
    {
        // 아이템 설정이 완료될 때까지 잠시 대기
        yield return new WaitForSeconds(0.5f);
        ToggleInventoryItems(false);
    }

    public void ClearInventory()
    {
        Debug.Log("[InventoryManager] 인벤토리 초기화 시작");
        
        // 모든 슬롯의 아이템 제거
        foreach (var slot in inventorySlots)
        {
            if (slot != null && slot.CurrentSlotItem != null)
            {
                Debug.Log($"[InventoryManager] 슬롯 아이템 제거: {slot.CurrentSlotItem.name}");
                Destroy(slot.CurrentSlotItem.gameObject);
            }
        }

        // 인벤토리 UI 상태 초기화
        if (pannel != null)
        {
            pannel.SetActive(false);
        }
        
        IsInventoryOpen = false;
        isActive = true;

        Debug.Log("[InventoryManager] 인벤토리 초기화 완료");
    }
}