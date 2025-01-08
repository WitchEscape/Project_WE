using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 筌뤴뫀諭?ui????뺤쓰????롪돌??몄춸 鈺곕똻???뺣뼄
// 

public class UI_LoadStage : MonoBehaviour
{
    [SerializeField] private Button Class535SceneButton;
    [SerializeField] private Button DomitorySceneButton;
    [SerializeField] private Button PotionClassSceneButton;
    [SerializeField] private Button LibrarySceneButton;
    [SerializeField] private Button CommonRoomSceneButton;
    public float StartPanelDistance = 2f;
    private void Start()
    {
        Class535SceneButton.onClick.AddListener(OnClass535SceneButtonClick);
        DomitorySceneButton.onClick.AddListener(OnDomitorySceneButtonClick);
        PotionClassSceneButton.onClick.AddListener(OnPotionClassSceneButtonClick);
        LibrarySceneButton.onClick.AddListener(OnLibrarySceneButtonClick);
        CommonRoomSceneButton.onClick.AddListener(OnCommonRoomSceneButtonClick);
    }

    private void Initialize()
    {
        //Todo : ??쎈??? ???????????怨뺚뀲 筌ｌ꼶???袁⑹뒄
    }
    private void OnEnable()
    {
        Initialize();
        SetPositionAndRotation();
    }


    private void OnClass535SceneButtonClick()
    {
        SceneManager.LoadScene("Class535");
    }

    private void OnDomitorySceneButtonClick()
    {
        SceneManager.LoadScene("Domitory");
    }

    private void OnPotionClassSceneButtonClick()
    {
        SceneManager.LoadScene("PotionClass");
    }

    private void OnLibrarySceneButtonClick()
    {
        SceneManager.LoadScene("Library");
    }

    private void OnCommonRoomSceneButtonClick()
    {
        SceneManager.LoadScene("CommonRoom");
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
