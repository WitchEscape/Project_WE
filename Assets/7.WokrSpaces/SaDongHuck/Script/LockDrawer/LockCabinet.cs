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
        limits.min = 0;
        limits.max = 123;
        joint.limits = limits;
        joint.useLimits = false;
        print("서랍문이 열렸습니다");
    }
}
