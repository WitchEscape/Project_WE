using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;


public class GhostCanvas : MonoBehaviour
{
    private CanvasGroup canvas;
    public Chapters chapters;
    [SerializeField, Header("Yes버튼")] private Button yesButton;
    [SerializeField, Header("No버튼")] private Button noButton;
    [SerializeField, Header("호출 횟수에 따른 난이도")] private Button[] hintButtons;
    [Header("클리어 체크용 bool 변수")] public bool[] isCleared;

    //유령 호출 횟수
    private int callIndex;

    private int clearCount;
    private string[] hintTexts = new string[3];

    public TextMeshProUGUI hintText;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0;
        canvas.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        yesButton.onClick.AddListener(OnClickYes);
        noButton.onClick.AddListener(OnClickNo);
        Debug.Log($"힌트배열 : {hintButtons.Length}");
        Debug.Log($"힌트텍스트 : {hintTexts.Length}");

        for (int i = 0; i < hintTexts.Length; i++)
        {
            hintTexts[i] = "집에가게해줘";
        }

        for (int i = 0; i < hintButtons.Length; i++)
        {
            int index = i;
            hintButtons[i].onClick.RemoveAllListeners();
            //hintButtons[i].onClick.AddListener(() => { OnClickHintButton(i); });
            hintButtons[i].onClick.AddListener(()=>OnClickHintButton(index));
            print(hintButtons[i].onClick);
            Debug.Log($"힌트 텍스트 배열 {hintTexts[i]}");
            Debug.Log($"Awake {i}");
            
        }
    }

    private void OnClickYes()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        //호출할때마다 clearCount 초기화
        clearCount = 0;

        //isCleared배열에서 클리어한 퍼즐 갯수 검사, 선형적 구조이니까 가능한 방법
        for (int i = 0; i < isCleared.Length; i++)
        {
            //클리어 한 상태라면
            if (isCleared[i] == true)
            {
                //클리어 카운트 증가시킴
                clearCount++;
            }
        }
        InitializationChapters();
    }

    private void SetHintButtons()
    {
        //호출한 횟수에 따라 버튼 초기화
        for (int i = 0; i <= callIndex; i++)
        {
            hintButtons[i].gameObject.SetActive(true);
        }
    }

    public void OnClickHintButton(int i)
    {
        
        Debug.Log($"i : {i}");
        if (hintTexts.Length >= i)
        {
            Debug.Log($"hint : {hintTexts[i]}");

            hintText.text = $"{hintTexts[i]}";
        }
        callIndex++;
    }

    private void OnClickNo()
    {
        StartCoroutine(DisableCanvasCoroutine());
    }

    public IEnumerator DisableCanvasCoroutine()
    {
        while (true)
        {
            canvas.alpha = canvas.alpha - (Time.deltaTime * 0.5f);

            if (canvas.alpha <= 0)
            {
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }

    }

    private void InitializationChapters()
    {
        switch (chapters)
        {
            case Chapters.Lobby:
                InitializationLobby();
                break;
            case Chapters.Class535:
                InitializationClass535();
                break;
            case Chapters.Dormitory:
                InitializationDormitory();
                break;
            case Chapters.PostionClass:
                InitializationPostionClass();
                break;
            case Chapters.Library:
                InitializationLibrary();
                break;
            case Chapters.TeachersRoom:
                InitializationTeachersRoom();
                break;
        }

        SetHintButtons();
    }

    private void InitializationLobby()
    {
        switch (clearCount)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    private void InitializationClass535()
    {
        switch (clearCount)
        {
            case 0:
                hintTexts[0] = "하하 멍청일";
                hintTexts[1] = "하하 멍청이";
                hintTexts[2] = "하하 멍청삼";
                break;
            case 1:
                hintTexts[0] = "하하 멍청삼";
                hintTexts[1] = "하하 멍청사";
                hintTexts[2] = "하하 멍청오";
                break;
            case 2:
                hintTexts[0] = "하하 멍청육";
                hintTexts[1] = "하하 멍청칠";
                hintTexts[2] = "하하 멍청팔";
                break;
            case 3:
                break;
        }

    }

    private void InitializationDormitory()
    {
        switch (clearCount)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    private void InitializationPostionClass()
    {
        switch (clearCount)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    private void InitializationLibrary()
    {
        switch (clearCount)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    private void InitializationTeachersRoom()
    {
        switch (clearCount)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    private void OnDisable()
    {
        canvas.alpha = 0f;
        if(yesButton.onClick != null)
        yesButton.onClick.RemoveListener(OnClickYes);
        if (noButton.onClick != null)
            noButton.onClick.RemoveListener(OnClickNo);

        for (int i = 0; i < hintButtons.Length; i++)
        {
            if(hintButtons[i].onClick != null)
            hintButtons[i].onClick.RemoveListener(() => OnClickHintButton(i));
            hintButtons[i].gameObject.SetActive(false);
        }

        hintText.text = "";
    }
}
