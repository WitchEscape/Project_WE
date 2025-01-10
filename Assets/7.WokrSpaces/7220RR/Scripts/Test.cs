using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Test : MonoBehaviour
{
    public XRBaseInteractable interactable;
    public Collider colliderx;

    private void Start()
    {
        interactable.selectEntered.AddListener((x) =>
        {
            colliderx.isTrigger = true;
        });
        interactable.selectExited.AddListener((x) =>
        {
            colliderx.isTrigger = false;
        });
    }
}
