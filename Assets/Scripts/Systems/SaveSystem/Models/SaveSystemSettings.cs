using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveSystemSettings {
    public string SaveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
    public string quickSaveFileName = "quickSave";
    public string quickSaveDisplayName = "Quick Save";
    public string metaDataFileName = "metadata.meta";
    public string saveDataFileName = "savedata.json";
    public JsonSerializerSettings jsonSettings;

    public SaveSystemSettings(JsonSerializerSettings jsonFormatting) {
        //default settings
        if (jsonFormatting == null)
        {
            jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            };
        }
        else
        {
            jsonSettings = jsonFormatting; 
        }
     
    }
}