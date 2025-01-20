using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
            stageNameText.text = ConvertStageName(stageName);
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

    private string ConvertStageName(string stageName)
    {
        if(stageName == "WE_Level_1")
        {
            return "535교실";
        }
        else if (stageName == "WE_Level_2")
        {
            return "기숙사";
        }
        else if (stageName == "WE_Level_3")
        {
            return "마법약 교실";
        }
        else if (stageName == "WE_Level_4")
        {
            return "도서관";
        }
        else if (stageName == "WE_Level_5")
        {
            return "교무실";
        }
        else if (stageName == "WE_Level_Tuturial")
        {
            return "로비";
        }
        else
        {
            return "UNKNOWN";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (focus != null)
        {
            focus.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (focus != null)
        {
            focus.SetActive(false);
        }
    }
}