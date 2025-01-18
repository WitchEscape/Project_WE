using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_DataLoadButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private TextMeshProUGUI saveTimeText;
    [SerializeField] private Button loadButton;
    //[SerializeField] private Button deleteButton;

    private string stageName;
    private Action<string> onLoadCallback;
    private Action<string> onDeleteCallback;

    private void Awake()
    {
        if (stageNameText == null || saveTimeText == null || loadButton == null /*|| deleteButton == null*/)
        {
            Debug.LogError($"Missing component references in {gameObject.name}");
        }
    }

    public void Initialize(string stageName, DateTime saveTime, Action<string> onLoad, Action<string> onDelete)
    {
        this.stageName = stageName;
        this.onLoadCallback = onLoad;
        this.onDeleteCallback = onDelete;

        if (stageNameText != null)
        {
            stageNameText.text = stageName;
        }

        if (saveTimeText != null)
        { 
            saveTimeText.text = saveTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => this.onLoadCallback?.Invoke(this.stageName));
        }
    }

    public void OnLoadButtonClick()
    {
        onLoadCallback?.Invoke(stageName);
    }
}
