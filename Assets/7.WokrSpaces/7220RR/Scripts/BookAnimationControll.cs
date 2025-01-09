using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BookAnimationControll : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private XRGrabInteractable grab;
    [SerializeField]
    private GameObject modelling;
    [SerializeField]
    private GameObject aniObject;
    private bool isGrab;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (grab == null) grab = GetComponent<XRGrabInteractable>();
        ObjectControll();
    }

    private void ObjectControll()
    {
        aniObject.SetActive(isGrab);
        modelling.SetActive(!isGrab);
    }

    private void GrabInteractorEventSet()
    {
        if (grab == null)
        {
            Debug.LogError("BookAnimationControll / GrabInteratable is Null");
            return;
        }

        //grab.selectEntered.AddListener();
    }
}
