using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examiner : MonoBehaviour
{
    private GhostCanvas ghostCanvas;

    public bool[] isCorrectPostion = new bool[3];

    private bool isComplite = false;
    private bool isCleared = false;

    private TriggerZone[] triggerZone = new TriggerZone[3];
    private void Awake()
    {
        ghostCanvas = FindObjectOfType<GhostCanvas>();
        triggerZone = GetComponentsInChildren<TriggerZone>();
    }

    private void OnEnable()
    {
        for (int i = 0; triggerZone.Length > i; i++)
        {
            int index = i;
            triggerZone[index].OnEnterEvent.AddListener(x => SetPostionEnter(index));
            triggerZone[index].OnExitEvent.AddListener(x => SetPostionExit(index));
        }
    }

    private void OnDisable()
    {
        for (int i = 0; triggerZone.Length > i; i++)
        {
            int index = i;
            triggerZone[index].OnEnterEvent.RemoveListener(x => SetPostionEnter(index));
            triggerZone[index].OnExitEvent.RemoveListener(x => SetPostionExit(index));
        }
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
