using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class WandSocket : MonoBehaviour
{
    private const int changeLayer = 31;
    private int baseWandBodyLayer;
    private int baseWandHeadLayer;
    [SerializeField]
    private XRSocketInteractor wandSocketInteractor;

    private void Awake()
    {
        baseWandBodyLayer = gameObject.layer;
        wandSocketInteractor ??= GetComponentInChildren<XRSocketInteractor>();

        if (SceneManager.GetActiveScene().buildIndex != 5 && wandSocketInteractor != null)
        {
            wandSocketInteractor.enabled = false;
        }

    }

    private void OnDisable()
    {
        if (wandSocketInteractor != null && wandSocketInteractor.enabled == true)
        {
            wandSocketInteractor.selectEntered.RemoveListener(LayerChangeSet);
            wandSocketInteractor.selectExited.RemoveListener(LayerChangeReset);
        }
    }

    private void OnEnable()
    {
        if (wandSocketInteractor != null && wandSocketInteractor.enabled == true)
        {
            wandSocketInteractor.selectEntered.AddListener(LayerChangeSet);
            wandSocketInteractor.selectExited.AddListener(LayerChangeReset);
        }
    }

    private void LayerChangeSet(SelectEnterEventArgs arg)
    {
        GameObject wandHead = arg.interactableObject.transform.gameObject;
        baseWandHeadLayer = wandHead.layer;
        wandHead.layer = changeLayer;
        gameObject.layer = changeLayer;
    }

    private void LayerChangeReset(SelectExitEventArgs arg)
    {
        GameObject wandHead = arg.interactableObject.transform.gameObject;
        wandHead.layer = baseWandHeadLayer;
        gameObject.layer = baseWandBodyLayer;
    }



}
