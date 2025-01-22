using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE_Ending_Trigger1 : MonoBehaviour
{
    public bool isActivate = false;
    private void Start()
    {
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(TriggerActivate);
    }

    public void TriggerActivate(GameObject go)
    {

        if (PuzzleProgressManager.Instance.GetPuzzleState("Claass535_Puzzle_01") == PuzzleProgressManager.PuzzleState.Available)
        {
            if (!isActivate)
            {
                DialogPlayer.Instance.PlayDialogSequence("535CLASSROOM_01");
                isActivate = true;
            }
        }
    }
}
