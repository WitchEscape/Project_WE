using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketLayerChange : MonoBehaviour
{
    [SerializeField]
    private XRSocketInteractor xrSocketInteractor;
    private const int changeLayer = 32;
    private void Awake()
    {
        xrSocketInteractor ??= GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        if (xrSocketInteractor != null)
        {

        }
    }
}
