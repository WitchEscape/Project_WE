using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    //기존 OnTriggerEnter방식을 대신한 방식
    //이벤트만들어 하여 하드코딩을 방지함
    [Header("상호작용할 타겟 태그")] public string[] targetTags;
    public UnityEvent<GameObject> OnEnterEvent;
    public UnityEvent<GameObject> OnExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        //배열에서 태그를 순회하며 비교
        foreach (string tag in targetTags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                OnEnterEvent?.Invoke(other.gameObject);
                return; //태그를 찾았으므로 더 이상 비교하지 않음
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (string tag in targetTags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                OnExitEvent?.Invoke(other.gameObject);
                return;
            }
        }
    }
}

//사용 예시
//public class Test : MonoBehaviour
//{
//    private void Start()
//    {
//        GetComponent<TriggerZone>().OnEnterEvent.AddListener(TriggerTest);
//    }

//    public void TriggerTest(GameObject go)
//    {
//        go.SetActive(false);
//    }
//}
