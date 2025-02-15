using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialChar : MonoBehaviour
{
    //각각의 UI에 A ~ I를 표시하시 위해서 알파벳 저장할 변수 배열
    string[] dialChars;
    //위 배열를 컨트롤하기 위한 배열 인덱스 변수
    int dialCharIndex;

    public TextMeshProUGUI dialCharTxt;
    public DeskDrawerPuzzle deskdrawerpuzzle;

    void Awake()
    {
        dialCharIndex = 0;
        dialChars = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
        "O", "P", "Q", "R", "S" };
        dialCharTxt.text = dialChars[dialCharIndex];
    }

    public void CharUp()
    {
        dialCharIndex++;
        if (dialCharIndex == dialChars.Length)
        {
            dialCharIndex = 0;
            dialCharTxt.text = dialChars[dialCharIndex];
        }
        else
        {
            dialCharTxt.text = dialChars[dialCharIndex];
        }

        deskdrawerpuzzle.AnswerCheck();
    }

    public void CharDown()
    {
        dialCharIndex--;
        if (dialCharIndex < 0)
        {
            dialCharIndex = dialChars.Length - 1;
            dialCharTxt.text = dialChars[dialCharIndex];
        }
        else
        {
            dialCharTxt.text = dialChars[dialCharIndex];
        }

        deskdrawerpuzzle.AnswerCheck();
    }
}
