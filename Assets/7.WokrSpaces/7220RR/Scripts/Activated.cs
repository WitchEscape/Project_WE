using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Activated : MonoBehaviour
{
    public GameObject activatedUI;
    public XRBaseInteractable interactable;
    public List<GameObject> objects;
    public List<XRBaseInteractable> interactables;

    protected virtual void Awake()
    {
        if( activatedUI != null )
        ActivateUI(false);

        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
        {
            interactable.activated.AddListener((args) =>
            {
                ActivateUI();
            });
            interactable.selectExited.AddListener((args) =>
            {
                ActivateUI(false);
            });
        }

        ObjectOnOff<GameObject>(objects, false);
        ObjectOnOff<XRBaseInteractable>(interactables, false);

    }

    public virtual void ActivateUI()
    {
        activatedUI?.SetActive(!activatedUI.activeSelf);
    }

    public virtual void ActivateUI(bool isbool)
    {
        activatedUI?.SetActive(isbool);
    }

    public virtual void Activate()
    {
        ObjectOnOff<GameObject>(objects, true);
        ObjectOnOff<XRBaseInteractable>(interactables, true);
    }

    public virtual void ObjectOnOff<T>(List<T> list, bool isBooooool) where T : class
    {
        if (list != null)
        {
            foreach (T howRyou in list)
            {
                if (howRyou is XRBaseInteractable whatbase)
                {
                    whatbase.enabled = isBooooool;
                }
                else if (howRyou is GameObject whatname)
                {
                    whatname.SetActive(isBooooool);
                }
            }
        }
    }
}
