using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CardManager : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 중력 비활성화
        rb.isKinematic = true;

        // Y축 위치와 회전 고정
        //rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY;
    }

    // XR Grab Interactable로 잡았을 때 동작
    public void OnGrabbed()
    {
        // 잡았을 때 고정을 해제하고 중력 활성화
        rb.isKinematic = false;
        //rb.constraints = RigidbodyConstraints.None; // 모든 제약 해제
    }

    // XR Grab Interactable에서 놓았을 때 동작
    public void OnReleased()
    {
        // 중력 활성화 상태를 유지하며 Y축만 고정
        rb.isKinematic = false;
    }
}
