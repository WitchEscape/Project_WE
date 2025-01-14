using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE_Level_03_TestData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Puzzle_1",
            "Puzzle_2",
            "Puzzle_3",
        });
    }
}
