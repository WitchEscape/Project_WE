using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumDial : MonoBehaviour
{
    [SerializeField] private string answer = "BROP"; // 초기값 설정
    [SerializeField] private GameObject uipanel; //닫을 패널

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
            if(uipanel != null)
            {
                uipanel.SetActive(false);
            }
        }
        else
            return;
    }
}
