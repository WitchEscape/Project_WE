using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableItemData : MonoBehaviour
{
    public bool canInventory = true;

    public bool canDistanceGrab = true;

    private void Awake()
    {
        // XRGrabInteractable이 있는지 확인
        if (!GetComponent<XRGrabInteractable>())
        {
            Debug.LogError($"[InteractableItemData] {gameObject.name}에 XRGrabInteractable이 없습니다!");
        }
    }
}