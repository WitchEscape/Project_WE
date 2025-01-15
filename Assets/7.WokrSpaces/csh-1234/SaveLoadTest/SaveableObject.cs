using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SaveableObject : MonoBehaviour, ISaveable
{
    public enum SaveableType
    {
        InteractableObject,
        Puzzle,
        Inventory,
        Player,
    }

    [SerializeField] private string uniqueID;
    [SerializeField] private SaveableType objectType;

    public SaveableType ObjectType => objectType;
    public string UniqueID => uniqueID;

    private void Awake()
    {
        // uniqueID가 없으면 새로 생성
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = gameObject.name;
        }
    }

    public ObjectData GetSaveData()
    {
        ObjectData data = new ObjectData
        {
            uniqueID = uniqueID,
            objectType = objectType,
            position = transform.position,
            rotation = transform.rotation,
            customData = new SerializableCustomData()
        };

        // 임시 Dictionary를 사용하여 데이터 수집
        var tempDict = new Dictionary<string, object>();
        var saveComponents = GetComponents<ISaveableComponent>();

        foreach (var component in saveComponents)
        {
            if (component != null)
            {
                component.CollectSaveData(tempDict);
            }
        }
        data.customData.FromDictionary(tempDict);

        return data;
    }

    public void LoadFromSaveData(ObjectData data)
    {
        if (data == null)
        {
            Debug.LogError($"ObjectData is null for {gameObject.name}");
            return;
        }

        transform.position = data.position;
        transform.rotation = data.rotation;
        
        // 위치정보를 제외한 데이터 로드 및 복구(ex.TMP)
        var customDict = new Dictionary<string, object>();
        foreach (var entry in data.customData.entries)
        {
            customDict[entry.key] = entry.value;
        }
        
        var saveComponents = GetComponents<ISaveableComponent>();
        foreach (var component in saveComponents)
        {
            if (component != null)
            {
                component.LoadFromSaveData(customDict);
            }
        }
    }
}