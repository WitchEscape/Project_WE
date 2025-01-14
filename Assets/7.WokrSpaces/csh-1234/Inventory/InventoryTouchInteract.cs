using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


/// <summary>
/// 아이템을 그랩한 상태에서 슬롯에 터치(닿이면) 들어가게 하는 컴포넌트
/// </summary>
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
            return;
        }

        var directInteractor = other.GetComponent<XRDirectInteractor>();
        if (directInteractor == null)
        {
            directInteractor = other.GetComponentInParent<XRDirectInteractor>();
        }

        if (!directInteractor)
        {
            return;
        }

        if (!inventorySlot.CurrentSlotItem && !directInteractor.hasSelection)
        {
            return;
        }
        inventorySlot.TryInteractWithSlot(directInteractor);
    }
}