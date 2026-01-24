using System.Collections.Generic;

[System.Serializable]
public class SceneSaveData
{
    public Dictionary<string, object> SaveObjects = new();
    public List<string> DynamicObjectIds = new();
}
