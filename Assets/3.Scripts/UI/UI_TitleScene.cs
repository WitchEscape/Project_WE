using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_TitleScene : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button ExitButton;

    private void Start()
    {
        startButton.onClick.AddListener(GameStart);
        optionButton.onClick.AddListener(OpenOption);
        ExitButton.onClick.AddListener(GameExit);
    }

    private void GameStart()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    private void OpenOption()
    {
    }

    private void GameExit()
    {
        Application.Quit();
    }
}
