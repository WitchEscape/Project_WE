using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ForceGrabSetting : MonoBehaviour
{
    [SerializeField]
    private XRBaseInteractable interactor;

    private void Awake()
    {
        if (interactor == null)
            interactor = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (interactor != null)
        {
            interactor.hoverEntered.AddListener(InteractorForceGrabSet);
            interactor.hoverExited.AddListener(InteractorForceGrabUnSet);
        }
    }

    private void OnDisable()
    {
        if (interactor != null)
        {
            interactor.hoverEntered.RemoveListener(InteractorForceGrabSet);
            interactor.hoverExited.RemoveListener(InteractorForceGrabUnSet);
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
