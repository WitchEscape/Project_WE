using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneTest : MonoBehaviour
{
    //단순 예시용 스크립트
    private void Start()
    {
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(Test);
    }

    public void Test(GameObject obj)
    {
        obj.SetActive(false);
    }
}
