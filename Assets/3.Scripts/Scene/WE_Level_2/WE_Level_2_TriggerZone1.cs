using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE_Level_2_TriggerZone1 : MonoBehaviour
{
    public bool isActivate = false;
    private void Start()
    {
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(Start_Domitory_Puzzle_01);
    }

    public void Start_Domitory_Puzzle_01(GameObject go)
    {

        if (PuzzleProgressManager.Instance.GetPuzzleState("Domitory_Puzzle_01") == PuzzleProgressManager.PuzzleState.Available)
        {
            if (!isActivate)
            {
                DialogPlayer.Instance.PlayDialogSequence("DORMITORY_02");
                isActivate = true;
            }
        }

    }
}
