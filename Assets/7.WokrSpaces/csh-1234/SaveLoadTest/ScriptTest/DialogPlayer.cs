using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogPlayer : MonoBehaviour
{
    private static DialogPlayer instance = null;
    public static DialogPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogPlayer>();
                if (instance == null)
                {
                    GameObject go = new GameObject("@DialogPlayer");
                    instance = go.AddComponent<DialogPlayer>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private List<DataManager.DialogData> currentDialogSequence;
    private int currentDialogIndex = 0;
    private bool isPlaying = false;
    private Coroutine playCoroutine;

    [SerializeField] private float delayBetweenDialogs = 0.2f; 

    public UnityEvent<DataManager.DialogData> OnDialogStart = new UnityEvent<DataManager.DialogData>();
    public UnityEvent OnDialogEnd = new UnityEvent();
    public UnityEvent OnSequenceEnd = new UnityEvent();

    /// <summary>
    /// 대화 시작
    /// </summary>
    /// <param name="scriptIdPrefix">DialogId</param>
    public void PlayDialogSequence(string scriptIdPrefix)
    {
        StopDialog();
        
        currentDialogSequence = DataManager.Instance.GetDialogSequence(scriptIdPrefix);
        if (currentDialogSequence.Count > 0)
        {
            currentDialogIndex = 0;
            isPlaying = true;
            playCoroutine = StartCoroutine(PlayDialogCoroutine());
        }
    }

    private IEnumerator PlayDialogCoroutine()
    {
        while (isPlaying && currentDialogIndex < currentDialogSequence.Count)
        {
            DataManager.DialogData currentDialog = currentDialogSequence[currentDialogIndex];
            OnDialogStart.Invoke(currentDialog);
            yield return new WaitForSeconds(currentDialog.Duration);

            OnDialogEnd.Invoke();
            currentDialogIndex++;

            if (currentDialogIndex >= currentDialogSequence.Count)
            {
                isPlaying = false;
                OnSequenceEnd.Invoke();
            }
            else
            {
                yield return new WaitForSeconds(delayBetweenDialogs);
            }
        }
    }

    /// <summary>
    /// 일시 정지
    /// </summary>
    public void PauseDialog()
    {
        if (isPlaying)
        {
            isPlaying = false;
        }
    }

    /// <summary>
    /// 대화 재개
    /// </summary>
    public void ResumeDialog()
    {
        if (!isPlaying && currentDialogSequence != null && currentDialogIndex < currentDialogSequence.Count)
        {
            isPlaying = true;
            playCoroutine = StartCoroutine(PlayDialogCoroutine());
        }
    }

    /// <summary>
    /// 대화 강제종료
    /// </summary>
    public void StopDialog()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
        
        isPlaying = false;
        currentDialogIndex = 0;
        currentDialogSequence = null;
    }


    /// <summary>
    /// 다음 대화로 넘기기
    /// </summary>
    public void ForceNextDialog()
    {
        if (isPlaying && currentDialogSequence != null)
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
            }
            
            OnDialogEnd.Invoke();
            currentDialogIndex++;

            if (currentDialogIndex >= currentDialogSequence.Count)
            {
                isPlaying = false;
                OnSequenceEnd.Invoke();
            }
            else
            {
                playCoroutine = StartCoroutine(PlayDialogCoroutine());
            }
        }
    }

    /// <summary>
    /// 현재 재생 중인 대화 정보 반환
    /// </summary>
    /// <returns></returns>
    public DataManager.DialogData GetCurrentDialog()
    {
        if (currentDialogSequence != null && currentDialogIndex < currentDialogSequence.Count)
        {
            return currentDialogSequence[currentDialogIndex];
        }
        return null;
    }
}