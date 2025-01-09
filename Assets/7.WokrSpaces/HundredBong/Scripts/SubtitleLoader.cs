using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;


public class SubtitleLoader : MonoBehaviour
{
    //참조받을꺼면 굳?이 Resources폴더에 안넣어도 되지않나
    //TextAsset csvFile = Resources.Load<TextAsset>($"Subtitles/{language}"); 이렇게 쓰면 되긴 하는데
    //Auto-K

    public Languages language;
    public Chapters chapters;
    [Header("CSV파일")] public TextAsset csvFile;
    //[Header("언어"), Tooltip("KR, EN")] public string currentLanguage = "KR";
    //[Header("챕터")] public int chapter;



    private List<SubtitleData> subtitles = new List<SubtitleData>();

    private void Awake()
    {
        LoadCSV();
    }

    void Start()
    {
        LoadSubtitles();

        //테스트용
        DisplaySubtitles(); 
    }
    private void Update()
    {
        //Debug.Log($"subtitles : {subtitles.Count}");
    }
    private void LoadCSV()
    {
        //Debug.Log("CSV 로드 시작");

        //스트링 빌더를 사용해 텍스트 데이터를 한 줄씩 읽어옴
        //첫 번째 줄은 헤더니까 미리 읽고 버림
        StringReader reader = new StringReader(csvFile.text);
        string header = reader.ReadLine();
        while (true)
        {


            //두 번째 줄부터 읽기 시작
            string line = reader.ReadLine();

            //더이상 읽을 라인이 없다면 루프 종료
            if (line == null)
            {
                //Debug.Log("CSV 끝");
                break;
            }
            // Debug.Log($"읽은 줄: {line}");
            //CSV파일은 쉼표로 구분됨, Split을 사용해 쉼표 단위로 쪼개면 배열에 저장됨
            string[] values = line.Split(',');

            //쪼갠 데이터를 기반으로 SubtileDate 생성
            SubtitleData subtitle = new SubtitleData
            {
                Index = int.Parse(values[0]),
                Chapter = int.Parse(values[1]),
                Language = values[2],
                Content = values[3],
                Duration = float.Parse(values[4]),
            };
            //Debug.Log($"로드된 데이터: {subtitle.Language} - {subtitle.Content}");
            //생성한 SubtitleData를 리스트에 저장
            subtitles.Add(subtitle);
        }
        //Debug.Log($"총 로드된 데이터: {subtitles.Count}");
    }

    public List<string> LoadSubtitles()
    {
        List<string> filteredSubtitles = new List<string>();
        //Debug.Log($"{subtitles.Count} & {language}");

        foreach (var subtitle in subtitles)
        {
            //Debug.Log($"{subtitle.Language} & {language}");

            if (subtitle.Language == language.ToString() && subtitle.Chapter == (int)chapters)
            {
                filteredSubtitles.Add(subtitle.Content);
            }
        }
        //Debug.Log($"필터링 : {filteredSubtitles.Count}");
        for (int i = 0; i < filteredSubtitles.Count; i++)
        {
            //Debug.Log($"{i} : {filteredSubtitles[i]}");
        }
        return filteredSubtitles;
    }

    public List<float> LoadSubtitlesDuration()
    {
        List<float> filteredSubtitlesDuration = new List<float>();

        foreach (var subtitle in subtitles)
        {
            if (subtitle.Language == language.ToString() && subtitle.Chapter == (int)chapters)
            {
                filteredSubtitlesDuration.Add(subtitle.Duration);
            }
        }

        return filteredSubtitlesDuration;
    }

    private void DisplaySubtitles()
    {
        foreach (var subtitle in subtitles)
        {
            if (subtitle.Language == language.ToString())
            {
                //Debug.Log($"[Index: {subtitle.Index}, Chapter : {subtitle.Chapter}, Laungue : {subtitle.Language}] {subtitle.Content}");
            }
        }


    }
}
public class SubtitleData
{
    public int Index;
    public int Chapter;
    public float Duration;
    public string Language;
    public string Content;
}