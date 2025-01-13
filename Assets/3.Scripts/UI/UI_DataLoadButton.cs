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
        if (stageNameText == null)
            stageNameText = transform.Find("StageNameText")?.GetComponent<TextMeshProUGUI>();
        if (saveTimeText == null)
            saveTimeText = transform.Find("SaveTimeText")?.GetComponent<TextMeshProUGUI>();
        if (loadButton == null)
            loadButton = transform.Find("LoadButton")?.GetComponent<Button>();
        //if (deleteButton == null)
        //    deleteButton = transform.Find("DeleteButton")?.GetComponent<Button>();

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
            stageNameText.text = stageName;
        if (saveTimeText != null)
            saveTimeText.text = saveTime.ToString("yyyy-MM-dd HH:mm:ss");

        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => this.onLoadCallback?.Invoke(this.stageName));
            Debug.Log($"Load button initialized for stage: {stageName}");
        }
    }

    public void OnLoadButtonClick()
    {
        Debug.Log($"Load button clicked for stage: {stageName}");
        onLoadCallback?.Invoke(stageName);
    }
}
