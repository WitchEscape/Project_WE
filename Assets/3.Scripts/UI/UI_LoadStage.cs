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

    [SerializeField] private Button stageSelectButton;
    [SerializeField] private Button stageLoadButton;

    private void Start()
    {
        Initialize();

        LoadSelectStages();
        stageSelectButton.onClick.AddListener(LoadSelectStages);
        stageLoadButton.onClick.AddListener(LoadSaveStages);
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

    private void LoadSelectStages()
    {
        Initialize();
        UI_DataLoadButton button = Instantiate(buttonPrefab, buttonContainer);
        button.Initialize("535교실", DateTime.Now, OnLoadButtonClicked, OnDeleteButtonClicked);
    }


    private void LoadSaveStages()
    {
        Initialize();
        if(SaveLoadManager.Instance != null)
        {
            var saveFiles = SaveLoadManager.Instance.GetSavedStageFiles();
            foreach (var saveFile in saveFiles)
            {
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
        SaveLoadManager.Instance.isDataLoadScene = true;
        SceneManager.LoadScene(stageName);
        //StartCoroutine(LoadStageCoroutine(stageName));
    }

    private void OnDeleteButtonClicked(string stageName)
    {
        SaveLoadManager.Instance.DeleteSaveFile(stageName);
        Initialize();
        LoadSaveStages();
    }

    private IEnumerator LoadStageCoroutine(string stageName)
    {
        UIManager.Instance.CloseAllCurrentUI();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(stageName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (SaveLoadManager.Instance == null)
        {
            yield break;
        }

        try
        {
            SaveLoadManager.Instance.LoadGame(stageName);
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }
}
