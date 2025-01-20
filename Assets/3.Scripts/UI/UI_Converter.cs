using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Converter : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] GameObject ui_Menu;
    
    [SerializeField] private InputActionReference openInventoryRightHand;
    [SerializeField] private InputActionReference openMenuInputRightHand;

    public bool isMenuOpen = false;

    private InputAction.CallbackContext inventoryAction;
    private InputAction.CallbackContext menuAction;

    private void Start()
    {
        if (ui_Menu == null)
        {
            return;
        }

        if (openInventoryRightHand != null)
        {
            openInventoryRightHand.GetInputAction().performed += OnInventoryPerformed;
        }
        if (openMenuInputRightHand != null)
        {
            openMenuInputRightHand.GetInputAction().performed += OnMenuPerformed;
        }
    }

    private void OnDestroy()
    {
        if (openInventoryRightHand != null)
        {
            openInventoryRightHand.GetInputAction().performed -= OnInventoryPerformed;
        }
        if (openMenuInputRightHand != null)
        {
            openMenuInputRightHand.GetInputAction().performed -= OnMenuPerformed;
        }
    }

    private void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        ToggleInventoryAtController();
    }

    private void OnMenuPerformed(InputAction.CallbackContext context)
    {
        HandleMenuInput();
    }

    private void HandleMenuInput()
    {
        if (ui_Menu == null)
        {
            isMenuOpen = false;
            return;
        }

        if (inventoryManager.IsInventoryOpen && !isMenuOpen)
        {
            inventoryManager.ToggleInventoryItems(false);
            UIManager.Instance.OpenUI(ui_Menu);
            isMenuOpen = true;
        }
        else if (!inventoryManager.IsInventoryOpen && !isMenuOpen)
        {
            UIManager.Instance.OpenUI(ui_Menu);
            isMenuOpen = true;
        }
        else if (isMenuOpen)
        {
            if (UIManager.Instance.GetUIStackCount() == 1)
            {
                UIManager.Instance.CloseAllCurrentUI();
                isMenuOpen = false;
            }
            else
            {
                UIManager.Instance.CloseCurrentUI();
            }
        }
    }

    private void ToggleInventoryAtController()
    {
        if (!inventoryManager.IsInventoryOpen)
        {
            UIManager.Instance.CloseAllCurrentUI();
            inventoryManager.ToggleInventoryItems(true);
            isMenuOpen = false;
        }
        else
        {
            inventoryManager.ToggleInventoryItems(false);
        }
    }
}
