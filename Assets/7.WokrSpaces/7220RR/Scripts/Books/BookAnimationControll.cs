using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BookAnimationControll : MonoBehaviour
{
    public Animator animator;
    [SerializeField]
    private XRBaseInteractable grab;
    [SerializeField]
    private GameObject modelling;
    [SerializeField]
    private GameObject aniObject;
    [SerializeField]
    private AnimationClip openClip;
    [SerializeField]
    private AnimationClip closeClip;
    private bool isGrab;
    private float animationTime;
    [SerializeField]
    private Activated activated;
    [SerializeField, Header("페이지 프리팹")] private XRAlyxGrabInteractable page;
    [SerializeField, Header("페이지 프리팹 Rigid")] private Rigidbody pageR;

    public UnityEvent CloseEvent;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (grab == null) grab = GetComponent<XRGrabInteractable>();
        if (activated == null) activated = GetComponent<Activated>();
        ObjectControll();
        GrabInteractorEventSet();

        if(page != null) page.enabled = false;
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
        grab.selectEntered.AddListener((x) =>
        {
            isGrab = true;
            ObjectControll();
        });
        //grab.selectEntered.AddListener(ControllerEventSet);
        //grab.selectExited.AddListener(ContollerEventRemove);

        grab.selectEntered.AddListener(ControllerEventSetMk1);
        grab.selectExited.AddListener(ContollerEventRemoveMk1);

        grab.activated.AddListener((x) => { BookAnimation();
            if (page != null && page.enabled == false)
            {
                print("실행됨");
                page.enabled = true;
                page.selectExited.AddListener((x) => { page.transform.SetParent(null); pageR.isKinematic = false; } );
                //page.selectEntered.AddListener((x)=> pageR.isKinematic = false);
            }
        });

        grab.selectExited.AddListener(IsOpenBookCheak);
    }

    private void ContollerEventRemove(SelectExitEventArgs arg)
    {
        ActionBasedController controller = arg.interactorObject.transform.GetComponentInParent<ActionBasedController>();
        if (controller != null)
            controller.scaleToggleAction.action.performed -= ControllerEvent;
    }

    private void ControllerEventSet(SelectEnterEventArgs arg)
    {
        ActionBasedController controller = arg.interactorObject.transform.GetComponentInParent<ActionBasedController>();
        if (controller != null)
            controller.scaleToggleAction.action.performed += ControllerEvent;
    }


    private void ContollerEventRemoveMk1(SelectExitEventArgs arg)
    {
        activated.ActivateUI(false);
        ActionBasedController controller = arg.interactorObject.transform.GetComponentInParent<ActionBasedController>();
        if (controller != null)
            controller.scaleToggleAction.action.performed -= ControllerEventMk1;
    }

    private void ControllerEventSetMk1(SelectEnterEventArgs arg)
    {
        ActionBasedController controller = arg.interactorObject.transform.GetComponentInParent<ActionBasedController>();
        if (controller != null)
            controller.scaleToggleAction.action.performed += ControllerEventMk1;

    }

    private void ControllerEventMk1(InputAction.CallbackContext call)
    {
        activated.ActivateUI();
    }




    private void ControllerEvent(InputAction.CallbackContext call)
    {
        BookAnimation();

        
    }

    private void BookAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("BookAnimationController / Animator is Null");
            return;
        }
        if (closeClip == null || openClip == null)
        {
            Debug.LogError("BookAnimationController / AnimationClip is Null");
            return;
        }

        if (Time.time >= animationTime)
        {
            print(animator.GetBool("IsOpen"));
            animator.SetBool("IsOpen", !animator.GetBool("IsOpen"));
            animationTime = Time.time + (animator.GetBool("IsOpen") ? openClip.length : closeClip.length);
            if (!animator.GetBool("IsOpen"))
            {
                activated.ActivateUI(false);
                CloseEvent?.Invoke();
            }
        }
    }

    private void IsOpenBookCheak(SelectExitEventArgs arg)
    {
        isGrab = false;

        if (animator == null)
        {
            Debug.LogError("BookAnimationController / Animator is Null");
            return;
        }
        if (closeClip == null || openClip == null)
        {
            Debug.LogError("BookAnimationController / AnimationClip is Null");
            return;
        }

        if (aniObject.activeSelf && animator.GetBool("IsOpen"))
            if (Time.time >= animationTime)
            {
                _ = StartCoroutine(BookCloseCouortine());
            }
            else
            {
                ObjectControll();
            }
    }

    private IEnumerator BookCloseCouortine()
    {
        while (true)
        {
            if (!animator.GetBool("IsOpen")) activated.ActivateUI(false);
            yield return new WaitUntil(() =>
            {
                return Time.time >= animationTime;
            });
            if (animator.GetBool("IsOpen"))
            {
                animator.SetBool("IsOpen", false);
                animationTime = Time.time + closeClip.length;                
            }
            else
            {
                ObjectControll();
                yield break;
            }
        }
    }
}
