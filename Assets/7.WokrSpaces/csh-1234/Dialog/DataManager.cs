using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

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
    public Dictionary<string, UserData> UserDic { get; private set; } = new Dictionary<string, UserData>();



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
            return Path.Combine(Application.dataPath, "1.Resources/Data/Excel", fileName);
#else
            // 빌드된 버전에서는 StreamingAssets 폴더 사용
            return Path.Combine(Application.streamingAssetsPath, "Data/Excel", fileName);
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
        TextAsset csvFile = null;

#if UNITY_EDITOR
        // 에디터에서는 기존 경로도 시도해봅니다
        string csvPath = GetDataPath("DialogData.csv", true);
        if (File.Exists(csvPath))
        {
            try
            {
                string csvText = File.ReadAllText(csvPath);
                ParseDialogDataFromText(csvText);
                return;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"파일 직접 읽기 실패, Resources 폴더에서 시도합니다: {e.Message}");
            }
        }
#endif

        // Resources 폴더에서 로드 시도
        csvFile = Resources.Load<TextAsset>("Data/DialogData");
        
        if (csvFile != null)
        {
            ParseDialogDataFromText(csvFile.text);
        }
        else
        {
            Debug.LogError("DialogData.csv 파일을 찾을 수 없습니다. Resources/Data 폴더에 파일을 넣어주세요.");
        }
    }

    private void ParseDialogDataFromText(string csvText)
    {
        string[] lines = csvText.Split('\n');
        List<DialogData> dialogList = new List<DialogData>();

        for (int y = 1; y < lines.Length; y++)
        {
            if (string.IsNullOrEmpty(lines[y].Trim())) continue;

            List<string> row = new List<string>();
            bool inQuotes = false;
            string currentValue = "";
            
            foreach (char c in lines[y].Replace("\r", ""))
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }
                
                if (c == ',' && !inQuotes)
                {
                    row.Add(currentValue.Trim());
                    currentValue = "";
                    continue;
                }
                
                currentValue += c;
            }
            
            if (!string.IsNullOrEmpty(currentValue))
            {
                row.Add(currentValue.Trim());
            }

            if (row.Count == 0 || string.IsNullOrEmpty(row[0])) continue;

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