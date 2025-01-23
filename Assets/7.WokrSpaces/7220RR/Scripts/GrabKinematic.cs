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
            //rigidbodyee.isKinematic = true;
            xrGrabInteractable?.selectExited.AddListener(KinematicReset);
        }
    }

    private void KinematicReset(SelectExitEventArgs args)
    {
        rigidbodyee.isKinematic = false;
        transform.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("누가이김/");
        if (other.CompareTag("Match"))
        {
            print("내가 이김");
            rigidbodyee.isKinematic = true;
            transform.parent = other.transform;
        }
    }
}
