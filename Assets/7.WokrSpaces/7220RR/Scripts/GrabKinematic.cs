using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabKinematic : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable xrGrabInteractable;
    [SerializeField] private Rigidbody rigidbodyee;
    [SerializeField] private int changeLayer = 8;
    private int baseLayer;


    private void Awake()
    {
        xrGrabInteractable ??= GetComponent<XRGrabInteractable>();
        rigidbodyee ??= GetComponent<Rigidbody>();
        baseLayer = gameObject.layer;

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
        gameObject.layer = baseLayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Match"))
        {
            gameObject.layer = changeLayer;
            rigidbodyee.isKinematic = true;
            //rigidbodyee.velocity = Vector3.zero;
            transform.parent = other.transform.parent;
            Vector3 newV3 = transform.localEulerAngles;
            newV3.x = 0f;
            newV3.z = 0f;
            transform.localEulerAngles = newV3;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Match"))
        {
            rigidbodyee.useGravity = true;
        }
    }
}
