using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialNum : MonoBehaviour
{
    //각각의 UI에 1 ~ 9를 표시하시 위해서 알파벳 저장할 변수 배열
    string[] dialChars;
    //위 배열를 컨트롤하기 위한 배열 인덱스 변수
    int dialCharIndex;

    public TextMeshProUGUI dialCharTxt;
    public DrawerPuzzle drawerPuzzleManager;

    void Awake()
    {
        dialCharIndex = 0;
        dialChars = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        dialCharTxt.text = dialChars[dialCharIndex];
    }

    public void NumUp()
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

        drawerPuzzleManager.AnswerCheck();
    }

    public void NumDown()
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

        drawerPuzzleManager.AnswerCheck();
    }
}
