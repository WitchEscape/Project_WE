using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryPuzzleData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Puzzle_1",
        });
    }
}
