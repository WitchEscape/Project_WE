using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BookAnimationControll : MonoBehaviour
{
    public Animator animator;
    [SerializeField]
    private XRGrabInteractable grab;
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

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (grab == null) grab = GetComponent<XRGrabInteractable>();
        ObjectControll();
        GrabInteractorEventSet();
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
        //grab.activated.AddListener(BookAnimation);
        grab.selectEntered.AddListener(ControllerEventSet);
        grab.selectExited.AddListener(ContollerEventRemove);

        grab.selectExited.AddListener(IsOpenBookCheak);
    }

    private void ContollerEventRemove(SelectExitEventArgs arg)
    {
        ActionBasedController controller = arg.interactorObject.transform.GetComponentInParent<ActionBasedController>();
        controller.scaleToggleAction.action.performed -= ControllerEvent;
    }

    private void ControllerEventSet(SelectEnterEventArgs arg)
    {
        ActionBasedController controller = arg.interactorObject.transform.GetComponentInParent<ActionBasedController>();
        controller.scaleToggleAction.action.performed += ControllerEvent;
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
        }
    }


    private void BookAnimation(ActivateEventArgs arg)
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
            yield return new WaitUntil(() =>
            {
                return Time.time >= animationTime;
            });
            if (animator.GetBool("IsOpen"))
            {
                animator.SetBool("isOpen", false);
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
