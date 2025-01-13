using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LockMachine : MonoBehaviour
{
    /*public Animator lockAnimator; // 자물쇠의 애니메이터
    public GameObject lockObject; // 자물쇠 오브젝트
    public GameObject Object;

    private void OnTriggerEnter(Collider other)
    {
        // 열쇠의 태그 확인
        if (other.CompareTag("Key"))
        {
            // 열리는 애니메이션 실행
            if (lockAnimator != null)
            {
                lockAnimator.SetBool("IsUnLock", true);
            }

            // 3초 뒤에 자물쇠 삭제
            DestroyLock(); ;
        }
    }

    private void DestroyLock()
    {
        Destroy(Object, 2f);
    }*/

    public XRSocketInteractor socketInteractor; // 자물쇠의 소켓
    public GameObject lockObject;               // 자물쇠 오브젝트 (애니메이션 or 상태 변경)
    public Animator lockAnimator; // 자물쇠의 애니메이터
    private void Start()
    {
        // 소켓 이벤트 등록
        socketInteractor.selectEntered.AddListener(OnKeyInserted);
    }

    private void OnKeyInserted(SelectEnterEventArgs args)
    {
        // 열쇠가 소켓에 들어갔을 때 실행
        Debug.Log("Key inserted!");
        OpenLock(); // 자물쇠 열기 함수 호출
    }

    private void OpenLock()
    {
        // 자물쇠를 여는 로직 (애니메이션 or 상태 변경)
        if (lockObject != null)
        {
            if (lockAnimator != null)
            {
                lockAnimator.SetBool("IsUnLock",true); // "Open" 트리거 애니메이션 실행
            }
            Debug.Log("Lock is opened!");
        }
    }

    private void OnDestroy()
    {
        // 이벤트 제거
        socketInteractor.selectEntered.RemoveListener(OnKeyInserted);
    }
}
