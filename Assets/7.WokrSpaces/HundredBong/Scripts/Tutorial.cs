using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.AI;

using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

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


    [SerializeField] private DynamicMoveProvider dynamic; 
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


    // tutorialCleared 에 포함되지 않음
    private bool isActivated = false;
    public void OnStartDialog()
    {
        if(isActivated == false)
        {
            DialogPlayer.Instance.PlayDialogSequence("LOBBY_01");
            isActivated = true;
        }
    }

    public void OnGrapArea(int i) //0
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;

        SetUI(i);

        UIText.text = "그립 버튼을 이용해 책을 집어보세요.";
        ghostText.text = "책상 위 책을 잡아봐~";
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

        UIText.text = "좌측 책상 위 트리거를 눌러 조준한 뒤, 그립 버튼을 눌러 잡은 물건을 끌어 당기세요.";
        ghostText.text = "멀리서 물건을 잡아오면 굳이 걸어서 가지 않아도 돼";
    }

    public void OnExplainInventory(int i) //2
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        rightButtonB.material = alphaOrange;

        SetUI(i);

        UIText.text = "A 버튼을 눌러 인벤토리 창을 켜세요.";
        ghostText.text = "인벤토리엔 여러가지 물건을 손으로 잡아 넣거나 뺄 수 있어!";
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

        UIText.text = "책을 잡은 상태에서 트리거 버튼을 눌러 펼치세요.";
        ghostText.text = "트리거 버튼을 눌러 책을 펼쳐봐~"; 
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

        UIText.text = "비밀번호를 입력해 서랍장을 여세요.";
        ghostText.text = "아까 책에있던 비밀번호를 입력해서 열어봐!"; 
    }

    public void OnMixPuzzle(int i) //5
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;

        SetUI(i);

        UIText.text = "등불에 초를 넣어 조합해 보세요.";
        //트리거존 지나면서 태그 변경한 뒤 그 이후에 상호작용 되게 하기
        ghostText.text = "등불이 여기 있네! 양초를 등불에 넣어보자~"; 
    }


    private bool isActivated2 = false;
    public void OnDialog(int i) //6
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        SetUI(i);

        UIText.text = "등불을 종이 근처에 두면 무슨 일이 생길까?";
        //트리거존 지나면서 태그 변경한 뒤 그 이후에 상호작용 되게 하기
        if (isActivated2 == false)
        {
            DialogPlayer.Instance.PlayDialogSequence("LOBBY_02");
            isActivated2 = true;
        }
        
        ghostText.text = "종이에 불을 비춰보세요."; 
    }

    public void LowSpeed()
    {
        dynamic.moveSpeed  =1f;
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