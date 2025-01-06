using UnityEngine;

public class CanvasFollow : MonoBehaviour
{
    //캔버스가 카메라를 따라서 항상 플레이어를 바라보도록 함, 자막이나 UI 표시용.

    [Header("메인 카메라")] public Transform playerCamera;
    [Header("따라가는 속도")] public float followSpeed = 2.0f;
    [Header("캔버스 오프셋")] public Vector3 offset = new Vector3(0, 0, 2.0f);

    void LateUpdate()
    {
        //헤드셋 움직임 업데이트 이후 자막 위치를 설정해야 하므로 Late Update

        Vector3 targetPosition = playerCamera.position + playerCamera.forward * offset.z + playerCamera.up * offset.y + playerCamera.right * offset.x;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        //Quaternion.LookRotation은 Unity에서 어떤 객체가 특정 방향을 바라보도록 회전을 계산해줌
        //캔버스가가 메인 카메라를 바라보도록 설정
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - playerCamera.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
    }
}

