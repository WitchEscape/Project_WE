using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class DisplayMaterial : MonoBehaviour
{
    [SerializeField] private string puzzleID = "Puzzle_3";

    private Material material;
    public float fadeDuration = 3f;
    private float elapsedTime = 0f;

    private bool isChanging;
    private bool wasPlayParticle;
    private bool wasPlaying;

    private float erase = 1f;

    [Header("최종 퍼즐 인덱스")] public int puzzleIndex;
    [Header("마법진 생성 시 재생할 파티클")] public ParticleSystem spawnParticle;
    [Header("마법진 생성 시작시 재생할 클립")] public AudioClip spawnClip;

    private GhostCanvas ghostCanvas;

    private int playerHandCount; 

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        ghostCanvas = FindObjectOfType<GhostCanvas>();
        //if (gameObject.activeSelf == true)
        //{
        //    gameObject.SetActive(false);    
        //}
    }

    private void OnEnable()
    {
        material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
        AudioManager.Instance.PlaySFX(spawnClip);
    }

    private void Start()
    {
        isChanging = true;
    }

    private void Update()
    {
        if (isChanging)
        {
            //페이드 인 실행 및 페이드 인이 끝나면 파티클 재생
            //한 번만 실행되는데 코루틴으로 할걸 그랬나요
            FadeIn();
        }
    }

    private void FadeIn()
    {
        elapsedTime = elapsedTime + Time.deltaTime;
        float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
        material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);

        Debug.Log($"FadeIn 알파값 : {alpha} , {material.color.a}");
        if (fadeDuration <= elapsedTime)
        {
            //더이상 페이드 인 되지 않도록 설정
            isChanging = false;

            //파티클 실행
            if (spawnParticle != null)
            {
                spawnParticle.Play();
                wasPlayParticle = true;
            }
            StartCoroutine(sceneChange());
        }
    }

    IEnumerator sceneChange()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("SaveLoadTest");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() != null)
        {
            playerHandCount++;
        }
    }

    #region 임시 페이드 아웃 로직
    private void OnTriggerStay(Collider other)
    {
        //추후 Cauldron처럼 velocity 조건 추가해야하나 플레이어 손에는 리지드바디가 없어서 다른 도구로 하던가 해야함
        //1. 마법진용 plane에 리지드 바디 장착 및 Use Gravity를 false로 설정
        //2. 마법진 지우개용 도구 지정, 인벤토리 영역 및 시나리오 영역이니까 회의 거쳐야함

        if (wasPlayParticle == false) { return; }

        //1 : 불투명      0 : 투명
        if (2 <=playerHandCount && wasPlaying == false)
        {            
            //알파값을 매 프레임 감소시킴
            erase = erase - (Time.deltaTime * 0.1f);

            //새로운 투명도 적용
            material.color = new Color(material.color.r, material.color.g, material.color.b, erase);

            //파티클에도 새로운 투명도 적용
            ParticleSystem.MainModule mainModule = spawnParticle.main;
            Color startColor = mainModule.startColor.color;
            startColor.a = erase;
            mainModule.startColor = startColor;


            //투명도가 일정 이하이면
            if (material.color.a <= 0f && wasPlaying == false)
            {
                //중복 호출 방지용으로 true 설정
                wasPlaying = true;

                //투명도를 0으로 만들고
                material.color = new Color(material.color.r, material.color.g, material.color.b, 0);

                startColor.a = 0;
                mainModule.startColor = startColor;

                Debug.Log("투명도 0");

                //TODO : 대화던 씬 이동이던 다음 로직 작성
                ghostCanvas.ClearPuzzle(puzzleIndex);
                Debug.Log("모든 퍼즐 클리어");
            }
        }
    }
    #endregion

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() != null)
        {
            playerHandCount--;
        }
    }
}
