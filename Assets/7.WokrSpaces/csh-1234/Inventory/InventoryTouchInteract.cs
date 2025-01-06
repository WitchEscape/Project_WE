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
            Debug.Log("[InventoryTouchInteract] 而댄룷?뚰듃媛 鍮꾪솢?깊솕 ?곹깭?낅땲??");
            return;
        }

        Debug.Log($"[InventoryTouchInteract] Trigger Enter: {other.name}");

        // XRDirectInteractor瑜?癒쇱? ?뺤씤
        var directInteractor = other.GetComponent<XRDirectInteractor>();
        if (directInteractor == null)
        {
            directInteractor = other.GetComponentInParent<XRDirectInteractor>();
        }

        if (!directInteractor)
        {
            Debug.Log("[InventoryTouchInteract] XRDirectInteractor瑜?李얠쓣 ???놁뒿?덈떎.");
            return;
        }

        Debug.Log($"[InventoryTouchInteract] ?명꽣?숉꽣 媛먯??? {directInteractor.name}");

        if (!inventorySlot.CurrentSlotItem && !directInteractor.hasSelection)
        {
            Debug.Log("[InventoryTouchInteract] ?щ’??鍮꾩뼱?덇퀬 而⑦듃濡ㅻ윭媛 ?꾨Т寃껊룄 ?ㅺ퀬?덉? ?딆뒿?덈떎.");
            return;
        }

        Debug.Log("[InventoryTouchInteract] TryInteractWithSlot ?몄텧");
        inventorySlot.TryInteractWithSlot(directInteractor);
    }
}