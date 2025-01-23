using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class JustLookPlayer : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        //Talk 상태 동안 플레이어를 바라보게 설정
        Vector3 direction = mainCamera.transform.position - transform.position;
        //Y축 제거
        direction.y = 0;
        //방향이 0이 아닌 경우에만 회전
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
