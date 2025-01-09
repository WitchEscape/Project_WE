using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private Button soundButton;
    [SerializeField] private Button displayButton;
    [SerializeField] private Button vrButton;

    [SerializeField] private GameObject soundOption;
    [SerializeField] private GameObject displayOption;
    [SerializeField] private GameObject vrOption;

    public float StartPanelDistance = 2f;

    private void Start()
    {
        soundButton.onClick.AddListener(OnSoundButtonClick);
        displayButton.onClick.AddListener(OnDisplayButtonClick);
        vrButton.onClick.AddListener(OnVrButtonClick);
    }
    private void OnEnable()
    {
        OnSoundButtonClick();
        SetPositionAndRotation();
    }

    private void OnSoundButtonClick()
    {
        soundOption.gameObject.SetActive(true);
        displayOption.gameObject.SetActive(false);
        vrOption.gameObject.SetActive(false);
    }

    private void OnDisplayButtonClick()
    {
        soundOption.gameObject.SetActive(false);
        displayOption.gameObject.SetActive(true);
        vrOption.gameObject.SetActive(false);
    }

    private void OnVrButtonClick()
    {
        soundOption.gameObject.SetActive(false);
        displayOption.gameObject.SetActive(false);
        vrOption.gameObject.SetActive(true);
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
