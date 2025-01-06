using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableItemData : MonoBehaviour
{
    public bool canInventory = true;

    public bool canDistanceGrab = true;

    private void Awake()
    {
        // XRGrabInteractable????덈뮉筌왖 ?類ㅼ뵥
        if (!GetComponent<XRGrabInteractable>())
        {
            Debug.LogError($"[InteractableItemData] {gameObject.name}??XRGrabInteractable????곷뮸??덈뼄!");
        }
    }
}