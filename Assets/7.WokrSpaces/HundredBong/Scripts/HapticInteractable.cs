using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


[System.Serializable]
public class Haptic
{
    [Range(0, 1)] public float intensity;
    [Range(0, 1)] public float duration;

    public void ActivateHaptic(ActivateEventArgs arg)
    {
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            SendHaptic(controllerInteractor.xrController);
        }
    }

    public void DeactivateHaptic(DeactivateEventArgs arg)
    {
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            SendHaptic(controllerInteractor.xrController);
        }
    }

    public void HoverEnterHaptic(HoverEnterEventArgs arg)
    {
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            SendHaptic(controllerInteractor.xrController);
        }
    }

    public void HoverExitHaptic(HoverExitEventArgs arg)
    {
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            SendHaptic(controllerInteractor.xrController);
        }
    }

    public void SelectEnterHaptic(SelectEnterEventArgs arg)
    {
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            SendHaptic(controllerInteractor.xrController);
        }
    }

    public void SelectExitHaptic(SelectExitEventArgs arg)
    {
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            SendHaptic(controllerInteractor.xrController);
        }
    }

    public void SendHaptic(XRBaseController controller)
    {
        if (intensity > 0)
        {
            controller.SendHapticImpulse(intensity, duration);
        }
    }

}

public class HapticInteractable : MonoBehaviour
{
    public Haptic hapticOnActivated;
    public Haptic hapticOnDectivated;
    public Haptic hapticHoverEnterd;
    public Haptic hapticHoverEXited;
    public Haptic hapticSelectEnterd;
    public Haptic hapticSelectExited;

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        interactable.activated.AddListener(hapticOnActivated.ActivateHaptic);
        interactable.deactivated.AddListener(hapticOnDectivated.DeactivateHaptic);
        interactable.hoverEntered.AddListener(hapticHoverEnterd.HoverEnterHaptic);
        interactable.hoverExited.AddListener(hapticHoverEXited.HoverExitHaptic);
        interactable.selectEntered.AddListener(hapticSelectEnterd.SelectEnterHaptic);
        interactable.selectExited.AddListener(hapticSelectExited.SelectExitHaptic);    
    }


}
