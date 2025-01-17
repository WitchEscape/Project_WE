using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class INk : MonoBehaviour
{
    [SerializeField]
    private XRSocketInteractor socket;
    [SerializeField]
    private Rigidbody capRigidbody;

    private const int changeLayerNum = 8;
    private int baseLayerNum;
    private void Awake()
    {
        socket ??= GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        if (socket != null)
        {
            socket.selectEntered.AddListener(LayerChange);
            socket.selectExited.AddListener(LayerChange);
        }
    }

    private void OnDisable()
    {
        if (socket != null)
        {
            socket.selectEntered.RemoveListener(LayerChange);
            socket.selectExited.RemoveListener(LayerChange);
        }
    }

    private void LayerChange(SelectEnterEventArgs arg0)
    {
        GameObject obj = arg0.interactableObject.transform.gameObject;
        baseLayerNum = obj.layer;
        obj.layer = changeLayerNum;
        capRigidbody.isKinematic = true;
    }

    private void LayerChange(SelectExitEventArgs arg0)
    {
        arg0.interactableObject.transform.gameObject.layer = baseLayerNum;
        capRigidbody.isKinematic = false;
    }
}
