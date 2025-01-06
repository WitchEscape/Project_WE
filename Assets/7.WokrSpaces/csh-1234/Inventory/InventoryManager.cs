using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryManager : MonoBehaviour
{
    // ����/������ ������ �޴��� ���� ���� �Է� �׼� ����
    [SerializeField]
    private InputActionReference openMenuInputLeftHand, openMenuInputRightHand;

    // �κ��丮 ���� �迭
    private InventorySlot[] inventorySlots;

    // VR ��Ʈ�ѷ� ����
    public ActionBasedController leftController = null, rightController = null;

    // �κ��丮 Ȱ��ȭ/��Ȱ��ȭ �� ����� �����
    [SerializeField] private AudioSource enableAudio = null, disableAudio = null;

    // ��Ʈ�ѷ��� �ٶ󺸵��� ���� ����
    [SerializeField] private bool lookAtController = false;
    private bool isActive = false;

    private void Start()
    {
        Debug.Log("[InventoryManager] Start ����");
        OnValidate();

        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            Debug.LogError("[InventoryManager] �κ��丮 ������ �����ϴ�!");
            return;
        }

        Debug.Log($"[InventoryManager] �κ��丮 ���� ����: {inventorySlots.Length}");

        // �� �κ��丮 ���Կ� ���� ������ ���� �� ��Ȱ��ȭ
        foreach (var itemSlot in inventorySlots)
        {
            if (itemSlot == null)
            {
                Debug.LogError("[InventoryManager] ���� ������ �ֽ��ϴ�!");
                continue;
            }
            itemSlot.StartCoroutine(itemSlot.CreateStartingItemAndDisable());
        }

        // �Է� �̺�Ʈ �ڵ鷯 ���
        if (openMenuInputLeftHand != null && openMenuInputRightHand != null)
        {
            openMenuInputLeftHand.GetInputAction().performed += x => ToggleInventoryAtController(false);
            openMenuInputRightHand.GetInputAction().performed += x => ToggleInventoryAtController(true);
            Debug.Log("[InventoryManager] �Է� �̺�Ʈ �ڵ鷯 ��� �Ϸ�");
        }
        else
        {
            Debug.LogError("[InventoryManager] �Է� �׼� ������ �������� �ʾҽ��ϴ�!");
        }
    }

    // �κ��丮 ���� ������Ʈ���� ã�Ƽ� �迭�� ����
    private void OnValidate()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>();
    }

    // ��ũ��Ʈ Ȱ��ȭ�� �Է� �׼� Ȱ��ȭ
    private void OnEnable()
    {
        openMenuInputLeftHand.EnableAction();
        openMenuInputRightHand.EnableAction();
    }

    // ��ũ��Ʈ ��Ȱ��ȭ�� �Է� �׼� ��Ȱ��ȭ
    private void OnDisable()
    {
        openMenuInputLeftHand.DisableAction();
        openMenuInputRightHand.DisableAction();
    }

    // ������ ��Ʈ�ѷ��� �κ��丮 ���
    private void ToggleInventoryAtController(bool isRightHand)
    {
        if (isRightHand)
            TurnOnInventory(rightController.gameObject);
        else
            TurnOnInventory(leftController.gameObject);
    }

    // �κ��丮 Ȱ��ȭ/��Ȱ��ȭ ó��
    private void TurnOnInventory(GameObject hand)
    {
        isActive = !isActive;
        ToggleInventoryItems(isActive, hand);
        PlayAudio(isActive);
    }

    // ���¿� ���� ����� ���
    private void PlayAudio(bool state)
    {
        if (state)
            enableAudio.Play();
        else
            disableAudio.Play();
    }

    // ��� �κ��丮 ������ ���� ����
    private void ToggleInventoryItems(bool state, GameObject hand)
    {
        foreach (var itemSlot in inventorySlots)
        {
            if (!state)
            {
                itemSlot.DisableSlot();
                Debug.Log($"[InventoryManager] ���� ��Ȱ��ȭ: {itemSlot.name}");
            }
            else
            {
                if (!itemSlot.gameObject.activeSelf)
                {
                    itemSlot.gameObject.SetActive(true);
                    Debug.Log($"[InventoryManager] ���� Ȱ��ȭ: {itemSlot.name}");
                }
                itemSlot.EnableSlot();
                SetPositionAndRotation(hand);
            }
        }
    }

    // �κ��丮 ��ġ�� ȸ�� ����
    private void SetPositionAndRotation(GameObject hand)
    {
        transform.position = hand.transform.position;
        transform.localEulerAngles = Vector3.zero;

        if (lookAtController)
            SetPosition(hand.transform);
        else
            transform.LookAt(Camera.main.transform); // ���� ī�޶� �ٶ󺸵��� ����
    }

    // ��Ʈ�ѷ� ���⿡ ���� �κ��丮 ��ġ ����
    private void SetPosition(Transform hand)
    {
        var handDirection = hand.transform.forward;
        transform.transform.forward = Vector3.ProjectOnPlane(-handDirection, transform.up);
    }
}