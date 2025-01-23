using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EndInven : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    [SerializeField] private GameObject wall;
    private void Start()
    {
        inventoryManager.OnItemAddedToSlot.AddListener(HandleItemAdded);
        inventoryManager.OnItemRemovedFromSlot.AddListener(HandleItemRemoved);
    }
    private void OnDisable()
    {
        inventoryManager.OnItemAddedToSlot.RemoveListener(HandleItemAdded);
        inventoryManager.OnItemRemovedFromSlot.RemoveListener(HandleItemRemoved);
    }

    private void HandleItemAdded(XRBaseInteractable item, int slotIndex)
    {
        wall.SetActive(false);
    }

    private void HandleItemRemoved(XRBaseInteractable item, int slotIndex)
    {
        //제거 이벤트
    }
}
