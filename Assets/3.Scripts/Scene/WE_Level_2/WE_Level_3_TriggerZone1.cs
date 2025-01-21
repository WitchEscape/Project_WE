using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE_Level_3_TriggerZone1 : MonoBehaviour
{
    public bool isActivate = false;
    private void Start()
    {
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(Start_POTIONROOM_01);
    }

    public void Start_POTIONROOM_01(GameObject go)
    {

        if (PuzzleProgressManager.Instance.GetPuzzleState("PotionClass_Puzzle_01") == PuzzleProgressManager.PuzzleState.Available)
        {
            if (!isActivate)
            {
                DialogPlayer.Instance.PlayDialogSequence("POTIONROOM_01");
                isActivate = true;
            }
        }

    }
}
