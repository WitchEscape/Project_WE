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
    private bool isActive = false;

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

        if (openMenuInputLeftHand != null && openMenuInputRightHand != null)
        {
            openMenuInputLeftHand.GetInputAction().performed += x => ToggleInventoryAtController(false);
            openMenuInputRightHand.GetInputAction().performed += x => ToggleInventoryAtController(true);
        }
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

    private void ToggleInventoryAtController(bool isRightHand)
    {
        if (isRightHand)
            TurnOnInventory(rightController.gameObject);
        else
            TurnOnInventory(leftController.gameObject);
    }

    private void TurnOnInventory(GameObject hand)
    {
        isActive = !isActive;
        ToggleInventoryItems(isActive, hand);
        PlayAudio(isActive);
    }

    private void PlayAudio(bool state)
    {
        if (state)
            enableAudio.Play();
        else
            disableAudio.Play();
    }

    private void ToggleInventoryItems(bool state, GameObject hand)
    {
        if (state)
        {
            pannel.SetActive(true);
        }
        else
        {
            pannel.SetActive(false);
        }
        foreach (var itemSlot in inventorySlots)
        {
            if (!state)
            {
                itemSlot.DisableSlot();
            }
            else
            {
                if (!itemSlot.gameObject.activeSelf)
                {
                    itemSlot.gameObject.SetActive(true);
                }
                itemSlot.EnableSlot();
                SetPositionAndRotation(hand);
            }
        }
    }

    private void SetPositionAndRotation(GameObject hand)
    {
        Transform cameraTransform = Camera.main.transform;
        
        Vector3 inventoryPosition = cameraTransform.position + (cameraTransform.forward * 0.5f);
        transform.position = inventoryPosition;
        
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180f, 0);
    }
}