using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Menu : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button saveLoadButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveExitButton;

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject saveLoadPanel;

    public float StartPanelDistance = 2f;

    private void Start()
    {
        settingButton.onClick.AddListener(OnSettingButtonClick);
        saveLoadButton.onClick.AddListener(OnSaveLoadButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
        saveExitButton.onClick.AddListener(OnSaveExitButtonClick);
    }

    private void OnEnable()
    {
        SetPositionAndRotation();
    }

    private void OnSettingButtonClick()
    {
        UI_Manager.Instance.OpenUI(settingPanel);
    }

    private void OnSaveLoadButtonClick()
    {
        UI_Manager.Instance.OpenUI(saveLoadPanel);
    }

    private void OnBackButtonClick()
    {
        UI_Manager.Instance.CloseCurrentUI();
    }

    private void OnSaveExitButtonClick()
    {
        SceneManager.LoadScene("LobbyScene");
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
