using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachersRoomPuzzleData : StageDataBase
{
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "TeachersRoom_Puzzle_01",   // => 칠판 캐비닛 비밀번호 퍼즐
            "TeachersRoom_Puzzle_02",   // => 첫번째 책상 서랍 열쇠퍼즐
            "TeachersRoom_Puzzle_03",   // => 만년필 잉크 퍼즐
            "TeachersRoom_Puzzle_04"    // => 서랍 비밀번호 퍼즐 => 스크롤 획득,
        });
    }
}
