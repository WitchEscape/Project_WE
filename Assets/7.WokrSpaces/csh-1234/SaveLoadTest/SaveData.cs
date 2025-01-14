using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public PlayerData playerData;
    public List<ObjectData> worldObjects;
    public List<StageProgressData> stageProgressList;
    public string saveTimeString;
    public List<InventorySlotData> inventoryData;
    public List<string> removedFromWorldItems;

    public DateTime saveTime
    {
        get
        {
            if (string.IsNullOrEmpty(saveTimeString))
                return DateTime.Now;
            return DateTime.Parse(saveTimeString);
        }
        set
        {
            saveTimeString = value.ToString("o");
        }
    }

    [Serializable]
    public class StageProgressData
    {
        public string stageID;
        public List<PuzzleProgressData> puzzleDataList;

        public StageProgressData(string id)
        {
            stageID = id;
            puzzleDataList = new List<PuzzleProgressData>();
        }
    }

    [Serializable]
    public class PuzzleProgressData
    {
        public string puzzleID;
        public PuzzleProgressManager.PuzzleState state;
        public string nextPuzzleID;
        public List<SerializableKeyValuePair> progressData;

        public PuzzleProgressData()
        {
            progressData = new List<SerializableKeyValuePair>();
        }
    }

    [Serializable]
    public class SerializableKeyValuePair
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class InventorySlotData
    {
        public string itemPrefabPath;
        public int slotIndex;
        public string itemUniqueID;
    }

    public Dictionary<string, Dictionary<string, PuzzleProgressManager.PuzzleData>> GetStageProgress()
    {
        var result = new Dictionary<string, Dictionary<string, PuzzleProgressManager.PuzzleData>>();
        
        if (stageProgressList != null)
        {
            foreach (var stageData in stageProgressList)
            {
                var puzzleDict = new Dictionary<string, PuzzleProgressManager.PuzzleData>();
                foreach (var puzzleData in stageData.puzzleDataList)
                {
                    var puzzleProgressData = new PuzzleProgressManager.PuzzleData
                    {
                        puzzleID = puzzleData.puzzleID,
                        state = puzzleData.state,
                        nextPuzzleID = puzzleData.nextPuzzleID
                    };

                    puzzleProgressData.progressData = new Dictionary<string, object>();
                    foreach (var kvp in puzzleData.progressData)
                    {
                        puzzleProgressData.progressData[kvp.key] = JsonUtility.FromJson<object>(kvp.value);
                    }

                    puzzleDict[puzzleData.puzzleID] = puzzleProgressData;
                }
                result[stageData.stageID] = puzzleDict;
            }
        }
        return result;
    }

    public void SetStageProgress(Dictionary<string, Dictionary<string, PuzzleProgressManager.PuzzleData>> stageProgress)
    {
        stageProgressList = new List<StageProgressData>();
        
        foreach (var stagePair in stageProgress)
        {
            var stageData = new StageProgressData(stagePair.Key);
            
            foreach (var puzzlePair in stagePair.Value)
            {
                var puzzleData = new PuzzleProgressData
                {
                    puzzleID = puzzlePair.Value.puzzleID,
                    state = puzzlePair.Value.state,
                    nextPuzzleID = puzzlePair.Value.nextPuzzleID,
                    progressData = new List<SerializableKeyValuePair>()
                };

                foreach (var kvp in puzzlePair.Value.progressData)
                {
                    puzzleData.progressData.Add(new SerializableKeyValuePair
                    {
                        key = kvp.Key,
                        value = JsonUtility.ToJson(kvp.Value)
                    });
                }

                stageData.puzzleDataList.Add(puzzleData);
            }
            stageProgressList.Add(stageData);
        }
    }

    public SaveData()
    {
        playerData = new PlayerData();
        worldObjects = new List<ObjectData>();
        stageProgressList = new List<StageProgressData>();
        saveTime = DateTime.Now;
        removedFromWorldItems = new List<string>();
    }
}

[System.Serializable]
public class PlayerData
{
    // 사실상 카메라 위치임
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class ObjectData
{
    public string uniqueID;
    public SaveableObject.SaveableType objectType;
    public Vector3 position;
    public Quaternion rotation;
    public SerializableCustomData customData;
    public bool isInInventory;

    public ObjectData()
    {
        customData = new SerializableCustomData();
        isInInventory = false;
    }
}

[System.Serializable]
public class SerializableCustomData
{
    public List<CustomDataEntry> entries = new List<CustomDataEntry>();

    public void FromDictionary(Dictionary<string, object> dict)
    {
        entries.Clear();
        foreach (var kvp in dict)
        {
            string value = kvp.Value as string;
            if (value == null)
            {
                value = JsonUtility.ToJson(kvp.Value);
            }

            entries.Add(new CustomDataEntry
            {
                key = kvp.Key,
                value = value
            });
        }
    }
}

[System.Serializable]
public class CustomDataEntry
{
    public string key;
    public string value;
}