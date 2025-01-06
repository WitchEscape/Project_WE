using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        anim.SetFloat("Trigger", triggerValue);
        //Debug.Log($"트리거 입력값 : {triggerValue}");

        float gripValue = gripAnimationAction.action.ReadValue<float>();
        anim.SetFloat("Grip", gripValue);
    }
}