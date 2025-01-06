using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitlePool : MonoBehaviour
{
    [SerializeField, Header("미리 생성해둘 풀")] private int poolSize = 10;
    [SerializeField, Header("미리 생성해둘 프리팹")] private GameObject subtitlePrefab;
    [SerializeField, Header("생성할 영역")] private RectTransform content;

    private Queue<TextMeshProUGUI> subtitlePool = new Queue<TextMeshProUGUI>();

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(subtitlePrefab, transform);
            TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
            obj.SetActive(false); 
            subtitlePool.Enqueue(text);
        }
    }

    public TextMeshProUGUI GetText()
    {
        Debug.Log("GetText 호출");
        TextMeshProUGUI subtitle;

        if (subtitlePool.Count > 0)
        {
            subtitle = subtitlePool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(subtitlePrefab, transform);
            subtitle = obj.GetComponent<TextMeshProUGUI>();
        }

        subtitle.gameObject.SetActive(true);
        subtitle.text = "";
        subtitle.transform.SetParent(content, false);
        return subtitle;
    }



    public void ReturnSubtitle(TextMeshProUGUI text, float delay = 0f)
    {
        if (delay <= 0f)
        {
            ReturnSubtitle(text);
        }
        else
        {
            DelayedReturn(text, delay);
        }
    }

    private void ReturnSubtitle(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(false);
        text.text = "";
        subtitlePool.Enqueue(text);
    }

    private IEnumerator DelayedReturn(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnSubtitle(text);
    }

}
