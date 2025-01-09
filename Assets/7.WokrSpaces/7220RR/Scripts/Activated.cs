using UnityEngine;

public class Activated : MonoBehaviour
{
    public GameObject activatedUI;

    private void Awake()
    {
        if (activatedUI != null)
            activatedUI.SetActive(false);
    }

    public virtual void activateUI()
    {
        if (activatedUI != null)
            activatedUI.SetActive(!activatedUI.activeSelf);
    }
    public virtual void activate()
    {

    }
}
