using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleDial : MonoBehaviour
{
    [SerializeField] // 인스펙터 창에서 설정 가능
    private string answer = "BROP"; // 초기값 설정

    private string result;

    public TextMeshProUGUI[] inputChars;

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
