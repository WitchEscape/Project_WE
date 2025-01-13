using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandData : MonoBehaviour
{
    public enum HandModelType { left, Right }
    public HandModelType handType;

    public Transform root;
    public Animator anim;
    public Transform[] fingerBones;

}
