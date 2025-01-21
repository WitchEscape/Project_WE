using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomitoryPuzzleData : StageDataBase
{

    [SerializeField] Puzzles Domitory_Puzzle_03;
    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Domitory_Puzzle_01",   // => 포스터 밑 서랍장 비밀번호 퍼즐
            "Domitory_Puzzle_02",   // => 꽃 색 입력퍼즐, 열쇠 획득
            "Domitory_Puzzle_03",   // => 열쇠로 서랍열면 지팡이 조각 2개, 스크롤
        });
    }

    protected override void Start()
    {
        base.Start();
        Domitory_Puzzle_03.ClearEvent.AddListener(Puzzle_3_Clear);

    }

    private void Puzzle_3_Clear()
    {
        DialogPlayer.Instance.PlayDialogSequence("DORMITORY_03");
    }
}
