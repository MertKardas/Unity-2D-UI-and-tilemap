
using System.Collections.Generic;
public class GameSaveData {
    public string SceneName;
    public string SaveName; 
    public Dictionary<string, object> DataDict = new();
    public Dictionary<string, List<string>> DynamicObjectRecord = new(); 
}
