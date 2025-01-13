using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCabinet : MonoBehaviour
{
    private HingeJoint joint;
    private bool isLokced = true;
    private void Start()
    {
        joint = GetComponent<HingeJoint>();
        LockDrawer();
    }

    public void LockDrawer()
    {
        isLokced = true;
        JointLimits limits = joint.limits;
        limits.min = 0;
        limits.max = 0;
        joint.limits = limits;
        joint.useLimits = true;
        print("서랍문이 잠겼음");
    }
    public void UnLockDrawer()
    {
        isLokced = false;
        JointLimits limits = joint.limits;
        limits.min = 0f;
        limits.max = 123.2095f;
        joint.limits = limits;
        joint.useLimits = true;
        print("서랍문이 열렸습니다");

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.WakeUp(); // Rigidbody 깨우기
        }

        print($"UnLockDrawer: useLimits={joint.useLimits}, limits.min={limits.min}, limits.max={limits.max}");
    }
}
