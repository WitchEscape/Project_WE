using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cauldron : MonoBehaviour
{
    [SerializeField] private string puzzleID = "Puzzle_2";

    [Header("생성할 빛나는 돌")] public GameObject magicStone;
    [Header("휘저을때 필요한 힘")] public float churnForce = 3f;
    [Header("물약 리스트")] public List<GameObject> positions;
    [Header("성공했을 때 파티클")] public ParticleSystem successParticle;
    [Header("실패했을 때 파티클")] public ParticleSystem failParticle;
    private GhostCanvas ghostCanvas;
    
    [HideInInspector] public Observable<int> postionCount;
    [HideInInspector] public Observable<bool> isCorrect;
    [HideInInspector] public bool isFire;

    private bool[] isCorrectRune = new bool[4];
    private bool isChurned;
    private bool wasInstantiate;

    private void Awake()
    {
        ghostCanvas = FindObjectOfType<GhostCanvas>();

        foreach (GameObject obj in positions)
        {
            //리셋될 때 Transform 리셋할 수 있도록 컴포넌트 부착
            obj.AddComponent<ResetPostion>();   
        }
    }

    private void OnEnable()
    {
        postionCount.onValueChanged.AddListener(OnValueChanged);
        isCorrect.onValueChanged.AddListener(OnValueChanged);
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(PutInCauldron);
    }

    private void Start()
    {
        //NOTE : Start때 한 번 ValueChanged 이벤트가 실행될것으로 예상됨 
        //이라고 할 뻔 했으나 쓰레기값이 아닌 0으로 초기화되서 괜찮을듯함
        postionCount.Value = 0;
        //isCorrect.Value = false;

        //Debug.Log(postionCount.Value);
        //Debug.Log(isCorrect.Value);

        if (magicStone.activeSelf == true)
        {
            magicStone.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (postionCount.Value < 0) { return; }
        if (isChurned == false) { return; }

        //포션카운트가 4일때 조건 검사
        if (postionCount.Value >= 4 && isChurned)
        {
            isCorrect.Value = CheckCorrectPostion();
        }
    }

   
    private void OnValueChanged(int newValue)
    {
        isChurned = false;
        #region 기존 이벤트 방식에서 가마솥 휘젓는걸로 바뀜에따라 업데이트에서 조건검사 진행
        //Debug.Log($"(int)new Value : {newValue}, {postionCount.Value}");

        //if (postionCount.Value < 0) { return; }

        ////포션카운트가 4일때마다 조건 검사
        //if (postionCount.Value == 4)
        //{
        //    isCorrect.Value = CheckCorrectPostion();

        //    //조건 검사가 끝나면 다시 포션카운트 0으로 초기화
        //    postionCount.Value = 0;

        //    //bool변수 false로 초기화
        //    for (int i = 0; i < isCorrectRune.Length; i++)
        //    {
        //        isCorrectRune[i] = false;
        //    }
        //}
        #endregion 
    }


    private void OnValueChanged(bool newValue)
    {
        Debug.Log($"(bool)new Value : {newValue}, isCorrect : {isCorrect.Value}");

        if (isCorrect.Value == false)
        {
            //DONE : 실패했을 때 파티클 재생
            failParticle.Play();
        }
        else if (isCorrect.Value == true)
        {
            if (wasInstantiate == false)
            {
                magicStone.gameObject.SetActive(true);

                //다음에 다시 생성되지 않도록 예외처리
                wasInstantiate = true;
                ghostCanvas.ClearPuzzle(1);
            }
            DialogPlayer.Instance.PlayDialogSequence("POSSIONCLASS_03_");
            successParticle.Play();
        }

        //조건 검사가 끝나면 다시 포션카운트 0으로 초기화
        postionCount.Value = 0;
        isChurned = false;

        //bool변수 false로 초기화
        for (int i = 0; i < isCorrectRune.Length; i++)
        {
            isCorrectRune[i] = false;
        }

        //제조에 성공하던 실패하던 위치 리셋
        ResetPostions();
    }

    private void PutInCauldron(GameObject arg)
    {
        //불 안붙은 상태
        if (isFire == false)
        {
            //유니티는 SetActive를 프레임 단위로 실행하기 때문에 같은 프레임에서 false -> true해도 하나밖에 실행 안됨
            //코루틴으로 한 프레임 유예를 두고 ResetPostions 메서드 실행
            arg.gameObject.SetActive(false);
            StartCoroutine(ResetPostionCoroutine());
            return;
        }

        //0일때 H를 넣으면 true
        if (postionCount.Value == 0 && arg.CompareTag("Rune_H"))
        {
            Debug.Log("Rune_H");
            isCorrectRune[0] = true;
        }
        if (postionCount.Value == 1 && arg.CompareTag("Rune_O"))
        {
            Debug.Log("Rune_O");
            isCorrectRune[1] = true;
        }
        if (postionCount.Value == 2 && arg.CompareTag("Rune_P"))
        {
            Debug.Log("Rune_P");
            isCorrectRune[2] = true;

        }
        if (postionCount.Value == 3 && arg.CompareTag("Rune_E"))
        {
            Debug.Log("Rune_E");
            isCorrectRune[3] = true;
        }

        //가마솥 안에 있는 트리거로 들어간 물약 비활성화
        //TODL : 물약 들어갈 때 자기 위치 기억해놓았다가 다시 그쪽으로 돌아가도록 하기
        if (arg != null)
        {
            arg.SetActive(false);
        }

        postionCount.Value++;
    }

    private bool CheckCorrectPostion()
    {
        return isCorrectRune[0] && isCorrectRune[1] && isCorrectRune[2] && isCorrectRune[3];
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log($"PostionCount : {postionCount.Value}");

        if (other.CompareTag("Scoop"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Debug.Log($"Velocity : {rb.velocity.magnitude}");
                Debug.Log($"PostionCount2 : {postionCount.Value}");
            }
            //else
            //    Debug.LogWarning("RB NOT FOUND");

            if (churnForce <= rb.velocity.magnitude)
            {
                isChurned = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Scoop"))
        {
            isChurned = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Scoop"))
        {
            //isChurned = false;
        }
    }

    [ContextMenu("Test2")]
    private void ResetPostions()
    {
        Debug.Log("포션 리셋 ");
        foreach (GameObject obj in positions)
        {
            if (obj.activeSelf == false)
            {
                obj.SetActive(true);
            }
        }
    }

    private IEnumerator ResetPostionCoroutine()
    {
        yield return null;
        ResetPostions();
    }

    [ContextMenu("Test")]
    public void Test()
    {
        postionCount.Value++;
        for (int i = 0; i < isCorrectRune.Length; i++)
        {
            isCorrectRune[i] = true;
        }
    }
}