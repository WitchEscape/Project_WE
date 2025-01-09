using UnityEngine.AI;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.GraphicsBuffer;

public class GhostController : MonoBehaviour
{
    public Transform ghostCallPos;
    public Transform mainCamera;
    private NavMeshAgent agent;
    private FinitStateMachine fsm;
    private Animator anim;
    private XRGrabInteractable grab;

    //순찰 지점
    [Header("유령 웨이포인트")]public Transform[] wayPoints = new Transform[4];
    [Header("애니메이션 파라미터")] public string animationName;

    // 플래그 추가
    private bool hasSetDestination = false;
    //다음 순찰을 위한 인덱스값
    private int nextPoint = 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fsm = GetComponent<FinitStateMachine>();
        anim = GetComponentInChildren<Animator>();
        grab = GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        Idle();
        Move();
        Talk();
    }

    private void Idle()
    {
        if (fsm.currentState == State.Idle)
        {
            //설정된 목표 초기화 및 이동 중지
            agent.ResetPath();

            //agent.isStopped = true;

            anim.SetBool("Idle", true);
            anim.SetBool("Move", false);

        }
    }

    private void Move()
    {
        if (fsm.currentState == State.Move)
        {
            anim.SetBool("Idle", false);
            anim.SetBool("Move", true);
            float dis = Vector3.Distance(transform.position, wayPoints[nextPoint].position);

            //Debug.Log($"Distance : {dis}");

            //정찰 포인트와의 거리가 0.5이하면
            if (dis < 0.5f)
            {
                //포인트를 바꿔줌
                ChangePoint();
            }

            //미리 정해놓은 정찰포인트로 이동
            agent.SetDestination(wayPoints[nextPoint].position);
        }
    }

    private void ChangePoint()
    {
        //최대 인덱스값이 3이므로 3이되면 다시 0으로 되돌려줌
        if (nextPoint == 3)
            nextPoint = 0;
        //그런거 아니면 인덱스값 1 증가시켜서 포인트 바꿔줌
        else
            nextPoint++;

        //두 달 전에는 이따구로 하드코딩했구나 알수있었던 유익한 시간이였읍니다.
    }

    private void Talk()
    {
        if (fsm.currentState == State.Talk)
        {
            // 목적지 설정은 한 번만 실행
            if (hasSetDestination == false)
            {
                agent.SetDestination(ghostCallPos.transform.position);
                hasSetDestination = true;
            }

            //Talk 상태 동안 플레이어를 바라보게 설정
            Vector3 direction = mainCamera.position - transform.position;
            //Y축 제거
            direction.y = 0;
            //방향이 0이 아닌 경우에만 회전
            if (direction != Vector3.zero) 
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            
        }
        else
        {
            //Talk 상태를 벗어나면 플래그 초기화
            //Debug.LogWarning("플래그 초기화");
            hasSetDestination = false;
        }
    }

    [ContextMenu("Animation Trigger Test")]
    private void PlayAnimation()
    {
        anim.SetTrigger($"{animationName}");
    }
}