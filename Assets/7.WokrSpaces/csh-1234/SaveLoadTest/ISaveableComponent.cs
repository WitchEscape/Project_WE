using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveableComponent
{
    void CollectSaveData(Dictionary<string, object> saveData);
    void LoadFromSaveData(Dictionary<string, object> saveData);
}
