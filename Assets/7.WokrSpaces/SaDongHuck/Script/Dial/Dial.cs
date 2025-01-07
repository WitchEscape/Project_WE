using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dial : MonoBehaviour
{
    string answer;
    string result;

    public TextMeshProUGUI[] inputChars;

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
            Debug.Log("서랍이 열렸습니다.");
        }
        else
            return;
    }
}
