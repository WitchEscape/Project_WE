using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleCanvas : CanvasFollow
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 2f;

    protected override void OnEnable()
    {
        base.OnEnable();
        fadeCanvasGroup.alpha = 1f;
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        //시작전 혹시 모르니 알파값 초기화
        fadeCanvasGroup.alpha = 1;

        float timer = 0f;

        //페이드 지속시간동안 반복
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            //fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            //화면이 너무 확확 돌아가는 느낌을 받아서 Lerp보다 부드러운 SmoothStep 사용, 인자로 들어가는 값은 Lerp랑 같음
            fadeCanvasGroup.alpha = Mathf.SmoothStep(1, 0, timer / fadeDuration);

            yield return null;
        }
        //마무리 하고도 초기화 진행
        fadeCanvasGroup.alpha = 0;
    }
}
