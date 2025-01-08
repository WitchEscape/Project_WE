using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ESceneType
{
    Class535Scene,
    CommonRoomScene,
    DomitoryScene,
    LibraryScene,
    LobbyScene,
    PotionClassScene,
    TitleScene,
    TutorialScene,
    None,
}

public class CustomSceneManager : MonoBehaviour
{
    private static CustomSceneManager instance = null;
    public static CustomSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CustomSceneManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("@CustomSceneManager");
                    instance = go.AddComponent<CustomSceneManager>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
