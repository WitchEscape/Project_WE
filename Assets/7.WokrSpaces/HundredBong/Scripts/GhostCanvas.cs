using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostCanvas : MonoBehaviour
{
    private CanvasGroup canvas;

    [SerializeField, Header("Yes버튼")] private Button yesButton;
    [SerializeField, Header("No버튼")] private Button noButton;

    [SerializeField,Header("난이도")]

    private Button[] selectHintButtons;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0;
        canvas.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        yesButton.onClick.AddListener(OnClickYes);
        noButton.onClick.AddListener(OnClickNo);
    }

    private void OnClickYes()
    {

    }

    private void OnClickNo()
    {
        StartCoroutine(DisableCanvasCoroutine());
    }

    public IEnumerator DisableCanvasCoroutine()
    {
        while (true)
        {
            canvas.alpha = canvas.alpha - (Time.deltaTime * 0.5f);

            if (canvas.alpha <= 0)
            {
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }

    }

    private void OnDisable()
    {
        canvas.alpha = 0f;
        yesButton.onClick.RemoveListener(OnClickYes);
        noButton.onClick.RemoveListener(OnClickNo);
    }
}
