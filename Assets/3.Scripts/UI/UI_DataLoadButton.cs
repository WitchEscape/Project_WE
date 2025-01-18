using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class UI_DataLoadButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private TextMeshProUGUI saveTimeText;

    [SerializeField] private Button loadButton;
    [SerializeField] private GameObject focus;
    //[SerializeField] private Button deleteButton;

    private string stageName;
    private Action<string> onLoadCallback;
    private Action<string> onDeleteCallback;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        focus.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        focus.gameObject.SetActive(false);
    }
}
