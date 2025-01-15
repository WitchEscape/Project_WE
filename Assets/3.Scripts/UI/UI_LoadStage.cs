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
        Debug.Log($"Found {saveFiles.Count} save files");
        
        foreach (var saveFile in saveFiles)
        {
            Debug.Log($"Creating button for stage: {saveFile.StageName}");
            UI_DataLoadButton button = Instantiate(buttonPrefab, buttonContainer);
            button.Initialize(saveFile.StageName, saveFile.SaveTime, OnLoadButtonClicked, OnDeleteButtonClicked);
        }
    }

    private void OnLoadButtonClicked(string stageName)
    {
        Debug.Log($"OnLoadButtonClicked called with stage: {stageName}");
        StartCoroutine(LoadStageCoroutine(stageName));
    }

    private IEnumerator LoadStageCoroutine(string stageName)
    {
        Debug.Log($"Starting to load stage: {stageName}");
        UIManager.Instance.CloseAllCurrentUI();
        // 씬 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(stageName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 씬이 완전히 로드될 때까지 잠시 대기
        yield return new WaitForSeconds(0.5f);

        // SaveLoadManager가 새 씬에서 초기화되었는지 확인
        if (SaveLoadManager.Instance == null)
        {
            Debug.LogError("SaveLoadManager.Instance is null after scene load!");
            yield break;
        }

        Debug.Log($"Scene loaded, now loading save data for: {stageName}");
        
        try
        {
            SaveLoadManager.Instance.LoadGame(stageName);
            Debug.Log($"Save data loaded successfully for: {stageName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save data: {e.Message}\nStackTrace: {e.StackTrace}");
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
