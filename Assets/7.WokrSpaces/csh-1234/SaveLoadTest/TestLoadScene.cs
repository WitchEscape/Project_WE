    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestLoadScene : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;

    private void Start()
    {
        button1.onClick.AddListener(saveData);
        button2.onClick.AddListener(loadData);
        if(button3 != null)
        {
            button3.onClick.AddListener(loadjust);
        }
    }

    private void saveData()
    {
        SaveLoadManager.Instance.SaveGame(SceneManager.GetActiveScene().name);
    }

    private void loadData()
    {
        SaveLoadManager.Instance.LoadGame(SceneManager.GetActiveScene().name);
    }

    private void loadjust()
    {
        SceneManager.LoadScene("SaveLoadTest2");
    }

}
