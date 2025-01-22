using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE_Level_2_BookOpen : MonoBehaviour
{
    [SerializeField] private BookAnimationControll bookAnim;
    [SerializeField] private bool isActivated = false;

    private void Start()
    {
        bookAnim.CloseEvent.AddListener(StartDialog);
    }

    private void StartDialog()
    {
        if(isActivated == false)
        {
            //if(PuzzleProgressManager.Instance.GetPuzzleState("Domitory_Puzzle_01") == PuzzleProgressManager.PuzzleState.Available)
            //{
            Debug.Log("이벤트 발생함");
                DialogPlayer.Instance.PlayDialogSequence("DORMITORY_01");
                isActivated = true;
            //}
        }
        
    }
}
