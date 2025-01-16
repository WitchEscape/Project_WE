using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ForceGrabSetting : MonoBehaviour
{
    [SerializeField]
    private XRBaseInteractable interactable;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(InteractorForceGrabSet);
            interactable.hoverExited.AddListener(InteractorForceGrabUnSet);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(InteractorForceGrabSet);
            interactable.hoverExited.RemoveListener(InteractorForceGrabUnSet);
        }
    }

    private void InteractorForceGrabSet(HoverEnterEventArgs arg)
    {
        if (arg.interactorObject.transform.TryGetComponent<XRRayInteractor>(out XRRayInteractor ray))
        {
            ray.useForceGrab = true;
        }
    }

    private void InteractorForceGrabUnSet(HoverExitEventArgs arg)
    {
        if (arg.interactorObject.transform.TryGetComponent<XRRayInteractor>(out XRRayInteractor ray))
        {
            ray.useForceGrab = false;
        }
    }
}
