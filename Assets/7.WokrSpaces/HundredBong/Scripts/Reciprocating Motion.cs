using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReciprocatingMotion : MonoBehaviour
{
    [Header("이동 속도, 최대 운동량")]
    public float mooveSpeed = 2f;
    public float moveAmplitude = 0.2f;

    //[Header("이동할 축")]
    //public bool dirX;
    //public bool dirY;
    //public bool dirZ;

    public bool project_WE_Tutorial;
    private void Update()
    {
        float dir = Mathf.Sin(Time.time * mooveSpeed) * moveAmplitude * 0.01f;

        //if (dirX)
        //    transform.rotation = new Quaternion(transform.rotation.x + dir, transform.rotation.y, transform.rotation.z, transform.rotation.w);

        //if (dirY)
        //    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + dir, transform.rotation.z, transform.rotation.w);

        //if (dirZ)
        //    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z + dir, transform.rotation.w);

        //if (project_WE_Tutorial)
            transform.position = new Vector3(transform.position.x, transform.position.y + dir, transform.position.z);
    }
}
