using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examiner : MonoBehaviour
{
    private GhostCanvas ghostCanvas;

    public bool[] isCorrectPostion = new bool[3];

    private bool isComplite = false;
    private bool isCleared = false; 
    private void Awake()
    {
        ghostCanvas = FindObjectOfType<GhostCanvas>();
    }
        
    public void SetPostionEnter(int i)
    {
        isCorrectPostion[i] = true;
        isComplite = CheckCorrectPostion();
        if (isComplite && isCleared == false)
        {
            //TODO : 발생할 이벤트 작성
            Debug.Log("포션 퍼즐 클리어");
            isCleared = true;
            ghostCanvas.ClearPuzzle(0);
        }
    }

    public void SetPostionExit(int i)
    {
        isCorrectPostion[i] = false;

    }

    private bool CheckCorrectPostion()
    {
        return isCorrectPostion[0] && isCorrectPostion[1] && isCorrectPostion[2];
    }
}
