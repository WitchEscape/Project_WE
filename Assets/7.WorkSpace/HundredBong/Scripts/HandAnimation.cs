using System.Collections;
using System.Collections.Generic;
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
        //트리거, 그립 버튼의 입력 값을 받아서 블렌드 트리를 조절함 

        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        anim.SetFloat("Trigger", triggerValue);
        //Debug.Log($"트리거 입력값 : {triggerValue}");

        float gripValue = gripAnimationAction.action.ReadValue<float>();
        anim.SetFloat("Grip", gripValue);
    }
}
