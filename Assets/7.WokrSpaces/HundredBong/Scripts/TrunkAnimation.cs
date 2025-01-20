using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TrunkAnimation : MonoBehaviour
{
    private Animator anim;
    private XRAlyxGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponentInParent<XRAlyxGrabInteractable>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        grabInteractable.activated.AddListener(ChangeAnimation);
    }

    public void ChangeAnimation(ActivateEventArgs arg)
    {
        anim.SetBool("Open", !anim.GetBool("Open"));
    }
}
