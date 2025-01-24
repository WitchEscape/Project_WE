using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examiner : MonoBehaviour
{
    [SerializeField] private string puzzleID = "PotionClass_Puzzle_01";

    private GhostCanvas ghostCanvas;

    public bool[] isCorrectPostion = new bool[3];
    [Header("활성화 할 성냥")] public GameObject match;
    [Header("활성화 할 파티클")] public ParticleSystem particle;
    [Header("활성화 할 캔버스")] public CanvasGroup canvas;
    [Header("퍼즐 풀렸을때 낼 사운드")] public AudioClip clip;
    [Header("성냥 숨겨놓은 캐비넷 조인트 2개")] public HingeJoint[] joints; 

    private bool isComplete = false;
    private bool isCleared = false;

    private TriggerZone[] triggerZone = new TriggerZone[3];

    private void Awake()
    {
        ghostCanvas = FindObjectOfType<GhostCanvas>();
        triggerZone = GetComponentsInChildren<TriggerZone>();
    }

    private void OnEnable()
    {
        for (int i = 0; triggerZone.Length > i; i++)
        {
            int index = i;
            triggerZone[index].OnEnterEvent.AddListener(x => SetPostionEnter(index));
            triggerZone[index].OnExitEvent.AddListener(x => SetPostionExit(index));
        }
    }

    private void OnDisable()
    {
        for (int i = 0; triggerZone.Length > i; i++)
        {
            int index = i;
            triggerZone[index].OnEnterEvent.RemoveListener(x => SetPostionEnter(index));
            triggerZone[index].OnExitEvent.RemoveListener(x => SetPostionExit(index));
        }
    }

    private void Start()
    {
        if (match != null && match.activeSelf == true)
        {
            match.gameObject.SetActive(false);
        }

        if (particle != null && particle.gameObject.activeSelf == true)
        {
            particle.Stop();
            particle.gameObject.SetActive(false);
        }
    }

    public void SetPostionEnter(int i)
    {
        isCorrectPostion[i] = true;
        isComplete = CheckCorrectPostion();
        if (isComplete && isCleared == false)
        {
            //DialogPlayer.Instance.PlayDialogSequence("LOBBY_11_");
            PuzzleProgressManager.Instance.CompletePuzzle(puzzleID);

            match.gameObject.SetActive(true);
            canvas.gameObject.SetActive(true);

            if (clip != null)
            {
                AudioManager.Instance.PlaySFX(clip);
            }

            SetJoint();


        //DONE: 서랍 열리는 소리 재생

            particle.gameObject?.SetActive(true);
            particle.Play();

            isCleared = true;

            ghostCanvas.ClearPuzzle(0);
        }
    }

    public void SetPostionExit(int i)
    {
        isCorrectPostion[i] = false;
    }

    private bool CheckCorrectPostion()
    {
        return isCorrectPostion[0] && isCorrectPostion[1] && isCorrectPostion[2];
    }

    private void SetJoint()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            HingeJoint hingeJoint = joints[i];
            JointLimits limits = hingeJoint.limits;
            limits.max = 120f;
            hingeJoint.limits = limits;
            Invoke("SetMotor", 1f);
        }
    }

    private void SetMotor()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].useMotor = false;
        }
    }
    
}
