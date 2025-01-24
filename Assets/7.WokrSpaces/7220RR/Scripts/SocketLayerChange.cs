using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketLayerChange : MonoBehaviour
{
    [SerializeField]
    private XRSocketInteractor xrSocketInteractor;
    private const int changeLayer = 9;
    private int baseInteractableLayer;

    private void Awake()
    {
        xrSocketInteractor ??= GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        if (xrSocketInteractor != null)
        {
            xrSocketInteractor.selectEntered.AddListener(LayerChangeSet);
            xrSocketInteractor.selectExited.AddListener(LayerChangeReset);
        }
    }

    private void OnDisable()
    {
        if (xrSocketInteractor != null)
        {
            xrSocketInteractor.selectEntered.RemoveListener(LayerChangeSet);
            xrSocketInteractor.selectExited.RemoveListener(LayerChangeReset);
        }
    }

    private void LayerChangeSet(SelectEnterEventArgs arg)
    {
        GameObject obj = arg.interactableObject.transform.gameObject;
        baseInteractableLayer = obj.layer;
        obj.layer = changeLayer;
        Debug.Log($"오브제 레이어1 {obj.layer}");

    }

    private void LayerChangeReset(SelectExitEventArgs arg)
    {
        Debug.Log($"오브제 레이어2 진입");

        GameObject obj = arg.interactableObject.transform.gameObject;
        obj.layer = baseInteractableLayer;
        Debug.Log($"오브제 레이어2 {obj.layer}");
    }
}
