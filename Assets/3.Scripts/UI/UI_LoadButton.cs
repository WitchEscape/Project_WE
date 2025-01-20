using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LoadButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject focus;
    private Button button;
    [SerializeField] private string sceneName;
    [SerializeField] private int sceneNum;
    [SerializeField] private GameObject sceneState;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SceneLoad);
    }

    private void SceneLoad()
    {
        if (SaveLoadManager.Instance.CurrentClearStage >= sceneNum)
        {
            SceneManager.LoadScene(sceneName);
        }
    }


    private void OnEnable()
    {
        if (focus != null)
        {
            focus.SetActive(false);
        }

        
        if(SaveLoadManager.Instance.CurrentClearStage >= sceneNum)
        {
            sceneState.SetActive(false);
        }
        else
        {
            sceneState.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (focus != null)
        {
            focus.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (focus != null)
        {
            focus.SetActive(false);
        }
    }
}
