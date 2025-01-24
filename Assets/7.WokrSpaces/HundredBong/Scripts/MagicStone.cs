using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MagicStone : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable interactable;
    private bool isTrun;

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        isTrun = true;
        rb.useGravity = false;
        interactable.selectEntered.AddListener(OnGrabCore);
        interactable.selectExited.AddListener(OnReleaseCore);
    }



    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrabCore);
        interactable.selectExited.RemoveListener(OnReleaseCore);
    }

    private void Update()
    {
        if (isTrun)
        {
            transform.Rotate(Vector3.up);

        }
    }

    private void OnGrabCore(SelectEnterEventArgs arg)
    {
        isTrun = false;
    }

    private void OnReleaseCore(SelectExitEventArgs arg)
    {
        //땡겨올 때 Use Gravity 설정해서 정상적으로 날아오도록 설정 
        rb.useGravity = true;
    }
}
