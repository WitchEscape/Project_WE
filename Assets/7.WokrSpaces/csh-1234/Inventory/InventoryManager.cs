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

        //if (openMenuInputLeftHand != null && openMenuInputRightHand != null)
        //{
        //    openMenuInputLeftHand.GetInputAction().performed += x => ToggleInventoryAtController(false);
        //    openMenuInputRightHand.GetInputAction().performed += x => ToggleInventoryAtController(true);
        //}
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

    public void ToggleInventoryAtController()
    {
        TurnOnInventory();
    }

    private void TurnOnInventory()
    {
        isActive = !isActive;
        ToggleInventoryItems(isActive);
        //PlayAudio(isActive);
    }

    private void PlayAudio(bool state)
    {
        if (state)
            enableAudio.Play();
        else
            disableAudio.Play();
    }

    public void ToggleInventoryItems(bool state)
    {
        IsInventoryOpen = state;
        
        if(state)
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

    private void SetPositionAndRotation()
    {
        Transform cameraTransform = Camera.main.transform;
        
        Vector3 inventoryPosition = cameraTransform.position + (cameraTransform.forward * 0.5f);
        transform.position = inventoryPosition;
        
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180f, 0);
    }
}