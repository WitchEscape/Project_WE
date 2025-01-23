using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyLayerChgange : MonoBehaviour
{
    [SerializeField]
    private int changeLayerIndex;
    [SerializeField]
    private XRSocketInteractor socket;
    private int baseLayerIndex;

    private void Awake()
    {
        socket ??= GetComponent<XRSocketInteractor>();
        if(socket != null)
        {
            socket.selectEntered.AddListener(ChangeIndex);
            socket.selectExited.AddListener(ChangeIndex);
        }
    }

    private void ChangeIndex(SelectEnterEventArgs args) {
        baseLayerIndex =       args.interactableObject.transform.gameObject.layer;
        args.interactableObject.transform.gameObject.layer = changeLayerIndex;
    }

    private void ChangeIndex(SelectExitEventArgs args)
    {
      args.interactableObject.transform.gameObject.layer = baseLayerIndex ;
    }




}
