using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDeskDrawer : MonoBehaviour
{
    private ConfigurableJoint joint;
    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        LockDrawer();
    }

    public void LockDrawer()
    {
        joint.xMotion = ConfigurableJointMotion.Locked;
    }
    public void UnLockDrawer()
    {
        joint.xMotion = ConfigurableJointMotion.Limited;
    }
}
