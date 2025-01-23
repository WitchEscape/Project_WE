using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialLatern : MonoBehaviour
{
    private XRSocketTagInteractor interactor;
    public Light pointLight;
    public GameObject invisibleWall;
    public GameObject triggerZone6;
    
    private void Awake()
    {
        interactor = GetComponent<XRSocketTagInteractor>();
    }

    private void OnEnable()
    {
        interactor.selectEntered.AddListener(CandleEnter);
    }

    private void CandleEnter(SelectEnterEventArgs arg)
    {
        if (arg.interactableObject.transform.gameObject.CompareTag("Lantern"))
        {
            pointLight.gameObject.SetActive(true);
            invisibleWall.gameObject.SetActive(false);
            triggerZone6.gameObject.SetActive(true);
        }
    }
}
