using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachersRoomPuzzleData : StageDataBase
{
    [SerializeField] private Puzzles TeachersRoom_Puzzle_03;
    [SerializeField] private BookAnimationControll bookOpen;

    protected override void Start()
    {
        base.Start();
        TeachersRoom_Puzzle_03.ClearEvent.AddListener(TeachersRoom_Puzzle_03_Event);
        bookOpen.OpenEvent.AddListener(LaffleBookOpen);
    }

    
    private void TeachersRoom_Puzzle_03_Event()
    {
        if(PuzzleProgressManager.Instance.GetPuzzleState("TeachersRoom_Puzzle_03") == PuzzleProgressManager.PuzzleState.Completed)
        {
            DialogPlayer.Instance.PlayDialogSequence("TEACHERSROOM_03");
        }
    }
    private void LaffleBookOpen()
    {
        if (PuzzleProgressManager.Instance.GetPuzzleState("TeachersRoom_Puzzle_03") == PuzzleProgressManager.PuzzleState.Completed)
        {
            DialogPlayer.Instance.PlayDialogSequence("TEACHERSROOM_04");
        }
    }


    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "TeachersRoom_Puzzle_01",   // => 칠판 캐비닛 비밀번호 퍼즐
            "TeachersRoom_Puzzle_02",   // => 첫번째 책상 서랍 열쇠퍼즐
            "TeachersRoom_Puzzle_03"    // => 서랍 비밀번호 퍼즐 => 스크롤 획득,
        });
    }
}
