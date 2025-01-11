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
    private GhostCanvas canvas;


    //순찰 지점
    [Header("유령 웨이포인트")] public Transform[] wayPoints = new Transform[4];
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
        canvas = GetComponentInChildren<GhostCanvas>();
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
        nextPoint = (nextPoint + 1) % wayPoints.Length;
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

            LookPlayer();

            //agent.pathPending : 경로 계산이 완료되었는지 검사함, 계산중인경우 Distance가 정확하지 않을 수 있음
            //agent.remainingDistance : 목적지까지의 남은 거리를 반환함 stoppingDistance를 활용하여 도착 여부 확인함
            if (agent.pathPending == false && agent.remainingDistance <= agent.stoppingDistance)
            {
                //hasPath: 경로가 유효한지 확인함
                //velocity.sqrMagnitude == 0f: NavMeshAgent가 정지했는지 확인함
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    StartCoroutine(canvas.FadeInCanvasCoroutine());
                }
            }
        }
        else
        {
            //Talk 상태를 벗어나면 플래그 초기화
            //Debug.LogWarning("플래그 초기화");
            hasSetDestination = false;

            ////캔바스 비활성화
            //if (canvas.gameObject.activeSelf == true)
            //{
            //    canvas.gameObject.SetActive(false);
            //}
        }
    }

    private void LookPlayer()
    {
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
    private void DisplayCanvas()
    {
        //if (canvas.gameObject.activeSelf == false)
        //{
        //    canvas.gameObject.SetActive(true);
        //}
        //if (canvas.alpha <= 1)
        //{
        //    canvas.alpha = canvas.alpha + (Time.deltaTime * fadeSpeed);
        //}

        //float dis = Vector3.Distance(gameObject.transform.position, mainCamera.transform.position);

        ////목적지에 도착한 상태고, 플레이어와의 거리가 1 이하인 동안
        //if (hasSetDestination == true && dis <= 1)
        //{
        //    //캔바스 활성화, 상태머신에서 Talk상태를 빠져나가면 캔버스 비활성화됨
        //    canvas.gameObject.SetActive(true);
        //}
    }

    [ContextMenu("Animation Trigger Test")]
    private void PlayAnimation()
    {
        anim.SetTrigger(animationName);
    }
}