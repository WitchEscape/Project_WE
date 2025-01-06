using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class InventoryGrabInteract : MonoBehaviour
{
    [SerializeField] private InteractButton interactButton = InteractButton.grip;
    [SerializeField] private InventoryManager inventoryManager;

    private List<ActionBasedController> controllers = new List<ActionBasedController>();
    private InventorySlot inventorySlot;
    private ActionBasedController leftHand, rightHand;

    private enum InteractButton
    {
        trigger,
        grip
    };

    private void Start()
    {
        OnValidate();

        if (interactButton == InteractButton.grip)
        {
            leftHand.selectAction.reference.GetInputAction().performed += x => SetControllerGrip(leftHand, true);
            rightHand.selectAction.reference.GetInputAction().performed += x => SetControllerGrip(rightHand, true);
            leftHand.selectAction.reference.GetInputAction().canceled += x => SetControllerGrip(leftHand, false);
            rightHand.selectAction.reference.GetInputAction().canceled += x => SetControllerGrip(rightHand, false);
        }

        else
        {
            leftHand.activateAction.reference.GetInputAction().performed += x => SetControllerGrip(leftHand, true);
            rightHand.activateAction.reference.GetInputAction().performed += x => SetControllerGrip(rightHand, true);
            leftHand.activateAction.reference.GetInputAction().canceled += x => SetControllerGrip(leftHand, false);
            rightHand.activateAction.reference.GetInputAction().canceled += x => SetControllerGrip(rightHand, false);
        }
    }

    private void OnValidate()
    {
        if (!inventoryManager)
            inventoryManager = GetComponentInParent<InventoryManager>();
        if (!inventorySlot)
            inventorySlot = GetComponent<InventorySlot>();
        if (!leftHand && inventoryManager)
            leftHand = inventoryManager.leftController;
        if (!rightHand && inventoryManager)
            rightHand = inventoryManager.rightController;
    }

    private void SetControllerGrip(ActionBasedController controller, bool state)
    {
        Debug.Log($"[InventoryGrabInteract] SetControllerGrip - Controller: {controller.name}, State: {state}");
        
        if (!controllers.Contains(controller))
        {
            Debug.Log("[InventoryGrabInteract] 컨트롤러가 리스트에 없습니다.");
            return;
        }
        
        if (!inventorySlot.gameObject.activeInHierarchy)
        {
            Debug.Log("[InventoryGrabInteract] 인벤토리 슬롯이 비활성화 상태입니다.");
            return;
        }
        
        Debug.Log("[InventoryGrabInteract] TryInteractWithSlot 호출");
        inventorySlot.TryInteractWithSlot(controller.GetComponentInChildren<XRDirectInteractor>());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[InventoryGrabInteract] Trigger Enter: {other.name}");
        
        if (!other.TryGetComponent(out XRBaseInteractor interactor))
        {
            Debug.Log("[InventoryGrabInteract] XRBaseInteractor를 찾을 수 없습니다.");
            return;
        }
        
        var controller = interactor.GetComponentInParent<ActionBasedController>();
        if (controller && !controllers.Contains(controller))
        {
            controllers.Add(controller);
            Debug.Log($"[InventoryGrabInteract] 컨트롤러 추가됨: {controller.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var controller = other.GetComponentInParent<ActionBasedController>();
        if (controller) controllers.Remove(controller);
    }
}