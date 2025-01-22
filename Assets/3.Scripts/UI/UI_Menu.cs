using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Menu : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button saveLoadButton;
    [SerializeField] private Button saveExitButton;

    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private GameObject settingPanel;

    public float StartPanelDistance = 2f;

    private void Start()
    {
        settingButton.onClick.AddListener(OnSettingButtonClick);
        saveExitButton.onClick.AddListener(OnSaveExitButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
    }

    private void OnLoadButtonClick()
    {
        SaveLoadManager.Instance.isDataLoadScene = true;
        SaveLoadManager.Instance.LoadGame(SceneManager.GetActiveScene().name);
    }

    private void OnSaveButtonClick()
    {
        SaveLoadManager.Instance.SaveGame(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        SetPositionAndRotation();
    }

    private void OnSettingButtonClick()
    {
        UIManager.Instance.OpenUI(settingPanel);
    }

    private void OnSaveExitButtonClick()
    {
        //SaveLoadManager.Instance.SaveGame(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("WE_Level_Tutorial");
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
