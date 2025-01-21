using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;
using static UserData;
using UnityEngine.Events;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance = null;
    public static SaveLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveLoadManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("@SaveLoadManager");
                    instance = go.AddComponent<SaveLoadManager>();
                }
            }
            return instance;
        }
    }

    public static event Action OnLoadComplete;

    private string savePath;
    public bool isDataLoadScene = false;

    public int CurrentClearStage = 1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        // 유니티 에디터에서 실행 시 프로젝트 내부 경로 사용
        savePath = Path.Combine(Application.dataPath, "1.Resources/Data/SaveData");
        #else
            // 빌드된 게임에서는 persistentDataPath 사용
            savePath = Path.Combine(Application.persistentDataPath, "SaveData");
        #endif
        Directory.CreateDirectory(savePath);
    }

    public void SaveUserData()
    {
        Debug.Log($"Save Start UserData");
        try
        {
            UserData userData = new UserData();
            userData.baseData = CollectUserData();

            string json = JsonUtility.ToJson(userData, true);
            string filePath = Path.Combine(savePath, $"save_UserData.json");
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }
    public void LoadUserData()
    {
        Debug.Log($"Load Start UserData");
        try
        {
            string filePath = Path.Combine(savePath, $"save_UserData.json");
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Save file not found: {filePath}");
                return;
            }
            string json = File.ReadAllText(filePath);
            UserData userData = JsonUtility.FromJson<UserData>(json);
            CurrentClearStage = userData.baseData.StageProgress;
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }

    public void SaveGame(string SceneName)
    {
        Debug.Log($"Save Start {SceneName}");
        SaveUserData();
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

            // 인벤토리 데이터 저장
            var inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                var inventoryItems = inventoryManager.GetInventoryData();
                saveData.inventoryData = inventoryItems;
                
                // 인벤토리에 있는 아이템 ID 저장
                saveData.removedFromWorldItems = inventoryItems
                    .Where(item => !string.IsNullOrEmpty(item.itemUniqueID))
                    .Select(item => item.itemUniqueID)
                    .ToList();
            }

            string json = JsonUtility.ToJson(saveData, true);
            string filePath = Path.Combine(savePath, $"save_{SceneName}.json");
            File.WriteAllText(filePath, json);

            //SceneManager.LoadScene("WE_Level_Tuturial");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    public void LoadGame(string SceneName)
    {
        Debug.Log($"Load Stage {SceneName}");
        LoadUserData();
        try
        {
            string filePath = Path.Combine(savePath, $"save_{SceneName}.json");
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Save file not found: {filePath}");
                return;
            }

            // 현재 씬의 인벤토리 초기화
            var currentInventoryManager = FindObjectOfType<InventoryManager>();
            if (currentInventoryManager != null)
            {
                currentInventoryManager.ClearInventory();
            }

            string json = File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            
            if(isDataLoadScene == true)
            {
                // 씬 로드 후에 데이터를 로드하도록 이벤트를 등록
                UnityAction<Scene, LoadSceneMode> onSceneLoaded = null;
                onSceneLoaded = (scene, mode) =>
                {
                    if (scene.name == SceneName)  // 원하는 씬이 로드됐을 때만 실행
                    {
                        StartCoroutine(LoadGameWithDelay(saveData));
                        SceneManager.sceneLoaded -= onSceneLoaded;  // 이벤트 한 번만 실행되도록 제거
                    }
                };
                SceneManager.sceneLoaded += onSceneLoaded;
            }
            isDataLoadScene = false;
            SceneManager.LoadScene(SceneName);
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }

    private IEnumerator LoadGameWithDelay(SaveData saveData)
    {
        yield return new WaitForSeconds(0.5f); 

        var inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null && saveData.inventoryData != null)
        {
            inventoryManager.LoadInventoryData(saveData.inventoryData);
            yield return new WaitForSeconds(0.2f);
        }
        LoadGameData(saveData);
        OnLoadComplete?.Invoke();
    }

    private void LoadGameData(SaveData saveData)
    {
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
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
                
                // 인벤토리에 있는 아이템인지 확인
                var itemData = obj.GetComponent<InteractableItemData>();
                if (itemData != null)
                {
                    // 인벤토리 매니저에서 현재 인벤토리에 있는 아이템의 ID 목록 가져오기
                    var inventoryManager = FindObjectOfType<InventoryManager>();
                    if (inventoryManager != null)
                    {
                        var inventoryItems = inventoryManager.GetInventoryData();
                        data.isInInventory = inventoryItems.Any(item => item.itemUniqueID == obj.UniqueID);
                    }
                }
                
                objectsData.Add(data);
            }
        }

        return objectsData;
    }

    private BaseData CollectUserData()
    {
        BaseData baseData = new BaseData();
        if(SaveLoadManager.instance != null)
        {
            baseData.StageProgress = SaveLoadManager.instance.CurrentClearStage;
        }
        else
        {
            //일단 없으면 터지니까 1로 초기화 하겠음
            baseData.StageProgress = 1;
        }
        return baseData;
    }

    #endregion

    #region LoadData
    private void LoadPlayerData(PlayerData playerData)
    {
        Debug.Log("로드 플레이어 데이터 호출됨~!!!!");
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

            //xrOrigin.MoveCameraToWorldLocation(Vector3.zero);
            //xrOrigin.RotateAroundCameraPosition(Vector3.up, 0f);

            xrOrigin.MoveCameraToWorldLocation(playerData.position);
            Vector3 currentRotation = xrOrigin.Camera.transform.eulerAngles;
            float rotationDifference = playerData.rotation.eulerAngles.y - currentRotation.y;
            xrOrigin.RotateAroundCameraPosition(Vector3.up, playerData.rotation.eulerAngles.y);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void LoadWorldObjects(List<ObjectData> objectsData)
    {
        try
        {
            // 먼저 인벤토리에 있는 아이템의 ID 목록을 가져옵니다
            HashSet<string> inventoryItemIDs = new HashSet<string>();
            
            // 저장된 removedFromWorldItems 목록 사용
            var saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(Path.Combine(savePath, $"save_{SceneManager.GetActiveScene().name}.json")));
            if (saveData.removedFromWorldItems != null)
            {
                foreach (var id in saveData.removedFromWorldItems)
                {
                    inventoryItemIDs.Add(id);
                    Debug.Log($"[SaveLoadManager] 제거 대상 아이템 ID: {id}");
                }
            }

            // 현재 씬의 모든 SaveableObject를 찾아서 처리합니다
            var foundObjects = FindObjectsOfType<SaveableObject>();
            Debug.Log($"[SaveLoadManager] 씬에서 발견된 SaveableObject 수: {foundObjects.Length}");

            foreach (var obj in foundObjects)
            {
                if (obj != null && !string.IsNullOrEmpty(obj.UniqueID))
                {
                    if (inventoryItemIDs.Contains(obj.UniqueID))
                    {
                        Debug.Log($"[SaveLoadManager] 인벤토리에 있는 아이템 제거: {obj.name} (ID: {obj.UniqueID})");
                        DestroyImmediate(obj.gameObject);
                    }
                }
            }

            // 남은 오브젝트들에 대해 저장된 데이터 적용
            Dictionary<string, SaveableObject> remainingObjects = new Dictionary<string, SaveableObject>();
            foreach (var obj in FindObjectsOfType<SaveableObject>()) // 다시 검색
            {
                if (!string.IsNullOrEmpty(obj.UniqueID))
                {
                    remainingObjects[obj.UniqueID] = obj;
                }
            }

            // 저장된 데이터 로드
            foreach (var data in objectsData)
            {
                if (data == null || data.isInInventory) continue;

                if (remainingObjects.TryGetValue(data.uniqueID, out var targetObject))
                {
                    targetObject.LoadFromSaveData(data);
                    remainingObjects.Remove(data.uniqueID);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
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


}
