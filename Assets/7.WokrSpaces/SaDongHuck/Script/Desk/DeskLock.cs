using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskLock : MonoBehaviour
{
    private ConfigurableJoint joint;
    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        LockDrawer();
    }

    public void LockDrawer()
    {
        joint.zMotion = ConfigurableJointMotion.Locked;
    }
    public void UnLockDrawer()
    {
        joint.zMotion = ConfigurableJointMotion.Limited;

        /*Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.WakeUp(); // Rigidbody를 깨움
        }*/

        Debug.Log("서랍 잠금 해제 완료");
    }
}
