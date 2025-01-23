using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Renderer leftBumper;
    public Renderer rightBumper;

    public Renderer leftTrigger;
    public Renderer rightTrigger;

    public Renderer leftButtonA;
    public Renderer rightButtonA;

    public Renderer leftButtonB;
    public Renderer rightButtonB;

    public Renderer leftThumbStick;
    public Renderer rightThumbStick;

    private List<Renderer> renderers = new List<Renderer>();

    public Material alphaWhite;
    public Material alphaOrange;

    public TextMeshProUGUI UIText;
    public TextMeshProUGUI ghostText;

    public Transform[] tutorialAreas;
    public Transform[] ghostWayPoints;
    public bool[] tutorialCleared = new bool[10];

    [Header("힌트 UI"), Range(0.1f, 2f)] public float fadeSpeed = 0.5f;
    [Header("힌트 UI 지속시간")] public float duration;
    public CanvasGroup canvas;

    private NavMeshAgent agent;
    //1. 플레이어 시작 위치에 UI 출력, 이동 관련해서 L스틱으로 이동 및 R스틱으로 시점전환, 텔레포트 기능을 알려줌
    //2. 플레이어가 이동해서 첫 번째 TriggerZone에서 UI를 정해진 위치로 이동 및 컨트롤러 색상 교체

    private void Awake()
    {
        agent = FindObjectOfType<NavMeshAgent>();
    }

    public void Start()
    {
        renderers.Add(leftBumper);
        renderers.Add(rightBumper);
        renderers.Add(leftTrigger);
        renderers.Add(rightTrigger);
        renderers.Add(leftButtonA);
        renderers.Add(rightButtonA);
        renderers.Add(rightButtonB);
        renderers.Add(leftThumbStick);
        renderers.Add(rightThumbStick);
        
        StartCoroutine(FadeInCanvasCoroutine());
    }

    //아래 이벤트는 Trigger Zone 스크립트 넣고 EnterEvent로 실행시키기
    //플레이어한테 Rigidbody가 없으니 콜라이더에 Rigidbody넣고 UseGravity = false로 해야함 

    #region 처음에 바로 출력하도록 기획 수정됨
    //public void OnMoveArea(int i)
    //{
    //    Debug.Log("무브");
    //    if (tutorialCleared[i] == true)
    //        return;

    //    ResetRenderers();

    //    leftThumbStick.material = alphaOrange;


    //    gameObject.transform.position = tutorialAreas[0].transform.position;
    //    tutorialCleared[i] = true;

    //    canvas.gameObject.SetActive(true);
    //    StartCoroutine(FadeInCanvasCoroutine());
    //    text.text = "이동은 L스틱";
    //}

    //public void OnLookArea(int i)
    //{
    //    ResetRenderers();

    //    rightThumbStick.material = alphaOrange;

    //    text.text = "둘러보려면 R스틱";

    //    gameObject.transform.position = tutorialAreas[0].transform.position;
    //}
    #endregion

    public void OnGrapArea(int i) //0
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;

        SetUI(i);

        UIText.text = "책상 위에 있는 물건을 잡으려면 그립 버튼";
        ghostText.text = "ㅇ";
    }
    public void OnAlyxGrabArea(int i) //1
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftTrigger.material = alphaOrange;
        rightTrigger.material = alphaOrange;
        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;

        SetUI(i);

        UIText.text = "멀리 있는 물건을 잡으려면 트리거버튼 + 그립버튼 + 당기기";
        ghostText.text = "ㅇ";
    }

    public void OnExplainInventory(int i) //2
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        rightButtonB.material = alphaOrange;

        SetUI(i);

        UIText.text = "대충 인벤토리에 넣고 빼는 기능 설명하는 텍스트";
        ghostText.text = "ㅇ";
        //인벤토리에 이벤트 없으면 하나로 대신하고싶어요

    }

    public void OnKeyPuzzle(int i) //3
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;
        leftTrigger.material = alphaOrange;
        rightTrigger.material = alphaOrange;

        SetUI(i);

        UIText.text = "책과 가방은 그립 + 트리거로 열 수 있다는 텍스트";
        ghostText.text = "ㅇ";
    }

    public void OnDialPuzzle(int i) //4
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;
        leftTrigger.material = alphaOrange;
        rightTrigger.material = alphaOrange;

        SetUI(i);

        UIText.text = "다이얼, 키패드 옆에서 그립 + 트리거를 누르면 UI를 출력한다는 텍스트";
        ghostText.text = "ㅇ";
    }

    public void OnMixPuzzle(int i) //5
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;

        SetUI(i);

        UIText.text = "대충 랜턴이랑 양초랑 쓰까보라는 텍스트";
        //트리거존 지나면서 태그 변경한 뒤 그 이후에 상호작용 되게 하기
        ghostText.text = "ㅇ";
    }

    public void OnDialog(int i) //6
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        SetUI(i);

        UIText.text = "살여주세요";
        //트리거존 지나면서 태그 변경한 뒤 그 이후에 상호작용 되게 하기
        ghostText.text = "살고싶어요";
    }

    //public void OnRayInteractorArea(int i)
    //{
    //    ResetRenderers();

    //    leftTrigger.material = alphaOrange;
    //    rightTrigger.material = alphaOrange;

    //    UIText.text = "UI와 상호작용하려면 트리거 버튼";

    //    gameObject.transform.position = tutorialAreas[0].transform.position;
    //}



    private void ResetRenderers()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material = alphaWhite;
        }
    }

    private void SetUI(int i)
    {
        gameObject.transform.position = tutorialAreas[i].transform.position;
        tutorialCleared[i] = true;
        agent.SetDestination(ghostWayPoints[i].transform.position);

        canvas.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeInCanvasCoroutine());
    }

    public IEnumerator FadeOutCanvasCoroutine()
    {
        canvas.alpha = 1f;
        Debug.Log("캔버스 안보이게");
        //캔버스 안보이게 하는거
        if (canvas.alpha <= 0)
        {
            canvas.gameObject.SetActive(false);
            yield break;
        }

        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        //업데이트문
        //따로 시작할 수 있는 업데이트 문2 <- 그런데 업데이트랑은 따로 돌아감
        //
        canvas.alpha = 0;
        canvas.gameObject.SetActive(false);
    }

    public IEnumerator FadeInCanvasCoroutine()
    {
        canvas.alpha = 0;

        Debug.Log("캔버스 보이게");
        //캔바스 보이게 하는거
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

        yield return new WaitForSeconds(duration);

        //지속시간동안 표시하다가 날려버리기
        StartCoroutine(FadeOutCanvasCoroutine());
    }


}