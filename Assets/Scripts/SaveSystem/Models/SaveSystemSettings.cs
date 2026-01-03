using Newtonsoft.Json;

public class SaveSystemSettings {
    public bool enableEncryption = true;
    public bool enableBackup = true;
    public string saveDirectory = "Saves";
    public int maxBackupCount = 3;
    public JsonSerializerSettings jsonSettings;

    public SaveSystemSettings() {
        jsonSettings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
            //Auto type handling gibi ayarlar eklenebilir
            TypeNameHandling = TypeNameHandling.Auto
        };
    }
}