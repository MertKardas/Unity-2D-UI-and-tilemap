using System;
using UnityEngine;
[Serializable]
public class MetaData {
    public string id;      //File name
    public string displayName; 
    public DateTime saveTime;
    public string gameVersion;

    public MetaData(string uniqueId, string visibleName) {
        id = uniqueId;
        displayName = visibleName;
        saveTime = DateTime.Now;
        gameVersion = Application.version;
    }
}
