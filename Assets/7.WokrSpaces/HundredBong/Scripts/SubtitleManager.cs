using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    public SubtitleLoader subtitleLoader;
    public SubtitlePool subtitlePool; 

    public float displayTime = 3f;

    private List<string> subtitles;

    private void Start()
    {
        subtitles = subtitleLoader.LoadSubtitles();
        StartCoroutine(DisplaySubtitles());
    }

    private IEnumerator DisplaySubtitles()
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
