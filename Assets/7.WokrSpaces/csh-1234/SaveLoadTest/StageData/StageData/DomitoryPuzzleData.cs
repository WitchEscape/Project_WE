using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomitoryPuzzleData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Domitory_Puzzle_01",   // => 포스터 밑 서랍장 비밀번호 퍼즐
            "Domitory_Puzzle_02",   // => 꽃 색 입력퍼즐
            "Domitory_Puzzle_03",   // => 타로 옆자리 비밀번호 퍼즐
            "Domitory_Puzzle_03",   // => 교탁밑 상자 퍼즐 => 스크롤, 부러진 지팡이 획득
        });
    }
}
