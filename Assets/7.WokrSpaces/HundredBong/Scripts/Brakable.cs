using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brakable : MonoBehaviour
{
    [Header("부서진 모델 프리팹")] public GameObject brokenObjectPrefab;
    [Header("이벤트에 사용할 태그")] public string colliderTag;

    private bool wasDestroyed;

    [Serializable] public class BreakEvent : UnityEvent<GameObject, GameObject> { }
    [SerializeField] private BreakEvent m_OnBreak = new BreakEvent();
    public BreakEvent onBreak => m_OnBreak;

    private void OnCollisionEnter(Collision other)
    {
        //충돌이 감지되면 깨진 모델 활성화 및 기존 모델 제거 
        if (wasDestroyed) return;

        if (other.transform.gameObject.CompareTag($"{colliderTag}"))
        {
            wasDestroyed = true;
            GameObject brokenObject = Instantiate(brokenObjectPrefab, transform.position, transform.rotation);
            m_OnBreak.Invoke(other.gameObject, brokenObject);
            Destroy(gameObject);
        }
    }
}
