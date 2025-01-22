using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE_Level_4_TriggerZone1 : MonoBehaviour
{
    public bool isActivate = false;
    private void Start()
    {
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(StartDialog);
    }

    public void StartDialog(GameObject go)
    {

        if (PuzzleProgressManager.Instance.GetPuzzleState("Library_Puzzle_01") == PuzzleProgressManager.PuzzleState.Available)
        {
            if (isActivate == false)
            {
                DialogPlayer.Instance.PlayDialogSequence("LIBRARY_01");
                isActivate = true;
            }
        }
    }
}
