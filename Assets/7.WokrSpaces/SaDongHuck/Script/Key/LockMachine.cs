using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockMachine : MonoBehaviour
{
    public Animator lockAnimator; // 자물쇠의 애니메이터
    public GameObject lockObject; // 자물쇠 오브젝트

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
            Invoke("DestroyLock", 3f);
        }
    }

    private void DestroyLock()
    {
        Destroy(lockObject);
    }
}
