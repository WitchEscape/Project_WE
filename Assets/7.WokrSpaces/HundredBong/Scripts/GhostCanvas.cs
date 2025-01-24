using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GhostCanvas : MonoBehaviour
{
    private FinitStateMachine fsm;
    public CanvasGroup canvas;
    [Header("힌트 UI 페이드 속도"), Range(0.1f, 2f)] public float fadeSpeed = 0.5f;
    public Chapters chapters;
    [SerializeField, Header("Yes버튼")] private Button yesButton;
    [SerializeField, Header("No버튼")] private Button noButton;
    [SerializeField, Header("취소 버튼")] private Button cancelButton;
    [SerializeField, Header("힌트 버튼 출력할 영역")] private RectTransform hintButtonArea;
    [SerializeField, Header("호출 횟수에 따른 난이도")] private Button[] hintButtons;
    [Header("클리어 체크용 bool 변수")] public bool[] isCleared;
    [Header("힌트 지속시간")] public float hintDuration;
    
    //유령 호출 횟수
    private int currentIndex = 0;
    private int callIndex { get { return currentIndex; } set { currentIndex = Mathf.Clamp(value, 0, 2); } }

    public int clearCount = 0;
    private string[] hintTexts;

    public TextMeshProUGUI hintText;

    private Coroutine hintEndCoroutine;

    private bool isYesButtonClicked = false;

    private void Awake()
    {
        fsm = GetComponentInParent<FinitStateMachine>();
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0;
    }

    private void OnEnable()
    {
        InitializationButtonsOnEnable();
    }

    private void Start()
    {
        if (chapters == Chapters.Library)
        {
            hintTexts = new string[4];
        }
        else
        {
            hintTexts = new string[3];
        }

        canvas.gameObject.SetActive(false);
    }

    private void InitializationButtonsOnEnable()
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(false);

        yesButton.onClick.AddListener(OnClickYes);
        noButton.onClick.AddListener(OnClickNo);
        cancelButton.onClick.AddListener(OnClickCancel);

        for (int i = 0; i < hintButtons.Length; i++)
        {
            //클로저 문제 방지용 인덱스 추가
            int index = i;
            hintButtons[i].onClick.AddListener(() => OnClickHintButton(index));
        }
    }

    private void InitializationButtonsOnDisable()
    {
        yesButton.onClick.RemoveListener(OnClickYes);
        noButton.onClick.RemoveListener(OnClickNo);
        cancelButton.onClick.RemoveListener(OnClickCancel);


        for (int i = 0; i < hintButtons.Length; i++)
        {
            int index = i;
            hintButtons[i].onClick.RemoveListener(() => OnClickHintButton(index));
            hintButtons[i].gameObject.SetActive(false);
        }

        hintButtonArea?.gameObject.SetActive(false);

        hintText.text = "도와줄까?";
    }

    private void OnClickYes()
    {
        isYesButtonClicked = true;

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);

        if (chapters != Chapters.Library)
        {
            CheckClearedPuzzles();
        }


        InitializationChapters();

        SetHintButtons();

        //기존 코루틴 중단 및 초기화
        if (hintEndCoroutine != null)
        {
            StopCoroutine(hintEndCoroutine);
            hintEndCoroutine = null;
        }

        //새로운 코루틴 시작
        hintEndCoroutine = StartCoroutine(EndTalkAfterDelayCoroutine());

        callIndex++;
    }

    private void OnClickNo()
    {
        fsm.EndTalkByButtonOrDistance();
        StartCoroutine(FadeOutCanvasCoroutine());
    }

    private void OnClickCancel()
    {
        StartCoroutine(FadeOutCanvasCoroutine());
        fsm.EndTalkByEvent();
    }

    private void CheckClearedPuzzles()
    {

        //int previousClearCount = clearCount; //이전 클리서탕태 저장용

        //호출할때마다 clearCount 초기화
        clearCount = 0;

        //isCleared배열에서 클리어한 퍼즐 갯수 검사, 선형적 구조이니까 가능한 방법
        for (int i = 0; i < isCleared.Length; i++)
        {
            Debug.Log(isCleared[i]);
            //클리어 한 상태라면
            //if (isCleared[i] == true)
            //{
            //    //클리어 카운트 증가시킴
            //    clearCount++;
            //}

            if (isCleared[i] == false)
            {
                //클리어 카운트 증가시킴
                clearCount = i;
                break;
            }

        }

        //if (previousClearCount != clearCount) // 클리어 상태가 변했을 경우만 초기화
        //{
        //    InitializationChapters();
        //    Debug.Log($"ClearCount 변경 감지: {previousClearCount} -> {clearCount}, 초기화 실행");
        //}
    }

    private void SetHintButtons()
    {
        Debug.Log($"Call Index1 : {callIndex}");
        
        hintButtonArea?.gameObject.SetActive(true);

        if (chapters == Chapters.Library)
        {
            foreach (Button button in hintButtons)
            {
                button.gameObject.SetActive(true);
            }
        }
        else
        {
            //호출한 횟수에 따라 버튼 초기화
            for (int i = 0; i <= callIndex; i++)
            {
                hintButtons[i].gameObject.SetActive(true);
            }
        }

    }

    public void OnClickHintButton(int i)
    {

        Debug.Log($"i : {i}");
        if (i < hintTexts.Length)
        {
            Debug.Log($"Call Index : {callIndex}");

            hintText.text = $"{hintTexts[i]}";
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
                hintTexts[0] = "노란색 글자만 읽어봐";
                hintTexts[1] = "노란색 글자를 숫자라고 생각해봐";
                hintTexts[2] = "3번 자리에 5242를 입력해";
                break;
            case 1:
                hintTexts[0] = "쪽지에 글자가 타로 카드랑 연관이 있어";
                hintTexts[1] = "1번 책상에 타로 카드를 쪽지에 적힌 순서대로 놓아봐";
                hintTexts[2] = "1번 책상에 바보, 마법, 힘, 은둔자 카드를 순서대로 배치해봐";
                break;
            case 2:
                hintTexts[0] = "오른쪽에 적혀 있는 숫자를 휴대폰 패턴을 풀듯 생각해 보는 게 어때?";
                hintTexts[1] = "오른쪽에 적힌 숫자가 한 줄에 하나의 글자를 나타내고 있어";
                hintTexts[2] = "H E R A 를 교탁 밑 상자에 입력해봐!";
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
                hintTexts[0] = "포스터에 어떤 문자가 적혀 있어! 서랍장 비밀번호와 관련 있어 보여";
                hintTexts[1] = "포스터에 적힌 문자가 로마 숫자인 것 같아";
                hintTexts[2] = "1042를 서랍장에 입력해봐!";
                break;
            case 1:
                hintTexts[0] = "물뿌리개로 시든 꽃에 물을 줘보자!";
                hintTexts[1] = "꽃의 색이 퍼즐과 연관이 있는 것 같지 않아?";
                hintTexts[2] = "꽃에 나타난 색을 순서대로 퍼즐에 입력해";
                break;
            case 2:
                hintTexts[0] = "";
                hintTexts[1] = "";
                hintTexts[2] = "";
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
                hintTexts[0] = "선반 위에 포션이 올려져 있어! 색을 알맞게 배치하면 될 것 같아";
                hintTexts[1] = "포션의 색이 책상 위 시험관 색과 같아!";
                hintTexts[2] = "포션을 초록, 빨강, 파랑 순서로 배치해봐";
                break;
            case 1:
                hintTexts[0] = "곳곳에 쪽지가 떨어져 있어! 벽에 있는 글자의 힌트가 될 거야";
                hintTexts[1] = "벽에 있는 글자 중 네 글자를 쪽지에 힌트를 얻어 찾아내고, 룬 문자로 된 물약을 답에 맞게 솥에 넣자!";
                hintTexts[2] = "성냥으로 솥에 불을 붙이고, HOPE를 룬 문자에 맞게 물약을 넣어봐";
                break;
            case 2:
                hintTexts[0] = "돌이 밝게 빛나고 있어! 조명처럼 쓸 수 있을거 같은데..";
                hintTexts[1] = "밑에 꺼진 등불이 놓여져 있어! 쟤를 잘 이용하면 되지 않을까?";
                hintTexts[2] = "꺼진 등불에 빛나는 돌을 넣고, 빈 종이 위에 불을 비쳐봐";
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
                hintTexts[0] = "A x B + C 의 규칙을 떠올려봐";
                hintTexts[1] = "숫자를 시간이라고 생각해봐";
                hintTexts[2] = "글자를 뒤집어서 확인해봐";
                hintTexts[3] = "A=1, B=2 ... Z=26 의 규칙을 잘 생각해봐";
                break;
            case 1:
                hintTexts[0] = "";
                hintTexts[1] = "";
                hintTexts[2] = "";
                break;
            case 2:
                hintTexts[0] = "";
                hintTexts[1] = "";
                hintTexts[2] = "";
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
                hintTexts[0] = "칠판의 문제가 8이라는 숫자와 관련 있는 것 같아";
                hintTexts[1] = "8의 곱셈을 떠올려 볼까?";
                hintTexts[2] = "3287을 잠겨 있는 캐비넷에 입력해보자!";
                break;
            case 1:
                hintTexts[0] = "책상에 수상한 종이가 하나 놓여져 있어. 펜으로 무언가 적어볼까?";
                hintTexts[1] = "펜에 잉크를 묻혀 종이에 그려보면 문양이 나타나!";
                hintTexts[2] = "문양에 빨간 부분을 빼고 회색 부분만 보면 네 가지 숫자가 있네!";
                break;
            case 2:
                hintTexts[0] = "";
                hintTexts[1] = "";
                hintTexts[2] = "";
                break;
            case 3:
                break;
        }
    }


    public IEnumerator FadeOutCanvasCoroutine()
    {
        isYesButtonClicked = false;
        if (canvas.alpha <= 0)
        {
            canvas.gameObject.SetActive(false);
            yield break;
        }

        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime * 0.5f;
            yield return null;
        }

        canvas.alpha = 0;
        hintText.text = "도와줄까?";
        canvas.gameObject.SetActive(false);
    }

    public IEnumerator FadeInCanvasCoroutine()
    {
        Debug.Log("Fade In Coroutine 진입");

        if (canvas.gameObject.activeSelf == false)
        {
            canvas.gameObject.SetActive(true);
        }

        while (canvas.alpha < 1)
        {
            canvas.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvas.alpha = 1;
    }

    private IEnumerator EndTalkAfterDelayCoroutine()
    {
        yield return new WaitForSeconds(hintDuration);
        StartCoroutine(FadeOutCanvasCoroutine());
        fsm.EndTalkByEvent();
        hintEndCoroutine = null;
    }

    public void ClearPuzzle(int i)
    {
        //퍼즐 클리어시 호출

        //해당하는 인덱스의 클리어 유무를 true로 만듬
        isCleared[i] = true;

        if (hintEndCoroutine != null)
        {
            StopCoroutine(hintEndCoroutine);
            hintEndCoroutine = null;
        }

        //오브젝트가 활성화된 상태명
        if (gameObject.activeSelf == true)
        {
            //오브젝트 비활성화
            StartCoroutine(FadeOutCanvasCoroutine());

            //유령 상태변경 및 쿨타임 초기화
            fsm.EndTalkByEvent();
        }

        callIndex = 0;
    }


    private void OnDisable()
    {
        InitializationButtonsOnDisable();
    }

    public bool IsYesButtonClicked()
    {
        //소소한 캡슐화
        return isYesButtonClicked;
    }
}
