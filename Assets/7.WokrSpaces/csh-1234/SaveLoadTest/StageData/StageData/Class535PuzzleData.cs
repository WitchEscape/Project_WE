using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class535PuzzleData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Claass535_Puzzle_01", //=> 3번 자리 책상 비밀번호 퍼즐
            "Claass535_Puzzle_02", //=> 타로 퍼즐
            "Claass535_Puzzle_03", //=> 타로 옆자리 책상 비밀번호 퍼즐4
            "Claass535_Puzzle_03", //=> 교탁밑 상자 퍼즐 => 스크롤 획득

        });
    }
}
