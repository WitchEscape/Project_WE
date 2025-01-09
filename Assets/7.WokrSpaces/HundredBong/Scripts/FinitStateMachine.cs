using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public enum State { Idle, Move, Talk }

public class FinitStateMachine : MonoBehaviour
{
    //초기상태
    public State currentState = State.Idle;
    [Header("유령 호출키")] public InputActionProperty callAction;
    [Header("유령 호출 쿨타임")] public float callInterval = 120f;

    private float lastCallTime;
    private Coroutine stateCoroutine;


    private void Awake()
    {

    }

    private void Start()
    {
        //시작시 쿨타임 초기화
        lastCallTime = Time.time - callInterval;
    }


    void Update()
    {

        //Debug.Log($"현재 상태 : {currentState}");
        //Debug.Log($"현재 코루틴 상태 : {stateCoroutine}");

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Move:
                Move();
                break;
            case State.Talk:
                Talk();
                break;
        }
    }

    private void Idle()
    {
        if (stateCoroutine == null)
        {
            //Debug.Log($"코루틴 시작");
            stateCoroutine = StartCoroutine(SetStateCoroutine());
        }

        //Idle에서 호출 버튼 누르면 하던 행동 종료하고 Talk상태로 전환
        if (callAction.action.WasPressedThisFrame() && IsCallOnCooldown() == false)
        {
            if (stateCoroutine != null)
            {
                StopCoroutine(stateCoroutine);
                stateCoroutine = null;
            }
            ChangeState(State.Talk);
        }
    }

    private IEnumerator SetStateCoroutine()
    {
        //Debug.Log($"코루틴 진입");

        //랜덤한 시간 대기
        yield return new WaitForSeconds(Random.Range(2, 5));

        //랜덤한 인덱스 생성
        State nextState = (State)Random.Range(0, 2);

        // Debug.Log($"랜덤 인덱스 {nextState}");

        //기획팀에 물어보고 Move -> 랜덤 포인트 도착 -> 일정시간 Idle -> 다시 Move 상태로 전환 반복할건지 회의
        if (nextState == currentState)
        {
            //Debug.Log("같은 상태 유지됨. 코루틴 다시 시작");
            //기존 코루틴 초기화 및 새로운 코루틴 시작
            stateCoroutine = null;
            stateCoroutine = StartCoroutine(SetStateCoroutine());
            yield break;
        }

        switch (nextState)
        {
            case State.Idle:
                ChangeState(State.Idle);
                break;
            case State.Move:
                ChangeState(State.Move);
                break;
        }

    }

    private void Move()
    {
        if (stateCoroutine == null)
        {
            Debug.Log("Move 코루틴 시작");
            stateCoroutine = StartCoroutine(SetStateCoroutine());
        }

        //플레이어가 호출하면 Talk 상태로 전환
        if (callAction.action.WasPressedThisFrame() && IsCallOnCooldown() == false)
        {
            ChangeState(State.Talk);
        }
    }

    private void Talk()
    {
        if (currentState != State.Talk) { return; }

        //Talk상태에서 한번 더 누르면 Talk 종료
        if (callAction.action.WasPressedThisFrame())
        {
            EndTalkByButton();
        }
    }

    private void EndTalkByButton()
    {
        //쿨타임은 초기화하지 않음
        ChangeState(State.Move);
    }

    private void EndTalkByEvent()
    {
        if (currentState != State.Talk) { return; }

        //힌트를 받아서 이벤트에서 호출되면 쿨타임 초기화 후 상태 전환
        lastCallTime = Time.time;
        ChangeState(State.Move);
    }

    private void ChangeState(State newState)
    {
        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
        }
        currentState = newState;
    }

    [ContextMenu("Change Talk Test")]
    private void TestTalk()
    {
        //if (IsCallOnCooldown() == false)
        //{
        lastCallTime = Time.time;
        ChangeState(State.Talk);
        // }
        //else
        //{
        //   Debug.LogError($"쿨타임 에러, last : {lastCallTime}, interval : {callInterval}, {Time.time}");
        // }
    }

    [ContextMenu("Change Move Test")]
    private void TestMove()
    {
        ChangeState(State.Move);
    }


    private bool IsCallOnCooldown()
    {
        //쿨타임 검사
        return Time.time - lastCallTime < callInterval;
    }





}

