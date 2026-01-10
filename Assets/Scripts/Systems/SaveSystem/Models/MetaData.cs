using System;
using UnityEngine;
[Serializable]
public class MetaData {
    public string SaveName;    
    public string SceneName;   
    public string DisplayName;
    public DateTime SaveTime;
    public string GameVersion;

    public MetaData(string saveName, string displayName, string sceneName) {
        this.SaveName = saveName;
        this.DisplayName = displayName;
        SaveTime = DateTime.Now;
        SceneName = sceneName;
        GameVersion = Application.version;
    }
}
