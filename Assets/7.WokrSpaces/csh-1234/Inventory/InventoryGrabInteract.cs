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
        Debug.Log($"[InventoryGrabInteract] SetControllerGrip - Controller: {controller.name}, State: {state}");

        if (!controllers.Contains(controller))
        {
            Debug.Log("[InventoryGrabInteract] 而⑦듃濡ㅻ윭媛 由ъ뒪?몄뿉 ?놁뒿?덈떎.");
            return;
        }

        if (!inventorySlot.gameObject.activeInHierarchy)
        {
            Debug.Log("[InventoryGrabInteract] ?몃깽?좊━ ?щ’??鍮꾪솢?깊솕 ?곹깭?낅땲??");
            return;
        }

        Debug.Log("[InventoryGrabInteract] TryInteractWithSlot ?몄텧");
        inventorySlot.TryInteractWithSlot(controller.GetComponentInChildren<XRDirectInteractor>());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[InventoryGrabInteract] Trigger Enter: {other.name}");

        // XRDirectInteractor瑜?癒쇱? ?뺤씤
        var directInteractor = other.GetComponent<XRDirectInteractor>();
        if (directInteractor != null)
        {
            var controllerComponent = directInteractor.GetComponentInParent<ActionBasedController>();
            if (controllerComponent && !controllers.Contains(controllerComponent))
            {
                controllers.Add(controllerComponent);
                Debug.Log($"[InventoryGrabInteract] 而⑦듃濡ㅻ윭 異붽??? {controllerComponent.name}");
            }
            return;
        }

        // XRDirectInteractor媛 ?녿떎硫?ActionBasedController瑜?吏곸젒 ?뺤씤
        var foundController = other.GetComponentInParent<ActionBasedController>();
        if (foundController && !controllers.Contains(foundController))
        {
            controllers.Add(foundController);
            Debug.Log($"[InventoryGrabInteract] 而⑦듃濡ㅻ윭 異붽??? {foundController.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var controller = other.GetComponentInParent<ActionBasedController>();
        if (controller) controllers.Remove(controller);
    }
}