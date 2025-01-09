using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Converter : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] GameObject ui_Menu;
    [SerializeField] GameObject PlayerPrefab;

    [SerializeField] private InputActionReference openInventoryRightHand;
    [SerializeField] private InputActionReference openMenuInputRightHand;

    private bool isMenuOpen = false;

    private void Start()
    {
        if (openInventoryRightHand != null)
        {
            openInventoryRightHand.GetInputAction().performed += x => ToggleInventoryAtController();
        }
        if (openMenuInputRightHand != null)
        {
            openMenuInputRightHand.GetInputAction().performed += x => HandleMenuInput();
        }
    }

    private void HandleMenuInput()
    {
        if (inventoryManager.IsInventoryOpen && !isMenuOpen)
        {
            inventoryManager.ToggleInventoryItems(false);
            UI_Manager.Instance.OpenUI(ui_Menu);
            isMenuOpen = true;
        }
        else if (!inventoryManager.IsInventoryOpen && !isMenuOpen)
        {
            UI_Manager.Instance.OpenUI(ui_Menu);
            isMenuOpen = true;
        }
        else if (isMenuOpen)
        {
            if (UI_Manager.Instance.GetUIStackCount() == 1)
            {
                UI_Manager.Instance.CloseAllCurrentUI();
                isMenuOpen = false;
            }
            else
            {
                UI_Manager.Instance.CloseCurrentUI();
            }
        }

        if(isMenuOpen == true && ui_Menu.activeSelf == false)
        {
            UI_Manager.Instance.OpenUI(ui_Menu);
            isMenuOpen = true;
        }
    }

    private void ToggleInventoryAtController()
    {
        if (!inventoryManager.IsInventoryOpen)
        {
            UI_Manager.Instance.CloseAllCurrentUI();
            inventoryManager.ToggleInventoryItems(true);
            isMenuOpen = false;
        }
        else
        {
            inventoryManager.ToggleInventoryItems(false);
        }
    }
}
