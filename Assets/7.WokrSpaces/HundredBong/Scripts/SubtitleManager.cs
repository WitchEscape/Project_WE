using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    public SubtitleLoader subtitleLoader;
    public SubtitlePool subtitlePool;

    public float displayTime = 3f;

    private List<string> subtitles;
    private List<float> subtitlesDuration;

    private void Start()
    {
        subtitles = subtitleLoader.LoadSubtitles();
        subtitlesDuration = subtitleLoader.LoadSubtitlesDuration();
        //StartCoroutine(DisplaySubtitleCoroutine());
        DisplaySubtitle(2);
    }

    public void DisplaySubtitle(int i)
    {
        TextMeshProUGUI subtitleText = subtitlePool.GetText();

        subtitleText.text = subtitles[i];
        subtitleText.color = Color.white;
        subtitlePool.ReturnSubtitle(subtitleText, subtitlesDuration[i]);
    }

    public IEnumerator DisplaySubtitleCoroutine()
    {
        //Debug.Log($"코루틴 :{subtitles.Count}");
        foreach (var content in subtitles)
        {
            //Debug.Log($"코루틴2 :{subtitles.Count}");

            TextMeshProUGUI subtitleText = subtitlePool.GetText();
            subtitleText.text = content;
            yield return new WaitForSeconds(displayTime);

            subtitlePool.ReturnSubtitle(subtitleText);
        }
    }

}
