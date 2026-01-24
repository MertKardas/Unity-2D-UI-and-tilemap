using System;
using UnityEngine;
using UnityEngine.SceneManagement;
[Serializable]
public class MetaData {
    public string SaveName;    
    public string SceneName;   
    public string DisplayName;
    public DateTime SaveTime;
    public string GameVersion;

    public MetaData(string saveName, string displayName) {
        this.SaveName = saveName;
        this.DisplayName = displayName;
        SaveTime = DateTime.Now;
        SceneName = SceneManager.GetActiveScene().name;
        GameVersion = Application.version;
    }
}
