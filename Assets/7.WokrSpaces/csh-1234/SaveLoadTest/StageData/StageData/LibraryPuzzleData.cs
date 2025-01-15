using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryPuzzleData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Library_Puzzle_01,",   // => 책 배치 퍼즐 => 빛나는 책 속 스크롤 획득"
        });
    }
}
