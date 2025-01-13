using UnityEngine;

public abstract class StageDataBase : MonoBehaviour
{
    [SerializeField] protected string stageID;

    protected virtual void Awake()
    {
        if (PuzzleProgressManager.Instance == null)
        {
            Debug.LogError("PuzzleProgressManager instance not found");
            return;
        }

        InitializePuzzleSequence();
    }

    protected abstract void InitializePuzzleSequence();
} 