using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TarotNumber : MonoBehaviour
{
    private SpriteRenderer spriteColor;

    private void Awake()
    {
        spriteColor = GetComponent<SpriteRenderer>();

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        spriteColor.color = new Color(spriteColor.color.r, spriteColor.color.g, spriteColor.color.b, 0f);
        StartCoroutine(DisplayImageCoroutine());

    }

    private void Start()
    {
    }

    private IEnumerator DisplayImageCoroutine()
    {
        float alpha = 0;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * 0.2f;

            if (alpha > 1f)
            {
                alpha = 1f;
            }

            spriteColor.color = new Color(spriteColor.color.r, spriteColor.color.g, spriteColor.color.b, alpha);

            yield return null;
        }
    }
}
