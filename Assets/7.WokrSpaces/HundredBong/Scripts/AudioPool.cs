using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    //풀에 미리 생성해둘 오디오 소스의 수
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxPoolSize = 50;
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();

    private static AudioPool instance;

    public static AudioPool Instance { get { return instance; } }


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

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = CreateNewAudioSource();
            source.gameObject.SetActive(false);
            audioSourcePool.Enqueue(source);
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject obj = new GameObject("PooledSFX");
        AudioSource audioSource = obj.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1.0f; // 3D 사운드
        audioSource.minDistance = 10f;
        audioSource.maxDistance = 200f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0.0f;
        audioSource.playOnAwake = false;

        DontDestroyOnLoad(obj);
        return audioSource;
    }

    public AudioSource GetAudioSource()
    {
        //큐에 있는 오디오 소스를 꺼내서 활성화하는 메서드
        //남아있는 오디오 소스가 0보다 많으면 실행됨
        if (audioSourcePool.Count > 0)
        {
            //Dequeue 한 번 실행되면 Count--됨 
            AudioSource source = audioSourcePool.Dequeue();
            //비활성화되있는 오브젝트를 활성화 시켜줌
            source.gameObject.SetActive(true);
            return source;
        }
        else
        {
            Debug.Log("풀이 가득참, 생성 시도");
        }
        Debug.Log($"Max : {maxPoolSize} , Count : {audioSourcePool.Count}");
        if (maxPoolSize <= audioSourcePool.Count)
        {
            Debug.LogWarning("오디오 풀 크기가 최대값을 초과함, 생성 중단");
            return null; // 최대 크기 초과 시 null 반환
        }


        AudioSource newSource = CreateNewAudioSource();
        newSource.gameObject.SetActive(true);
        DontDestroyOnLoad(newSource);
        return newSource;
    }

    public void ReturnAudioSource(AudioSource source)
    {
        if (source == null)
        {
            Debug.LogWarning($"오디오 소스가 null상태임");
            return;
        }
        source.Stop();
        source.volume = 1f;
        source.clip = null;
        source.gameObject.SetActive(false);

        if (audioSourcePool.Count < maxPoolSize)
        { 
            audioSourcePool.Enqueue(source);
        }
        else
        {
            Debug.LogWarning("풀 크기 초과됨, 소스 제거");
            Destroy(source.gameObject);
        }

    }
}
