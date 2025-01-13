using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockBox : MonoBehaviour
{
    private ConfigurableJoint joint;

    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();

        Lockbox();
    }

    public void Lockbox()
    {
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        print("상자가 잠겼습니다");
    }

    public void UnLockbox()
    {
        joint.angularXMotion = ConfigurableJointMotion.Limited;
    }
}
