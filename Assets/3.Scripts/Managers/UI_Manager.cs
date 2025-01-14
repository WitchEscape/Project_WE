using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager instance;
    public static UI_Manager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_Manager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("@UIManager");
                    instance = go.AddComponent<UI_Manager>();
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
    private Stack<GameObject> uiStack = new Stack<GameObject>();

    public void OpenUI(GameObject uiPanel)
    {
        if(uiPanel != null)
        {
            if (uiStack.Count > 0)
            {
                GameObject currentUI = uiStack.Peek();
                if (currentUI != null)
                {
                    currentUI.SetActive(false);
                }
            }

            uiPanel.SetActive(true);
            uiStack.Push(uiPanel);
        }
    }

    public void CloseCurrentUI()
    {
        if (uiStack.Count > 0)
        {
            GameObject currentUI = uiStack.Pop();
            if (currentUI != null)
            {
                currentUI.SetActive(false);
            }

            if (uiStack.Count > 0)
            {
                GameObject previousUI = uiStack.Peek();
                if (previousUI != null)
                {
                    previousUI.SetActive(true);
                }
            }
        }
    }

    public void CloseAllCurrentUI()
    {
        while (uiStack.Count > 0)
        {
            GameObject currentUI = uiStack.Pop();
            if (currentUI != null)
            {
                currentUI.SetActive(false);
            }
        }
        uiStack.Clear();
    }

    public bool HasActiveUI()
    {
        return uiStack.Count > 0;
    }

    public int GetUIStackCount()
    {
        return uiStack.Count;
    }
}