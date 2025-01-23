using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAlpha : MonoBehaviour
{
    private CanvasGroup canvas;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        canvas.alpha = 0;
        StartCoroutine(DisplayCoroutine());
    }

    private IEnumerator DisplayCoroutine()
    {
        canvas.alpha = 0;
        while (canvas.alpha < 1)
        {
            canvas.alpha += Time.deltaTime * 0.33f;
            yield return null;
        }
        canvas.alpha = 1;
    }
}
