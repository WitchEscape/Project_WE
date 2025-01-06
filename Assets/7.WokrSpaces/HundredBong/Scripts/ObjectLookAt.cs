using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLookAt : MonoBehaviour
{
    //툴팁 or 말풍선이 표시될 때 플레이어를 바라보도록 설정

    [Header("메인 카메라")] public Transform target; 

    private void Update()
    {
        transform.LookAt(target, Vector3.up);
    }
}
