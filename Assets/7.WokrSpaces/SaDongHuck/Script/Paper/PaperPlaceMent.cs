using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaperPlaceMent : MonoBehaviour
{
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
        UnlockPositionAndRotation(); // 놓았을 때 다시 고정
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
