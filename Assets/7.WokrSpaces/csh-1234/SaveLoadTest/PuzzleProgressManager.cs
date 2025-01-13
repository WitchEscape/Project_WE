using System.Collections.Generic;
using UnityEngine;

public class PuzzleProgressManager : MonoBehaviour
{
    private static PuzzleProgressManager instance;
    public static PuzzleProgressManager Instance => instance;

    [SerializeField] private List<string> puzzleSequence;

    private Dictionary<string, PuzzleData> puzzleStates = new Dictionary<string, PuzzleData>();

    public enum PuzzleState
    {
        Locked,      
        Available,   
        InProgress,  
        Completed    
    }

    [System.Serializable]
    public class PuzzleData
    {
        public string puzzleID;
        public PuzzleState state;
        public string nextPuzzleID;  
        public Dictionary<string, object> progressData = new Dictionary<string, object>();
    }
 
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        InitializePuzzles();
    }

    // 씬 로드할때 마다 초기화 해줘야 할듯?
    private void InitializePuzzles()
    {
        for (int i = 0; i < puzzleSequence.Count; i++)
        {
            var puzzleData = new PuzzleData
            {
                puzzleID = puzzleSequence[i],
                state = i == 0 ? PuzzleState.Available : PuzzleState.Locked,
                nextPuzzleID = i < puzzleSequence.Count - 1 ? puzzleSequence[i + 1] : null
            };
            puzzleStates[puzzleSequence[i]] = puzzleData;
        }
    }

    public void CompletePuzzle(string puzzleID)
    {
        Debug.Log($"{puzzleID} 성공함");
        if (puzzleStates.TryGetValue(puzzleID, out var puzzleData))
        {
            puzzleData.state = PuzzleState.Completed;
            UnlockNextPuzzle(puzzleID);
        }
    }

    private void UnlockNextPuzzle(string currentPuzzleID)
    {
        PuzzleData currentPuzzle = puzzleStates[currentPuzzleID];
        if (!string.IsNullOrEmpty(currentPuzzle.nextPuzzleID))
        {
            if (puzzleStates.TryGetValue(currentPuzzle.nextPuzzleID, out var nextPuzzle))
            {
                nextPuzzle.state = PuzzleState.Available;
            }
        }
    }

    public bool IsPuzzleAvailable(string puzzleID)
    {
        if(puzzleStates.TryGetValue(puzzleID, out var puzzleData) && puzzleData.state == PuzzleState.Available)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SavePuzzleProgress(Dictionary<string, object> data)
    {
        if (data != null)
        {
            data["puzzleStates"] = puzzleStates;
        }
    }

    public void LoadPuzzleProgress(Dictionary<string, object> data)
    {
        if (data != null && data.TryGetValue("puzzleStates", out object states))
        {
            puzzleStates = (Dictionary<string, PuzzleData>)states;
        }
    }

    public PuzzleState GetPuzzleState(string puzzleID)
    {
        if(puzzleStates.TryGetValue(puzzleID, out var puzzleData) == true)
        {
            return puzzleData.state;
        }
        else
        {
            return PuzzleState.Locked;
        }
    }

    public void SetPuzzleProgressData(string puzzleID, string key, object value)
    {
        if (puzzleStates.TryGetValue(puzzleID, out var puzzleData))
        {
            puzzleData.progressData[key] = value;
        }
    }

    public void SetPuzzleSequence(List<string> sequence)
    {
        puzzleSequence = sequence;
        InitializePuzzles();
    }
}
