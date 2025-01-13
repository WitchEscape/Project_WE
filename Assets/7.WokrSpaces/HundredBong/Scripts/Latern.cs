using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Latern : MonoBehaviour
{
    private XRSocketInteractor interactor;
    private Light pointLight;
    public string itemTag;

    private void Awake()
    {
        interactor = GetComponentInChildren<XRSocketInteractor>();
        pointLight = GetComponentInChildren<Light>();
    }

    private void OnEnable()
    {
        interactor.selectEntered.AddListener(CoreEnter);
        interactor.selectExited.AddListener(CoreExit);
    }

    private void OnDisable()
    {
        interactor.selectEntered.RemoveListener(CoreEnter);
        interactor.selectExited.RemoveListener(CoreExit);
    }

    private void Start()
    {
        pointLight.gameObject.SetActive(false);
    }

    private void CoreEnter(SelectEnterEventArgs arg)
    {
        if (arg.interactableObject.transform.CompareTag(itemTag))
        {
            pointLight.gameObject.SetActive(true);
        }
    }

    private void CoreExit(SelectExitEventArgs arg)
    {
        if (arg.interactableObject.transform.CompareTag(itemTag))
        {
            pointLight.gameObject.SetActive(false);
        }
    }

}
