using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

public class Dial : MonoBehaviour
{
    string answer;
    string result;

    public TextMeshProUGUI[] inputChars;
    public GameObject keyprefab;
    public Transform spawnLoation;

    void Awake()
    {
        answer = "BROP";
    }

    public void AnswerCheck()
    {
        result = null;
        for (int i = 0; i < inputChars.Length; i++)
        {
            result += inputChars[i].text;
        }
        if (answer == result)
        {
            //Debug.Log("서랍이 열렸습니다.");
            SpawnItem();
        }
        else
            return;
    }

    private void SpawnItem()
    {
        if (keyprefab != null && spawnLoation != null)
        {
            Instantiate(keyprefab, spawnLoation.position, spawnLoation.rotation);
            print("열쇠가 생성 되었습니다");
        }
    }

   

}
