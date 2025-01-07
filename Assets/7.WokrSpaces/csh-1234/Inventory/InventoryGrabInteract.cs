using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class InventoryGrabInteract : MonoBehaviour
{
    [SerializeField] private InteractButton interactButton = InteractButton.grip;
    [SerializeField] private InventoryManager inventoryManager;

    [SerializeField] private List<ActionBasedController> controllers = new List<ActionBasedController>();
    [SerializeField] private InventorySlot inventorySlot;
    [SerializeField] private ActionBasedController leftHand, rightHand;

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
        if (!controllers.Contains(controller))
        {
            return;
        }

        if (!inventorySlot.gameObject.activeInHierarchy)
        {
            return;
        }

        inventorySlot.TryInteractWithSlot(controller.GetComponentInChildren<XRDirectInteractor>());
    }

    private void OnTriggerEnter(Collider other)
    {
        var directInteractor = other.GetComponent<XRDirectInteractor>();
        if (directInteractor != null)
        {
            var controllerComponent = directInteractor.GetComponentInParent<ActionBasedController>();
            if (controllerComponent && !controllers.Contains(controllerComponent))
            {
                controllers.Add(controllerComponent);
            }
            return;
        }

        var foundController = other.GetComponentInParent<ActionBasedController>();
        if (foundController && !controllers.Contains(foundController))
        {
            controllers.Add(foundController);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var controller = other.GetComponentInParent<ActionBasedController>();
        if (controller) controllers.Remove(controller);
    }
}