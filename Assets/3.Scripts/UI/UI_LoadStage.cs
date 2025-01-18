using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LoadStage : MonoBehaviour
{
    public float StartPanelDistance = 2f;

    [SerializeField] private UI_DataLoadButton buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    private void OnEnable()
    {
        Initialize();
        SetPositionAndRotation();
        LoadSaveFiles();
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

    private void LoadSaveFiles()
    {
        var saveFiles = SaveLoadManager.Instance.GetSavedStageFiles();
        foreach (var saveFile in saveFiles)
        {
            UI_DataLoadButton button = Instantiate(buttonPrefab, buttonContainer);
            button.Initialize(saveFile.StageName, saveFile.SaveTime, OnLoadButtonClicked, OnDeleteButtonClicked);
        }
    }

    private void OnLoadButtonClicked(string stageName)
    {
        StartCoroutine(LoadStageCoroutine(stageName));
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

    private void OnDeleteButtonClicked(string stageName)
    {
        SaveLoadManager.Instance.DeleteSaveFile(stageName);
        Initialize();
        LoadSaveFiles();
    }

    private void SetPositionAndRotation()
    {
        Transform cameraTransform = Camera.main.transform;

        Vector3 inventoryPosition = cameraTransform.position + (cameraTransform.forward * StartPanelDistance);
        transform.position = inventoryPosition;

        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180f, 0);
    }
}
