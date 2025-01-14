using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonRoomPuzzleData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Puzzle_1",
            "Puzzle_2",
            "Puzzle_3",
            "Puzzle_4",
        });
    }
}
