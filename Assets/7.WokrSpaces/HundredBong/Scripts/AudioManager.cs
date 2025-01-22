using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource SFX;
    public AudioSource BGM;
    public AudioSource Voice;

    [Header("Lobby, 535, Dormitory, Postion, Library, Teacher, Totorial, Ending")] public AudioClip[] backgroundMusic;
    [HideInInspector]public float SFXVolume = 1f;
    [HideInInspector]public float BGMVolume = 1f;

    private static AudioManager instance;

    public static AudioManager Instance { get { return instance; } }

    private Dictionary<Chapters, AudioClip> bgmDictionary = new Dictionary<Chapters, AudioClip>();

    public Chapters currentChapter;

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

    private void OnEnable()
    {
        Application.focusChanged += OnApplicationFocusChanged;
    }

    private void OnDisable()
    {
        Application.focusChanged -= OnApplicationFocusChanged;
    }

    private void OnApplicationFocusChanged(bool hasFocus)
    {
        if (hasFocus)
        {
            BGM.UnPause();
            Voice.UnPause();
        }
        else
        {
            BGM.Pause();
            Voice.Pause();
        }
    }  
    private void Start()
    {
        bgmDictionary.Add(Chapters.Lobby, backgroundMusic[0]);
        bgmDictionary.Add(Chapters.Class535, backgroundMusic[1]);
        bgmDictionary.Add(Chapters.Dormitory, backgroundMusic[2]);
        bgmDictionary.Add(Chapters.PostionClass, backgroundMusic[3]);
        bgmDictionary.Add(Chapters.Library, backgroundMusic[4]);
        bgmDictionary.Add(Chapters.TeachersRoom, backgroundMusic[5]);
        bgmDictionary.Add(Chapters.Totorial, backgroundMusic[6]);
        bgmDictionary.Add(Chapters.Ending, backgroundMusic[7]);

        SFXVolume = 1f;
        BGMVolume = 1f;
        BGM.loop = true;


        BGMChange(currentChapter);
    }


    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        //2D 사운드 재생용 메서드. 다른 클립 재생중이여도 동시에 재생 가능
        if (clip != null)
        {
            SFX.PlayOneShot(clip, volume * SFX.volume);
        }
    }

    public void PlaySFX(AudioClip clip, Vector3 pos, Transform parent = null, float volume = 1f)
    {
        //3D 사운드, 오디오 리스너와의 거리, 방향에 따라 소리가 달라짐
        if (clip != null)
        {
            //풀에서 AudioSource 가져오기
            AudioSource tempAudioSource = AudioPool.Instance.GetAudioSource();
            tempAudioSource.transform.position = pos;

            if (parent != null)
            {
                Debug.Log("부모 설정");
                tempAudioSource.transform.SetParent(parent);
            }

            tempAudioSource.clip = clip;
            tempAudioSource.volume = tempAudioSource.volume * volume;
            tempAudioSource.Play();

            //재생후 다시 풀에 넣어줌
            StartCoroutine(ReturnToPool(tempAudioSource, clip.length));
        }
    }

    private IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (source == null)
        {
            Debug.LogWarning("오디오 소스가 파괴된 상태에서 접근함");
            yield break;
        }
        else if (source.gameObject == null)
        {
            Debug.LogWarning("게임 오브젝트가 파괴된 상태에서 접근함");
            yield break;
        }

        AudioPool.Instance.ReturnAudioSource(source);
    }

    public void BGMChange(Chapters chapter)
    {
        int chapterIndex = (int)chapter;
        if (chapterIndex >= 0 && chapterIndex < backgroundMusic.Length)
        {
            // 선택된 BGM을 재생하도록 PlayBGM 호출
            PlayBGM(backgroundMusic[chapterIndex]);
        }
        else
        {
            Debug.LogWarning($"{chapter}에 BGM할당 안됨");
        }
    }

    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        Debug.Log($"PlayBGM , {clip.name}");
        //이미 재생중인 BGM이 있다면 멈추고 재생
        if (BGM.isPlaying)
        {
            BGM.Stop();
        }

        BGM.clip = clip;
        BGM.volume = volume;
        BGM.Play();
    }

    public void SetVolume(float BGMvalue, float SFXVolume)
    {
        BGM.volume = BGMvalue;
        SFX.volume = SFXVolume;
    }
}
