using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionClassPuzzleData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "PUZZLE_1",
            "PUZZLE_2",
            "PUZZLE_3",
        });
    }
}
