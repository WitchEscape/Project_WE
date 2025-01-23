using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReciprocatingMotion : MonoBehaviour
{
    [Header("이동 속도, 최대 운동량")]
    public float mooveSpeed = 2f;
    public float moveAmplitude = 0.2f;

    [Header("리셋후 컨트롤러 높이"), Range(1, 2)] public float height; 
    //[Header("이동할 축")]
    //public bool dirX;
    //public bool dirY;
    //public bool dirZ;

    public bool project_WE_Tutorial;
    private void Update()
    {

        if (project_WE_Tutorial)
        {
            float dir = Mathf.Sin(Time.time * mooveSpeed) * moveAmplitude * 0.01f;
            transform.position = new Vector3(transform.position.x, transform.position.y + dir, transform.position.z);
        }

        //if (dirX)
        //    transform.rotation = new Quaternion(transform.rotation.x + dir, transform.rotation.y, transform.rotation.z, transform.rotation.w);

        //if (dirY)
        //    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + dir, transform.rotation.z, transform.rotation.w);

        //if (dirZ)
        //    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z + dir, transform.rotation.w);

        //if (project_WE_Tutorial)
    }

    public void SetBoolTrue()
    {
        project_WE_Tutorial = true;
    }

    public void SetBoolFalse()
    {
        project_WE_Tutorial = false;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, height, gameObject.transform.position.z);

    }
}
