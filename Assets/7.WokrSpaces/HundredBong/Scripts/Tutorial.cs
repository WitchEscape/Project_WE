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
    public Button button;

    public Transform[] tutorialAreas;
    public bool[] tutorialCleared;

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

        if (button.gameObject.activeSelf)
        {
            button.gameObject.SetActive(false);
        }
    }

    //아래 이벤트는 Trigger Zone 스크립트 넣고 EnterEvent로 실행시키기
    //플레이어한테 Rigidbody가 없으니 콜라이더에 Rigidbody넣고 UseGravity = false로 해야함 

    public void OnMoveArea(int i)
    {
        if (tutorialCleared[i] == true)
            return;

        ResetRenderers();

        leftThumbStick.material = alphaOrange;

        text.text = "이동은 L스틱";

        gameObject.transform.position = tutorialAreas[0].transform.position;
        tutorialCleared[i] = true;
    }

    public void OnLookArea(int i)
    {
        ResetRenderers();

        rightThumbStick.material = alphaOrange;

        text.text = "둘러보려면 R스틱";

        gameObject.transform.position = tutorialAreas[0].transform.position;
    }

    public void OnGrapArea(int i)
    {
        ResetRenderers();

        leftBumper.material = alphaOrange;
        rightBumper.material = alphaOrange;

        text.text = "물건을 집으려면 그립 버튼";

        gameObject.transform.position = tutorialAreas[0].transform.position;
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
}
