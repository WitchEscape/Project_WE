using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryPuzzleData : StageDataBase
{
    [SerializeField] private BookAnimationControll bookOpen;
    
    private bool isActivated = false;

    protected override void Start()
    {
        base.Start();
        bookOpen.OpenEvent.AddListener(PlayDialog);
    }

    private void PlayDialog()
    {
        if (isActivated == false)
        {
            if (PuzzleProgressManager.Instance.GetPuzzleState("Library_Puzzle_01") == PuzzleProgressManager.PuzzleState.Completed)
            {
                DialogPlayer.Instance.PlayDialogSequence("LIBRARY_02");
                isActivated = true;
            }
        }
    }

    protected override void InitializePuzzleSequence()
    {
        PuzzleProgressManager.Instance.SetPuzzleSequence(new List<string>
        {
            "Library_Puzzle_01",   // => 책 배치 퍼즐 => 빛나는 책 속 스크롤 획득"
        });
    }
}
