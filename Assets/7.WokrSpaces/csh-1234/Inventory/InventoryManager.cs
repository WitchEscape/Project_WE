using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryManager : MonoBehaviour
{
    // 왼쪽/오른쪽 손으로 메뉴를 열기 위한 입력 액션 참조
    [SerializeField]
    private InputActionReference openMenuInputLeftHand, openMenuInputRightHand;

    // 인벤토리 슬롯 배열
    private InventorySlot[] inventorySlots;

    // VR 컨트롤러 참조
    public ActionBasedController leftController = null, rightController = null;

    // 인벤토리 활성화/비활성화 시 재생할 오디오
    [SerializeField] private AudioSource enableAudio = null, disableAudio = null;

    // 컨트롤러를 바라보도록 할지 설정
    [SerializeField] private bool lookAtController = false;
    private bool isActive = false;

    private void Start()
    {
        Debug.Log("[InventoryManager] Start 시작");
        OnValidate();

        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            Debug.LogError("[InventoryManager] 인벤토리 슬롯이 없습니다!");
            return;
        }

        Debug.Log($"[InventoryManager] 인벤토리 슬롯 개수: {inventorySlots.Length}");

        // 각 인벤토리 슬롯에 시작 아이템 생성 후 비활성화
        foreach (var itemSlot in inventorySlots)
        {
            if (itemSlot == null)
            {
                Debug.LogError("[InventoryManager] 널인 슬롯이 있습니다!");
                continue;
            }
            itemSlot.StartCoroutine(itemSlot.CreateStartingItemAndDisable());
        }

        // 입력 이벤트 핸들러 등록
        if (openMenuInputLeftHand != null && openMenuInputRightHand != null)
        {
            openMenuInputLeftHand.GetInputAction().performed += x => ToggleInventoryAtController(false);
            openMenuInputRightHand.GetInputAction().performed += x => ToggleInventoryAtController(true);
            Debug.Log("[InventoryManager] 입력 이벤트 핸들러 등록 완료");
        }
        else
        {
            Debug.LogError("[InventoryManager] 입력 액션 참조가 설정되지 않았습니다!");
        }
    }

    // 인벤토리 슬롯 컴포넌트들을 찾아서 배열에 저장
    private void OnValidate()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>();
    }

    // 스크립트 활성화시 입력 액션 활성화
    private void OnEnable()
    {
        openMenuInputLeftHand.EnableAction();
        openMenuInputRightHand.EnableAction();
    }

    // 스크립트 비활성화시 입력 액션 비활성화
    private void OnDisable()
    {
        openMenuInputLeftHand.DisableAction();
        openMenuInputRightHand.DisableAction();
    }

    // 지정된 컨트롤러에 인벤토리 토글
    private void ToggleInventoryAtController(bool isRightHand)
    {
        if (isRightHand)
            TurnOnInventory(rightController.gameObject);
        else
            TurnOnInventory(leftController.gameObject);
    }

    // 인벤토리 활성화/비활성화 처리
    private void TurnOnInventory(GameObject hand)
    {
        isActive = !isActive;
        ToggleInventoryItems(isActive, hand);
        PlayAudio(isActive);
    }

    // 상태에 따른 오디오 재생
    private void PlayAudio(bool state)
    {
        if (state)
            enableAudio.Play();
        else
            disableAudio.Play();
    }

    // 모든 인벤토리 슬롯의 상태 변경
    private void ToggleInventoryItems(bool state, GameObject hand)
    {
        foreach (var itemSlot in inventorySlots)
        {
            if (!state)
            {
                itemSlot.DisableSlot();
                Debug.Log($"[InventoryManager] 슬롯 비활성화: {itemSlot.name}");
            }
            else
            {
                if (!itemSlot.gameObject.activeSelf)
                {
                    itemSlot.gameObject.SetActive(true);
                    Debug.Log($"[InventoryManager] 슬롯 활성화: {itemSlot.name}");
                }
                itemSlot.EnableSlot();
                SetPositionAndRotation(hand);
            }
        }
    }

    // 인벤토리 위치와 회전 설정
    private void SetPositionAndRotation(GameObject hand)
    {
        transform.position = hand.transform.position;
        transform.localEulerAngles = Vector3.zero;

        if (lookAtController)
            SetPosition(hand.transform);
        else
            transform.LookAt(Camera.main.transform); // 메인 카메라를 바라보도록 설정
    }

    // 컨트롤러 방향에 따른 인벤토리 위치 설정
    private void SetPosition(Transform hand)
    {
        var handDirection = hand.transform.forward;
        transform.transform.forward = Vector3.ProjectOnPlane(-handDirection, transform.up);
    }
}