using System.Collections.Generic;
using UnityEngine;

public class PuzzleProgressManager : MonoBehaviour
{
    private static PuzzleProgressManager instance;
    public static PuzzleProgressManager Instance => instance;

    [SerializeField] public List<string> puzzleSequence;

    private Dictionary<string, PuzzleData> puzzleStates = new Dictionary<string, PuzzleData>();

    public enum PuzzleState
    {
        Locked,      //0
        Available,   //1
        InProgress,  //2
        Completed    //3
    }

    [System.Serializable]
    public class PuzzleData
    {
        public string puzzleID;
        public PuzzleState state;
        public string nextPuzzleID;  
        public Dictionary<string, object> progressData = new Dictionary<string, object>();
    }
 
    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        isInitialized = true;
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

    /// <summary>
    /// 퍼즐이 풀 수 있는 상태인지 리턴하는 메서드
    /// </summary>
    /// <param name="puzzleID">퍼즐 이름</param>
    /// <returns></returns>
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

    public void SettPuzzleState(string puzzleID, PuzzleState state)
    {
        if (puzzleStates.TryGetValue(puzzleID, out var puzzleData) == true)
        {
            puzzleData.state = state;   
        }
        else
        {
            Debug.Log("Invalid puzzleId");
        }
    }
 

    public void SetPuzzleSequence(List<string> sequence)
    {
        puzzleSequence = sequence;
        InitializePuzzles();
    }

    #region Save&Load
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
    #endregion

    // 씬 이동시 호출 해주기
    public void ClearPuzzleProgress()
    {
        puzzleStates.Clear();
        InitializePuzzles();
    }
    
}
