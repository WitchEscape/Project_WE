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
            Debug.Log("[InventoryTouchInteract] ?뚮똾猷??곕뱜揶쎛 ??쑵??源딆넅 ?怨밴묶??낅빍??");
            return;
        }

        Debug.Log($"[InventoryTouchInteract] Trigger Enter: {other.name}");

        // XRDirectInteractor???믪눘? ?類ㅼ뵥
        var directInteractor = other.GetComponent<XRDirectInteractor>();
        if (directInteractor == null)
        {
            directInteractor = other.GetComponentInParent<XRDirectInteractor>();
        }

        if (!directInteractor)
        {
            Debug.Log("[InventoryTouchInteract] XRDirectInteractor??筌≪뼚??????곷뮸??덈뼄.");
            return;
        }

        Debug.Log($"[InventoryTouchInteract] ?紐낃숲??됯숲 揶쏅Ŋ??? {directInteractor.name}");

        if (!inventorySlot.CurrentSlotItem && !directInteractor.hasSelection)
        {
            Debug.Log("[InventoryTouchInteract] ???????쑴堉??뉙??뚢뫂?껅에?살쑎揶쎛 ?袁ⓓ℡칰猿딅즲 ??블??? ??녿뮸??덈뼄.");
            return;
        }

        Debug.Log("[InventoryTouchInteract] TryInteractWithSlot ?紐꾪뀱");
        inventorySlot.TryInteractWithSlot(directInteractor);
    }
}