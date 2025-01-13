using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStageManager : MonoBehaviour
{
    [SerializeField] List<GameObject> PuzzleList = new List<GameObject>();
    [SerializeField] private GameObject currentPuzzle;

    public event Action<int> OnCurrentPuzzleChanged;
    //public void AddGold(int addAmount)
    //{
    //    //OnCurrentPuzzleChanged?.Invoke();
    //}

}
