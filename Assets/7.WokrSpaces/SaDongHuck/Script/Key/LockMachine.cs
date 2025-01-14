using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LockMachine : MonoBehaviour
{

    public XRSocketInteractor socketInteractor; // 자물쇠의 소켓
    public GameObject lockObject;               // 자물쇠 오브젝트 (애니메이션 or 상태 변경)
    public Animator lockAnimator; // 자물쇠의 애니메이터
    public GameObject Key;

    [SerializeField] private LockDeskDrawer lockDeskDrawer;
    private void Start()
    {
        // 소켓 이벤트 등록
        socketInteractor.selectEntered.AddListener(OnKeyInserted);
        lockDeskDrawer = FindObjectOfType<LockDeskDrawer>();
        if(lockDeskDrawer != null )
        {
            print("lockDeskDrawer을 찾을 수 없음");
        }
    }

    private void OnKeyInserted(SelectEnterEventArgs args)
    {
        // 열쇠가 소켓에 들어갔을 때 실행
        Debug.Log("Key inserted!");
        OpenLock(); // 자물쇠 열기 함수 호출
        Destroy(lockObject, 2f);
        Destroy(Key, 1f);
        lockDeskDrawer.UnLockDrawer();
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
