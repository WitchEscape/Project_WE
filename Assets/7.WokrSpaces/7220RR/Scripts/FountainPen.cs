using System.Collections.Generic;
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
    [SerializeField]
    private Rigidbody penHeadRigidbody;
    [SerializeField]
    private Rigidbody penBodyRigidbody;

    private int changeLayerIndex;
    private int penHeadBaseLayerIndex = int.MaxValue;
    private FixedJoint j;
    private bool isInk = false;
    private Dictionary<Collider, Material> paperMaterials = new Dictionary<Collider, Material>();
    private const float penBodyRigidBodyForce = 0.5f;

    private void Awake()
    {
        changeLayerIndex = gameObject.layer;
        penBodySocket ??= GetComponent<XRSocketInteractor>();
        if (penBodyRigidbody == null)
            penBodyRigidbody = GetComponent<Rigidbody>();
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
        if (other.CompareTag("Water") && !isInk)
        {
            if (penNibCollider != null && penNibRedTexture != null)
            {
                penNibCollider.gameObject.GetComponent<Renderer>().material.mainTexture = penNibRedTexture;
                isInk = true;
            }
        }
        else if (other.CompareTag("Cube"))
        {
            if (!paperMaterials.ContainsKey(other))
            {
                Material newMaterial = other.gameObject.GetComponent<Renderer>().material;
                paperMaterials.Add(other, newMaterial);
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (!isInk) return;
        print("stay");
        if (paperMaterials.ContainsKey(other) && penBodyRigidbody.velocity.magnitude >= penBodyRigidBodyForce)
        {
            paperMaterials[other].color = Color.Lerp(paperMaterials[other].color, Color.red, Time.deltaTime);
            print(paperMaterials[other].color);
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

        if (penHeadRigidbody == null)
        {
            penHeadRigidbody = penHeadObject.GetComponent<Rigidbody>();
            if (penHeadRigidbody == null)
                Debug.LogError("FountainPen / PenHeadRigidbodyAndLayerChange / PenHeadRigidbody is null");
        }

        penHeadObject.layer = changeLayerIndex;
        penHeadRigidbody.isKinematic = true;
        penNibCollider.enabled = false;
    }

    private void PenHeadRigidbodyAndLayerChange(SelectExitEventArgs arg)
    {
        if (penHeadBaseLayerIndex == int.MaxValue || penHeadRigidbody == null)
        {
            Debug.LogError("FountainPen / PenHeadRigidbodyAndLayerChange / PenHead Components is Not Settings");
            return;
        }

        penHeadRigidbody.isKinematic = false;
        arg.interactableObject.transform.gameObject.layer = penHeadBaseLayerIndex;
        penNibCollider.enabled = true;
    }
}

