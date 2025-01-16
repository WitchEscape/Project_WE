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
    //오디오 매니저를 통해 sfx 실행 오디오매니저를 통해 bgm 실행
    private void Start()
    {
        button1 = GetComponent<Button>();
        button2 = GetComponent<Button>();
        button3 = GetComponent<Button>();
        button4 = GetComponent<Button>();
        button1.onClick.AddListener(AudioPlay);
        
    }
    public void AudioPlay()
    {
        AudioManager.Instance.PlaySFX(clip, 1.5f);
    }




}
