using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FountainPen : MonoBehaviour
{
    [SerializeField]
    private XRSocketInteractor penBodySocket;
    [SerializeField]
    private Collider penNibCollider;
    [SerializeField]
    private Texture penNibRedTexture;

    private int changeLayerIndex;
    private int penHeadBaseLayerIndex = int.MaxValue;
    private GameObject penHeadFixedJointObject = null;
    private FixedJoint j;
    private bool isInk = false;

    private void Awake()
    {
        changeLayerIndex = gameObject.layer;
        penBodySocket ??= GetComponent<XRSocketInteractor>();
        if (penNibCollider == null)
            penNibCollider = GetComponentsInChildren<Collider>().FirstOrDefault(x => (x.gameObject.CompareTag("Cube")));

        if (penBodySocket == null || penNibCollider == null)
        {
            Debug.LogError("FountainPen / PenNibCollider or PenBodySocket is null");
        }
    }

    private void OnEnable()
    {
        if (penBodySocket != null)
        {
            penBodySocket.selectEntered.AddListener(PenHeadRigidbodyAndLayerChange);
            penBodySocket.selectExited.AddListener(PenHeadRigidbodyAndLayerChange);
        }
        else
        {
            Debug.LogError("FountainPen / OnEnable / PenBodySocket is null");
        }
    }

    private void OnDisable()
    {
        if (penBodySocket != null)
        {
            penBodySocket.selectEntered.RemoveListener(PenHeadRigidbodyAndLayerChange);
            penBodySocket.selectExited.RemoveListener(PenHeadRigidbodyAndLayerChange);
        }
        else
        {
            Debug.LogError("FountainPen / OnDisable / PenBodySocket is null");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Cube") && other.CompareTag("Water"))
        {
            print("OnTriggerEnter1");

            if (!isInk && penNibRedTexture != null)
            {
                print("OnTriggerEnter2");

                penNibCollider.gameObject.GetComponent<MeshRenderer>().material.SetTexture(name, penNibRedTexture);
            }
        }
    }

    private void PenHeadRigidbodyAndLayerChange(SelectEnterEventArgs arg)
    {
        GameObject penHeadObject = arg.interactableObject.transform.gameObject;

        if (penHeadBaseLayerIndex == int.MaxValue)
        {
            penHeadBaseLayerIndex = penHeadObject.layer;

            if (penHeadBaseLayerIndex == int.MaxValue)
            {
                Debug.LogError("FountainPen / PenHeadRigidbodyAndLayerChange / PenHeadBaseLayerIndex is Not Setting");
            }
        }

        penHeadObject.layer = changeLayerIndex;

        if (penHeadFixedJointObject == null)
        {
            FixedJoint penHeadFixedJoint = penHeadObject.GetComponentInChildren<FixedJoint>();
            if (penHeadFixedJoint != null)
                penHeadFixedJoint.connectedBody ??= GetComponent<Rigidbody>();
            penHeadFixedJointObject ??= penHeadFixedJoint.gameObject;

            if (penHeadFixedJointObject == null)
            {
                Debug.LogError("FountainPen / PenHeadRigidbodyAndLayerChange / PenHeadFixedJoint is Not Setting");
                return;
            }
        }

        penHeadFixedJointObject.SetActive(true);
    }

    private void PenHeadRigidbodyAndLayerChange(SelectExitEventArgs arg)
    {
        if (penHeadBaseLayerIndex == int.MaxValue || penHeadFixedJointObject == null)
        {
            Debug.LogError("FountainPen / PenHeadRigidbodyAndLayerChange / PenHead Components is Not Settings");
            return;
        }

        arg.interactableObject.transform.gameObject.layer = penHeadBaseLayerIndex;
        penHeadFixedJointObject.SetActive(false);
    }
}

