using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Latern : MonoBehaviour
{
    private XRSocketTagInteractor interactor;
    public Light pointLight;
    public string itemTag;

    private void Awake()
    {
        interactor = GetComponentInChildren<XRSocketTagInteractor>();
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
        if (pointLight.gameObject.activeSelf && pointLight != null)
        {
            pointLight.gameObject.SetActive(false); 
        }

    }

    private void CoreEnter(SelectEnterEventArgs arg)
    {
        if (arg.interactableObject.transform.CompareTag(itemTag))
        {
            pointLight.gameObject.SetActive(true);
            arg.interactableObject.transform.gameObject.layer = LayerMask.NameToLayer("Interactable");
            gameObject.tag = "Core";
        }
    }

    private void CoreExit(SelectExitEventArgs arg)
    {
        if (arg.interactableObject.transform.CompareTag(itemTag))
        {
            pointLight.gameObject.SetActive(false);
            arg.interactableObject.transform.gameObject.layer = LayerMask.NameToLayer("Default");
            gameObject.tag = "Untagged";
        }
    }

}
