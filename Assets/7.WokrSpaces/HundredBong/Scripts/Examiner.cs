using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examiner : MonoBehaviour
{
    [SerializeField] private string puzzleID = "Puzzle_1";

    private GhostCanvas ghostCanvas;

    public bool[] isCorrectPostion = new bool[3];
    [Header("활성화 할 성냥")] public GameObject match;
    [Header("활성화 할 파티클")] public ParticleSystem particle;
    private bool isComplite = false;
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
        isComplite = CheckCorrectPostion();
        if (isComplite && isCleared == false)
        {
            //TODO : 발생할 이벤트 작성
            DialogPlayer.Instance.PlayDialogSequence("POSSIONCLASS_02_");
            Debug.Log("포션 퍼즐 클리어");

            match.gameObject.SetActive(true);

            particle.gameObject.SetActive(true);
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
}
