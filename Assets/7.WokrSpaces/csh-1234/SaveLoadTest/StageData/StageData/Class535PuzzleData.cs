using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Class535PuzzleData : StageDataBase
{
    [SerializeField] Puzzles Claass535_Puzzle_01;
    [SerializeField] Puzzles Claass535_Puzzle_02;
    [SerializeField] Puzzles Claass535_Puzzle_03;
    [SerializeField] Puzzles Claass535_Puzzle_04;

    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Claass535_Puzzle_01", //=> 3번 자리 책상 비밀번호 퍼즐
            "Claass535_Puzzle_02", //=> 타로 퍼즐
            "Claass535_Puzzle_03", //=> 타로 옆자리 책상 비밀번호 퍼즐4
            "Claass535_Puzzle_04", //=> 교탁밑 상자 퍼즐 => 스크롤 획득

        });
    }

    protected override void Start()
    {
        base.Start();
        Claass535_Puzzle_01.ClearEvent.AddListener(Puzzle_1_Clear);
        Claass535_Puzzle_02.ClearEvent.AddListener(Puzzle_2_Clear);
        Claass535_Puzzle_03.ClearEvent.AddListener(Puzzle_3_Clear);
        Claass535_Puzzle_04.ClearEvent.AddListener(Puzzle_4_Clear);

    }

    private void Puzzle_1_Clear()
    {
        
    }
    private void Puzzle_2_Clear()
    {

    }
    private void Puzzle_3_Clear()
    {

    }
    private void Puzzle_4_Clear()
    {
        DialogPlayer.Instance.PlayDialogSequence("535CLASSROOM_02");
    }
}
