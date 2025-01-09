using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController leftController;
    [SerializeField]
    private ActionBasedController rightController;
    public XRBaseInteractor a;
    private void Awake()
    {
        if (leftController != null)
        {

        }
        else
        {
            Debug.LogError("PlayerInput / LeftController is Null");
        }

        if (rightController != null)
        {
            //rightController.hapticDeviceAction.action.performed += ;
            rightController.scaleToggleAction.action.performed += ToggleAction;
            rightController.activateAction.action.performed += ToggleAction;
        }
        else
        {
            Debug.LogError("PlayerInput / RightController is Null");
        }
    }
    private void Update()
    {
        print(leftController.selectInteractionState);
        print(rightController.selectInteractionState);
        print(a.selectTarget);
    }


    private void ToggleAction(InputAction.CallbackContext call)
    {

    }

    private void TriggerButtonActionSet(InputAction.CallbackContext call)
    {
        //leftController.selectInteractionState
    }
}
