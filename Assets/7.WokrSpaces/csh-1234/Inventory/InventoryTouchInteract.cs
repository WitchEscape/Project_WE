using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryTouchInteract : MonoBehaviour
{
    private InventorySlot inventorySlot;

    private void Start()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (!inventorySlot)
            inventorySlot = GetComponent<InventorySlot>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
        {
            Debug.Log("[InventoryTouchInteract] 컴포넌트가 비활성화 상태입니다.");
            return;
        }

        Debug.Log($"[InventoryTouchInteract] Trigger Enter: {other.name}");

        var controller = other.GetComponent<XRDirectInteractor>();
        if (!controller)
        {
            Debug.Log("[InventoryTouchInteract] XRDirectInteractor를 찾을 수 없습니다.");
            return;
        }

        Debug.Log($"[InventoryTouchInteract] 컨트롤러 감지됨: {controller.name}");

        if (!inventorySlot.CurrentSlotItem && !controller.hasSelection)
        {
            Debug.Log("[InventoryTouchInteract] 슬롯이 비어있고 컨트롤러가 아무것도 들고있지 않습니다.");
            return;
        }

        Debug.Log("[InventoryTouchInteract] TryInteractWithSlot 호출");
        inventorySlot.TryInteractWithSlot(controller);
    }
}