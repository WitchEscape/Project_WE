using UnityEngine;

public abstract class StageDataBase : MonoBehaviour
{
    [SerializeField] protected string stageID;

    protected virtual void Start()
    {
        if (PuzzleProgressManager.Instance == null)
        {
            Debug.LogError("PuzzleProgressManager instance not found");
            return;
        }

        // PuzzleProgressManager가 초기화될 때까지 대기
        if (!PuzzleProgressManager.Instance.IsInitialized)
        {
            Debug.LogWarning("PuzzleProgressManager is not yet initialized");
            return;
        }

        InitializePuzzleSequence();
    }

    protected abstract void InitializePuzzleSequence();
} 