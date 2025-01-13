using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using static TMPro.Examples.TMP_ExampleScript_01;

public class DataManager : MonoBehaviour
{
    private static DataManager instance = null;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("@DataManager");
                    instance = go.AddComponent<DataManager>();
                }
            }
            return instance;
        }
    }
    
    public Dictionary<string, DialogData> DialogDic { get; private set; } = new Dictionary<string, DialogData>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Initialize();
        //foreach (var item in DialogDic)
        //{
        //    Debug.Log(item);
        //}
    }


    //파일 저장 경로 설정
    private string GetDataPath(string fileName, bool isLoad = true)
    {
        if (isLoad)
        {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, "1.Resources/Data", fileName);
#else
        // 빌드된 버전에서는 StreamingAssets 폴더 사용
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "Data", fileName);
        
        // 파일이 존재하는지 확인하고 로그 출력
        if (File.Exists(streamingPath))
        {
            Debug.Log($"Data file found at: {streamingPath}");
        }
        else
        {
            Debug.LogError($"Data file not found at: {streamingPath}");
        }
        
        return streamingPath;
#endif
        }
        else
        {
            // 저장 데이터용 경로
            string savePath = Path.Combine(Application.persistentDataPath, "SaveData", fileName);

            // 저장 폴더가 없다면 생성
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            return savePath;
        }
    }

  
    public void Initialize()
    {
        LoadDialogData();
    }

    private void LoadDialogData()
    {
        string csvPath = GetDataPath("Excel/DialogData.csv", true);

        if (File.Exists(csvPath))
        {
            ParseDialogData("Dialog");
        }
    }

    public void ParseDialogData(string filename)
    {
        List<DialogData> dialogList = new List<DialogData>();
        string csvPath = GetDataPath($"Excel/{filename}Data.csv", true);
        string[] lines = File.ReadAllText(csvPath).Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0 || string.IsNullOrEmpty(row[0])) continue;

            int i = 0;
            DialogData dd = new DialogData
            {
                DialogId = ConvertValue<string>(row[i++]),
                DialogType = ConvertValue<DialogType>(row[i++]),
                SpeakerName = ConvertValue<string>(row[i++]),
                Text = ConvertValue<string>(row[i++]),
                Duration = ConvertValue<float>(row[i++])
            };
            dialogList.Add(dd);
        }

        DialogDataWrapper wrapper = new DialogDataWrapper { Dialogs = dialogList };
        DialogDic = dialogList.ToDictionary(d => d.DialogId);
    }


    #region Util
    private static T ConvertValue<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFromString(value);
    }

    private static List<T> ConvertList<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<T>();

        return value.Split('&').Select(x => ConvertValue<T>(x)).ToList();
    }


    #endregion

    #region Dialog
    [Serializable]
    public class DialogData
    {
        public string DialogId;
        public DialogType DialogType;
        public string SpeakerName;
        public string Text;
        public float Duration;     
    }

    [Serializable]
    private class DialogDataWrapper
    {
        public List<DialogData> Dialogs;
    }
    #endregion


    /// <summary>
    /// 같은 접두사를 가진 DialogId들을 List로 묶어서 리턴하는 메서드
    /// </summary>
    /// <param name="scriptIdPrefix"></param>
    /// <returns></returns>
    public List<DialogData> GetDialogSequence(string scriptIdPrefix) 
    {
        return DialogDic.Values
            .Where(d => d.DialogId.StartsWith(scriptIdPrefix))
            .OrderBy(d => d.DialogId)
            .ToList();
    }

    /// <summary>
    /// 해당하는 dialog의 대사를 리턴하는 메서드
    /// </summary>
    /// <param name="scriptId"></param>
    /// <returns></returns>
    public DialogData GetDialog(string scriptId)
    {
        DialogDic.TryGetValue(scriptId, out DialogData dialog);
        return dialog;
    }
}