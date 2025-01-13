using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance;
    public static SaveLoadManager Instance => instance;

    private string savePath;

    private void Awake()
    {
        instance = this;
        #if UNITY_EDITOR
            // 유니티 에디터에서 실행 시 프로젝트 내부 경로 사용
            savePath = Path.Combine(Application.dataPath, "1.Resources/Data/SaveData");
        #else
            // 빌드된 게임에서는 persistentDataPath 사용
            savePath = Path.Combine(Application.persistentDataPath, "SaveData");
        #endif
        Directory.CreateDirectory(savePath);

        Debug.Log($"Save path: {savePath}");
    }


    public void SaveGame(string SceneName)
    {
        Debug.Log($"Save Start {SceneName}");
        try
        {
            SaveData saveData = new SaveData();
            
            // 현재 시간 설정
            saveData.saveTime = DateTime.Now;
            Debug.Log($"Saving at: {saveData.saveTime}");
            
            // 월드 오브젝트 저장
            saveData.worldObjects = CollectWorldObjectsData();
            saveData.playerData = CollectPlayerData();
            
            // 퍼즐 진행도 저장
            if (PuzzleProgressManager.Instance != null)
            {
                string currentStage = SceneManager.GetActiveScene().name;
                Dictionary<string, object> puzzleData = new Dictionary<string, object>();
                PuzzleProgressManager.Instance.SavePuzzleProgress(puzzleData);

                if (puzzleData.TryGetValue("puzzleStates", out object states))
                {
                    var stageProgress = new Dictionary<string, Dictionary<string, PuzzleProgressManager.PuzzleData>>
                    {
                        [currentStage] = (Dictionary<string, PuzzleProgressManager.PuzzleData>)states
                    };
                    saveData.SetStageProgress(stageProgress);
                }
            }

            string json = JsonUtility.ToJson(saveData, true);
            string filePath = Path.Combine(savePath, $"save_{SceneName}.json");
            File.WriteAllText(filePath, json);
            
            Debug.Log($"Save completed for {SceneName} at {saveData.saveTime}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    public void LoadGame(string SceneName)
    {
        Debug.Log($"Load Stage {SceneName}");
        try
        {
            string filePath = Path.Combine(savePath, $"save_{SceneName}.json");
            string json = File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // 오브젝트 불러오기
            LoadWorldObjects(saveData.worldObjects);
            LoadPlayerData(saveData.playerData);

            // 퍼즐 진행도 불러오기
            var puzzleManager = PuzzleProgressManager.Instance;
            if (puzzleManager != null)
            {
                string currentStage = SceneManager.GetActiveScene().name;
                var stageProgress = saveData.GetStageProgress();
                
                if (stageProgress.TryGetValue(currentStage, out var stageData))
                {
                    Dictionary<string, object> puzzleData = new Dictionary<string, object>
                    {
                        ["puzzleStates"] = stageData
                    };
                    puzzleManager.LoadPuzzleProgress(puzzleData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }


    #region CollectData
    private PlayerData CollectPlayerData()
    {
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin != null)
        {
            PlayerData data = new PlayerData
            {
                position = xrOrigin.Camera.transform.position,
                rotation = xrOrigin.Camera.transform.rotation
            };
            return data;
        }

        return null;
    }

    private List<ObjectData> CollectWorldObjectsData()
    {
        List<ObjectData> objectsData = new List<ObjectData>();
        SaveableObject[] saveableObjects = FindObjectsOfType<SaveableObject>();
        Debug.Log($"Found {saveableObjects.Length} SaveableObjects");

        foreach (var obj in saveableObjects)
        {
            if (obj.ObjectType == SaveableObject.SaveableType.InteractableObject)
            {
                ObjectData data = obj.GetSaveData();
                objectsData.Add(data);
            }
        }

        return objectsData;
    }

    #endregion

    #region LoadData
    private void LoadPlayerData(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("playerData is null");
            return;
        }

        try
        {
            var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin == null)
            {
                Debug.LogError("XR Origin not found in scene!");
                return;
            }

            xrOrigin.MoveCameraToWorldLocation(playerData.position);
            Vector3 currentRotation = xrOrigin.Camera.transform.eulerAngles;
            float rotationDifference = playerData.rotation.eulerAngles.y - currentRotation.y;
            xrOrigin.RotateAroundCameraPosition(Vector3.up, rotationDifference * Mathf.Deg2Rad);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void LoadWorldObjects(List<ObjectData> objectsData)
    {
        if (objectsData == null)
        {
            Debug.LogError("objectsData is null");
            return;
        }

        try
        {
            Dictionary<string, SaveableObject> existingObjects = new Dictionary<string, SaveableObject>();
            var foundObjects = FindObjectsOfType<SaveableObject>();

            foreach (var obj in foundObjects)
            {
                if (string.IsNullOrEmpty(obj.UniqueID) == false)
                {
                    existingObjects[obj.UniqueID] = obj;
                }
            }

            foreach (var data in objectsData)
            {
                if (data == null)
                {
                    Debug.LogWarning("ObjectData is null");
                    continue;
                }

                SaveableObject targetObject;
                if (existingObjects.TryGetValue(data.uniqueID, out targetObject))
                {
                    targetObject.LoadFromSaveData(data);
                    existingObjects.Remove(data.uniqueID);
                }
                else
                {
                    Debug.LogWarning($"unique ID Error");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    #endregion

    // 저장된 모든 스테이지 데이터 정보 가져오기
    public List<SaveFileInfo> GetSavedStageFiles()
    {
        List<SaveFileInfo> saveFiles = new List<SaveFileInfo>();
        
        try
        {
            if (!Directory.Exists(savePath))
                return saveFiles;

            string[] files = Directory.GetFiles(savePath, "save_*.json");
            foreach (string file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                    
                    saveFiles.Add(new SaveFileInfo
                    {
                        StageName = Path.GetFileNameWithoutExtension(file).Replace("save_", ""),
                        SaveTime = saveData.saveTime
                    });
                    Debug.Log(Path.GetFileNameWithoutExtension(file).Replace("save_", ""));
                    Debug.Log(saveData.saveTime);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error reading save file {file}: {e.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error getting save files: {e.Message}");
        }

        return saveFiles;
    }

    // 특정 스테이지의 저장 데이터 삭제
    public void DeleteSaveFile(string stageName)
    {
        try
        {
            string filePath = Path.Combine(savePath, $"save_{stageName}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Deleted save file for stage: {stageName}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deleting save file: {e.Message}");
        }
    }

    public class SaveFileInfo
    {
        public string StageName;
        public DateTime SaveTime;
    }
}
