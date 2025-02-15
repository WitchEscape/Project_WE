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
        if (activatedUI != null)
            ActivateUI(false);

        //if (interactable == null)
        //    interactable = GetComponent<XRBaseInteractable>();

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
        if (activatedUI != null)
            activatedUI.SetActive(isbool);
        else
        {
            Debug.LogError($"{gameObject.name} 뭐함");
        }
    }

    public virtual void Activate()
    {
        if (objects != null)
            ObjectOnOff<GameObject>(objects, true);
        if (interactables != null)
            ObjectOnOff<XRBaseInteractable>(interactables, true);
    }

    public virtual void ObjectOnOff<T>(List<T> list, bool isBooooool) where T : class
    {
        if (list != null)
        {
            foreach (T howRyou in list)
            {
                if (howRyou == null) continue;
                if (howRyou is XRBaseInteractable whatbase)
                {
                    if (whatbase != null)
                        whatbase.enabled = isBooooool;
                }
                else if (howRyou is GameObject whatname)
                {
                    if (whatname != null)
                        whatname.SetActive(isBooooool);
                }
            }
        }
    }
}
