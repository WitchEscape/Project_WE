using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DataLoadButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private TextMeshProUGUI saveTimeText;

    [SerializeField] private Button loadButton;
    [SerializeField] private GameObject focus;

    [SerializeField] private List<Sprite> images;
    [SerializeField] private Image stageImage;
    //[SerializeField] private Button deleteButton;

    private string stageName;
    private Action<string> onLoadCallback;
    private Action<string> onDeleteCallback;

    private void Awake()
    {
        if (stageImage == null)
        {
            stageImage = GetComponent<Image>();
        }
    }

    public void Initialize(string stageName, DateTime saveTime, Action<string> onLoad, Action<string> onDelete)
    {
        this.stageName = stageName;
        this.onLoadCallback = onLoad;
        this.onDeleteCallback = onDelete;

        if (stageNameText != null)
        {
            stageNameText.text = ConvertStageName(stageName);
            Debug.Log(stageName);
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

        switch(stageName)
        {
            case "WE_Level_1":
                stageImage.sprite = images[0];
                return "535교실";
            case "WE_Level_2":
                stageImage.sprite = images[1];
                return "기숙사";
            case "WE_Level_3":
                stageImage.sprite = images[2];
                return "마법약 교실";
            case "WE_Level_4":
                stageImage.sprite = images[3];
                return "도서관";
            case "WE_Level_5":
                stageImage.sprite = images[4];
                return "교무실";
            default:
                stageImage.sprite = images[0];
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