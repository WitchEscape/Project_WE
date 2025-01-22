using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    public TextMeshProUGUI text;

    public Transform[] tutorialAreas;
    public bool[] tutorialCleared = new bool[10];

    [Header("힌트 UI"), Range(0.1f, 2f)] public float fadeSpeed = 0.5f;
    [Header("힌트 UI 지속시간")] public float duration;
    public CanvasGroup canvas;

    //1. 플레이어 시작 위치에 UI 출력, 이동 관련해서 L스틱으로 이동 및 R스틱으로 시점전환, 텔레포트 기능을 알려줌
    //2. 플레이어가 이동해서 첫 번째 TriggerZone에서 UI를 정해진 위치로 이동 및 컨트롤러 색상 교체
    
    private void Awake()
    {

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
    public void OnMoveArea(int i)
    {
        Debug.Log("무브");
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftThumbStick.material = alphaOrange;


        gameObject.transform.position = tutorialAreas[0].transform.position;
        tutorialCleared[i] = true;

        canvas.gameObject.SetActive(true);
        StartCoroutine(FadeInCanvasCoroutine());
        text.text = "이동은 L스틱";
    }

    public void OnLookArea(int i)
    {
        ResetRenderers();

        rightThumbStick.material = alphaOrange;

        text.text = "둘러보려면 R스틱";

        gameObject.transform.position = tutorialAreas[0].transform.position;
    }
    #endregion

    public void OnGrapArea(int i) //0
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;

        gameObject.transform.position = tutorialAreas[i].transform.position;
        tutorialCleared[i] = true;

        canvas.gameObject.SetActive(true);
        StartCoroutine(FadeInCanvasCoroutine());
        text.text = "책상 위에 있는 물건을 잡으려면 그립 버튼";
    }

    public void OnRayInteractorArea(int i)
    {
        ResetRenderers();

        leftTrigger.material = alphaOrange;
        rightTrigger.material = alphaOrange;

        text.text = "UI와 상호작용하려면 트리거 버튼";

        gameObject.transform.position = tutorialAreas[0].transform.position;
    }

    public void OnAlyxGrabArea(int i)
    {
        //AlyxGrabTutorialObject 이벤트에서 SetActive(true)하고 다음 튜토리얼존에서 false 시키기
        ResetRenderers();

        leftTrigger.material = alphaOrange;
        rightTrigger.material = alphaOrange;
        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;

        text.text = "멀리 있는 물건을 잡으려면 트리거 버튼과 그립버튼을 누른 채로 손을 흔들기";

        gameObject.transform.position = tutorialAreas[0].transform.position;
    }

    private void ResetRenderers()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material = alphaWhite;
        }
    }

    public IEnumerator FadeOutCanvasCoroutine()
    {
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