using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayParticle : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    private float fadeDuration = 3f; // 알파값 감소에 걸리는 시간 (초)
    private float elapsedTime = 0f;  // 경과 시간

    private void Awake()
    {
        // 자식 오브젝트의 모든 ParticleSystem 가져오기
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Start()
    {
        foreach (ParticleSystem p in particleSystems)
        {
            p.Play();
        }
    }

    private void Update()
    {
        // 경과 시간 계산
        elapsedTime += Time.deltaTime;
        float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration); // 알파값 점진적으로 감소

        foreach (ParticleSystem particleSystem in particleSystems)
        {         
            // ParticleSystem의 MainModule 가져오기
            var mainModule = particleSystem.main;

            // 기존 startColor 가져오기
            Color startColor = mainModule.startColor.color;

            // 알파값 수정
            startColor.a = alpha;

            // 수정된 색상 적용
            mainModule.startColor = startColor;
        }

        // fadeDuration 이후 업데이트 멈춤
        if (elapsedTime >= fadeDuration)
        {
            enabled = false; // Update() 멈추기
        }
    }
}
