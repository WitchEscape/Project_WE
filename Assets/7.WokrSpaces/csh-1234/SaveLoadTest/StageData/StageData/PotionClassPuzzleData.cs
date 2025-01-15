using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionClassPuzzleData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "PotionClass_Puzzle_01",    // => 시험관 퍼즐"
            "PotionClass_Puzzle_02",    // => 포션제조 퍼즐"
            "PotionClass_Puzzle_03",    // => 등불 마법진 퍼즐 => 스크롤 획득"
        });
    }
}
