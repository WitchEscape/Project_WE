using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    ObjectData GetSaveData();
    void LoadFromSaveData(ObjectData data);
}
