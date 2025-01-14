using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PressButtonTest : XRSimpleInteractable
{
    public Transform button;
    public bool isTrigger;
    public float pressDistance;
    public Axis pressAxis;

    public UnityEvent OnPress;
    public UnityEvent OnRelease;
    private List<ActionBasedController> controllers = new List<ActionBasedController>();
    private void Start()
    {
        SetButtonHeight(0f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (isTrigger)
        {
            hoverEntered.AddListener(TriggerSelectSet);
            hoverExited.AddListener(TriggerSelectReset);
        }
        else
        {
            selectEntered.AddListener(HandleButtonPress);
            selectExited.AddListener(HandleButtonRelease);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (isTrigger)
        {
            hoverEntered.RemoveListener(TriggerSelectSet);
            hoverExited.RemoveListener(TriggerSelectReset);
        }
        else
        {
            selectEntered.RemoveListener(HandleButtonPress);
            selectExited.RemoveListener(HandleButtonRelease);
        }
    }

    private void TriggerSelectSet(HoverEnterEventArgs args)
    {
        ActionBasedController interactorController = args.interactorObject.transform.GetComponentInParent<ActionBasedController>();

        if (interactorController == null)
        {
            Debug.LogError("PressButtonTest / TriggerSleectReset InteractorController is Null");
            return;
        }

        if (controllers.Contains(interactorController))
        {
            Debug.LogError("PressButtonTest / Controllers is Error");
            return;
        }
        else
        {
            controllers.Add(interactorController);
            //interactorController.activateAction.action.performed += TriggerSelect;
            interactorController.activateAction.action.started += TriggerSelect;
            interactorController.activateAction.action.canceled += TriggerSelect;

        }
        //if (interactorController != null)
        //{
        //    var temp = interactorController.activateAction;
        //    interactorController.activateAction = interactorController.selectAction;
        //    interactorController.selectAction = temp;
        //}
    }

    private void TriggerSelectReset(HoverExitEventArgs args)
    {
        ActionBasedController interactorController = args.interactorObject.transform.GetComponentInParent<ActionBasedController>();


        if (interactorController == null)
        {
            Debug.LogError("PressButtonTest / TriggerSleectReset InteractorController is Null");
            return;
        }

        if (controllers.Contains(interactorController))
        {
            //interactorController.activateAction.action.performed -= TriggerSelect;
            interactorController.activateAction.action.started -= TriggerSelect;
            interactorController.activateAction.action.canceled -= TriggerSelect;
            controllers.Remove(interactorController);
        }
        else
        {
            Debug.LogError("PressButtonTest / Controllers is Error");
        }
    }

    private void TriggerSelect(InputAction.CallbackContext context)
    {
        print(context.ReadValue<float>());

        //if (context.ReadValue<float>() >= 0.5f)
        //{
        //    SetButtonHeight(-pressDistance);
        //    OnPress.Invoke();
        //}
        //else
        //{
        //    SetButtonHeight(0f);
        //    OnRelease.Invoke();
        //}
        if (context.started)
        {
            SetButtonHeight(-pressDistance);
            OnPress.Invoke();
        }
        else if (context.canceled)
        {
            SetButtonHeight(pressDistance);
            OnRelease.Invoke();
        }
    }

    private void HandleButtonPress(SelectEnterEventArgs args)
    {
        SetButtonHeight(-pressDistance);
        OnPress.Invoke();
    }

    private void HandleButtonRelease(SelectExitEventArgs args)
    {
        SetButtonHeight(pressDistance);
        OnRelease.Invoke();
    }

    private void SetButtonHeight(float height)
    {
        if (button == null)
        {
            Debug.LogError("PressButtonTest / Button is null");
            return;
        }

        Vector3 newPosition = button.localPosition;
        switch (pressAxis)
        {
            case Axis.XAxis:
                newPosition.x += height;
                break;
            case Axis.YAxis:
                newPosition.y += height;
                break;
            case Axis.ZAxis:
                newPosition.z += height;
                break;
            default:
                Debug.LogError("PressButtonTest / PressAxit is Error");
                break;
        }

        button.localPosition = newPosition;
    }

}
