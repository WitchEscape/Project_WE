using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Test : MonoBehaviour
{
    public XRBaseInteractable interactable;
    public Collider colliderx;
    public int[] asd = new int[10];
    public List<int> list = new List<int>(1);
    public int asss;
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
