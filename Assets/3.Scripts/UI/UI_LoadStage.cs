using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LoadStage : MonoBehaviour
{
    [SerializeField] private UI_DataLoadButton buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    [SerializeField] private Button selectStageButtons;
    [SerializeField] private Button loadStageButton;

    [SerializeField] private GameObject selectStage;
    [SerializeField] private GameObject loadStage;

    [SerializeField] private GameObject selectStageState;
    [SerializeField] private GameObject loadStageState;

    private void Start()
    {
        Initialize();

        SelectStages();
        selectStageButtons.onClick.AddListener(SelectStages);
        loadStageButton.onClick.AddListener(LoadStages);
    }

    private void OnEnable()
    {   
        selectStageState.SetActive(true);
        selectStageState.SetActive(false);
    }

    private void Initialize()
    {
        if (buttonContainer != null)
        {
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void SelectStages()
    {
        Initialize();

        selectStageState.gameObject.SetActive(false);
        loadStageState.gameObject.SetActive(true);

        selectStage.gameObject.SetActive(true);
    }

    private void LoadStages()
    {
        selectStage.gameObject.SetActive(false);

        selectStageState.gameObject.SetActive(true);
        loadStageState.gameObject.SetActive(false);

        Initialize();

        if (SaveLoadManager.Instance != null)
        {
            var saveFiles = SaveLoadManager.Instance.GetSavedStageFiles();
            foreach (var saveFile in saveFiles)
            {
                SaveLoadManager.Instance.isDataLoadScene = true;
                UI_DataLoadButton button = Instantiate(buttonPrefab, buttonContainer);
                button.Initialize(saveFile.StageName, saveFile.SaveTime, OnLoadButtonClicked, OnDeleteButtonClicked);
            }
        }
        else
        {
            Debug.Log("SaveLoadManager 없음");
        }
    }

    private void OnLoadButtonClicked(string stageName)
    {
        SceneManager.LoadScene(stageName);
    }

    private void OnDeleteButtonClicked(string stageName)
    {
        SaveLoadManager.Instance.DeleteSaveFile(stageName);
        Initialize();
        LoadStages();
    }
}