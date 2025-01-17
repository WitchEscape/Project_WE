using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE_LEVEL3_Trigger : MonoBehaviour
{

    public bool isActivate = false;

    void Start()
    {
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(DialogEvent_1);
    }

    private void DialogEvent_1(GameObject arg0)
    {

        if(PuzzleProgressManager.Instance.GetPuzzleState("Puzzle_1") == PuzzleProgressManager.PuzzleState.Available)
        {
            if (!isActivate)
            {
                DialogPlayer.Instance.PlayDialogSequence("LOBBY_01_");
                isActivate = true;
            }
        }
    }
}

