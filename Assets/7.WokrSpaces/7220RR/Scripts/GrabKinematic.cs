using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabKinematic : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable xrGrabInteractable;
    [SerializeField] private Rigidbody rigidbodyee;

    private void Awake()
    {
        xrGrabInteractable ??= GetComponent<XRGrabInteractable>();
        rigidbodyee ??= GetComponent<Rigidbody>();

        if (rigidbodyee != null)
        {
            rigidbodyee.isKinematic = true;
            xrGrabInteractable?.selectExited.AddListener(KinematicReset);
        }
    }

    private void KinematicReset(SelectExitEventArgs args)
    {
        rigidbodyee.isKinematic = false;
    }
}
