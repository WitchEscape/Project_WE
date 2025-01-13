using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyPlaceMent : MonoBehaviour
{
    /*private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 초기에는 위치와 회전 고정
        rb.isKinematic = true; // 물리 효과 비활성화
        LockPositionAndRotation(); // 위치와 회전 고정
    }

    // 열쇠를 서랍에서 가져갈 때 고정을 해제하고 중력을 활성화
    public void ActivateKey()
    {
        rb.isKinematic = false;       // 물리 효과 활성화
        rb.useGravity = true;         // 중력 활성화
        UnlockPositionAndRotation(); // 고정 해제
    }

    // 위치와 회전을 고정
    private void LockPositionAndRotation()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll; // 모든 위치와 회전 고정
    }

    // 위치와 회전을 고정 해제
    private void UnlockPositionAndRotation()
    {
        rb.constraints = RigidbodyConstraints.None; // 모든 고정 해제
    }*/

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // 초기 위치와 회전 고정
        LockPositionAndRotation();

        rb.isKinematic = true;

        // XR Grab 이벤트 연결
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    // 위치와 회전을 고정
    private void LockPositionAndRotation()
    {
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
       
    }

    // 위치와 회전 고정을 해제
    private void UnlockPositionAndRotation()
    {
        rb.constraints = RigidbodyConstraints.None;
       
    }

    // 열쇠를 잡았을 때 호출
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        UnlockPositionAndRotation(); // 잡을 때 고정 해제
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    // 열쇠를 놓았을 때 호출
    private void OnReleased(SelectExitEventArgs args)
    {
        LockPositionAndRotation(); // 놓았을 때 다시 고정
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void OnDestroy()
    {
        // 이벤트 연결 해제 (필수)
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
