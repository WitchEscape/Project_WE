using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryManager : MonoBehaviour
{
    private InventorySlot[] inventorySlots;

    public ActionBasedController leftController = null;
    public ActionBasedController rightController = null;

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

    }

    private void OnValidate()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>();
    }

    public void ToggleInventoryAtController()
    {
        TurnOnInventory();
    }

    private void TurnOnInventory()
    {
        isActive = !isActive;
        ToggleInventoryItems(isActive);
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
            return;
        }

        bool wasInventoryOpen = IsInventoryOpen;
        if (!wasInventoryOpen)
        {
            ToggleInventoryItems(true);
        }

        foreach (var slot in inventorySlots)
        {
            if (slot.CurrentSlotItem != null)
            {
                Destroy(slot.CurrentSlotItem.gameObject);
            }
        }

        foreach (var slotData in data)
        {
            if (slotData.slotIndex < 0 || slotData.slotIndex >= inventorySlots.Length)
            {
                continue;
            }

            var itemPrefab = Resources.Load<GameObject>(slotData.itemPrefabPath);
            if (itemPrefab == null)
            {
                continue;
            }

            var slot = inventorySlots[slotData.slotIndex];

            var startingItemField = typeof(InventorySlot).GetField("startingItem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (startingItemField != null)
            {
                var item = itemPrefab.GetComponent<XRBaseInteractable>();
                if (item != null)
                {
                    var itemDataPrefab = itemPrefab.GetComponent<InteractableItemData>();
                    if (itemDataPrefab != null)
                    {
                        string prefabPath = itemDataPrefab.prefabPath;
                        
                        var itemDataInstance = item.GetComponent<InteractableItemData>();
                        if (itemDataInstance != null)
                        {
                            itemDataInstance.prefabPath = prefabPath;
                        }
                    }

                    startingItemField.SetValue(slot, item);
                    
                    slot.StartCoroutine(slot.CreateStartingItemAndDisable());
                }
            }
        }


        // 모든 아이템이 로드된 후 인벤토리를 이전 상태로 되돌림
        if (!wasInventoryOpen)
        {
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
        foreach (var slot in inventorySlots)
        {
            if (slot != null && slot.CurrentSlotItem != null)
            {
                Destroy(slot.CurrentSlotItem.gameObject);
            }
        }

        if (pannel != null)
        {
            pannel.SetActive(false);
        }
        
        IsInventoryOpen = false;
        isActive = true;
    }
}