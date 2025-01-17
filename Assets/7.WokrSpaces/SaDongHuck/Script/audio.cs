using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class audio : MonoBehaviour
{
    //버튼 참조
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;
    public AudioClip clip;
    public AudioClip bgmClip;
    //오디오 매니저를 통해 sfx 실행 오디오매니저를 통해 bgm 실행
    private void Start()
    {
        button1.onClick.AddListener(AudioPlay);
        button2.onClick.AddListener(PlayBGM);

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        
    }
    public void AudioPlay()
    {
        AudioManager.Instance.SFX.PlayOneShot(clip, 1.5f);
    }

    public void PlayBGM()
    {
        AudioManager.Instance.BGM.clip = bgmClip;
        AudioManager.Instance.BGM.volume = 1.0f;
        AudioManager.Instance.BGM.Play();
    }



}
