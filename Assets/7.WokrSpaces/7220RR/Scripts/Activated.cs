using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Activated : MonoBehaviour
{
    public GameObject activatedUI;
    public GameObject activateObject;
    public ActiveUIType uiType;
    protected XRBaseInteractable interactable;

    protected virtual void Awake()
    {
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
    }

    public virtual void ActivateUI()
    {
        //if (activatedUI != null)
        //    activatedUI.SetActive(!activatedUI.activeSelf);

        if (uiType == ActiveUIType.None) return;

        switch (uiType)
        {
            case ActiveUIType.None:
                break;
            case ActiveUIType.UI:
                activatedUI?.SetActive(!activatedUI.activeSelf);
                break;
            case ActiveUIType.Object:
                activateObject?.SetActive(!activateObject.activeSelf);
                break;
        }
    }

    public virtual void ActivateUI(bool isbool)
    {
        if (uiType == ActiveUIType.None) return;

        switch (uiType)
        {
            case ActiveUIType.None:
                break;
            case ActiveUIType.UI:
                activatedUI?.SetActive(isbool);
                break;
            case ActiveUIType.Object:
                activateObject?.SetActive(isbool);
                break;
        }
    }

    public virtual void activate()
    {

    }
}
