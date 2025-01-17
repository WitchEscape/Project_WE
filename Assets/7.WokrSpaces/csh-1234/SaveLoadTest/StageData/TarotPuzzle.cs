using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace temp
{
    public class TarotPuzzle : MonoBehaviour
    {
        [SerializeField] private string puzzleID = "Puzzle_1";
        [SerializeField] private GameObject cards;

        public TextMeshProUGUI text;

        private void Start()
        {
            StartCoroutine(InitializePuzzle());
        }

        private IEnumerator InitializePuzzle()
        {
            yield return new WaitForEndOfFrame();

            var currentState = PuzzleProgressManager.Instance.GetPuzzleState(puzzleID);
            Debug.Log($"TarotPuzzle state: {currentState}");

            //TODO : 퍼즐 진행 못하게 막기
            //if (!PuzzleProgressManager.Instance.IsPuzzleAvailable(puzzleID))
            //{
            //    Debug.Log($"TarotPuzzle {puzzleID} is not available");
            //    gameObject.SetActive(false);
            //}
            //else
            //{
            //    Debug.Log($"TarotPuzzle {puzzleID} is available");
            //}
        }

        bool iscompleted = false;
        private void Update()
        {
            if (iscompleted == false)
            {
                if (text.text == "1" && PuzzleProgressManager.Instance.GetPuzzleState(puzzleID) == PuzzleProgressManager.PuzzleState.Available)
                {
                    OnPuzzleComplete();
                    iscompleted = true;
                }

            }

        }

        private void OnPuzzleComplete()
        {
            PuzzleProgressManager.Instance.CompletePuzzle(puzzleID);
        }

    }
}

