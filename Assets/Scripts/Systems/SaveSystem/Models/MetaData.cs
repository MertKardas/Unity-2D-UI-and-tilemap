using System;
using UnityEngine;
[Serializable]
public class MetaData {
    public string SavePath;      
    public string DisplayName;
    public DateTime SaveTime;
    public string GameVersion;

    public MetaData(string savePath, string displayName) {
        this.SavePath = savePath;
        this.DisplayName = displayName;
        SaveTime = DateTime.Now;
        GameVersion = Application.version;
    }
}
