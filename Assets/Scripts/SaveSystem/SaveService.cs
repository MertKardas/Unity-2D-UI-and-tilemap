using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;

public class SaveService {
    private SaveSystemSettings _settings;
    private static readonly Encoding DefaultEncoding = Encoding.UTF8;

    public string SaveDirectory => _settings.SaveDirectory;

    public SaveService(SaveSystemSettings settings) {
        _settings = settings;
    }

    public List<MetaData> GetAllMetaData() {
        var metaDataList = new List<MetaData>();

        if (!Directory.Exists(SaveDirectory)) {
            return metaDataList;
        }

        var metaDataFileName = _settings.metaDataFileName;
        var metadataPaths = Directory.GetFiles(SaveDirectory, metaDataFileName, SearchOption.AllDirectories);

        foreach (var path in metadataPaths) {
            try {
                var json = File.ReadAllText(path, DefaultEncoding);
                var metaData = JsonConvert.DeserializeObject<MetaData>(json);

                if (metaData == null) {
                    Debug.LogWarning($"MetaData deserialize edilemedi: {path}");
                    continue;
                }

                string folderPath = Path.GetDirectoryName(path);
                string slotName = Path.GetFileName(folderPath);
                metaData.SavePath = slotName;

                metaDataList.Add(metaData);
            } catch (JsonException je) {
                Debug.LogError($"JSON parse hatasý (Dosya: {path}): {je.Message}");
            } catch (System.Exception e) {
                Debug.LogError($"Save dosyasý okunurken hata oluþtu (Dosya: {path}): {e.Message}");
            }
        }

        return metaDataList;
    }

    public Result Save(GameSaveData saveData, string slotName, string displayName) {
        if (saveData == null) {
            return Result.Fail("SaveData null olamaz.");
        }

        if (string.IsNullOrEmpty(slotName)) {
            return Result.Fail("Slot adý boþ olamaz.");
        }

        var savePath = Path.Combine(SaveDirectory, slotName);
        try {
            if (!Directory.Exists(savePath)) {
                Directory.CreateDirectory(savePath);
            }

            var saveDataJson = JsonConvert.SerializeObject(saveData, _settings.jsonSettings);
            File.WriteAllText(
                Path.Combine(savePath, _settings.saveDataFileName),
                saveDataJson,
                DefaultEncoding);

            var metaData = new MetaData(slotName, displayName);
            var metaDataJson = JsonConvert.SerializeObject(metaData, _settings.jsonSettings);
            File.WriteAllText(
                Path.Combine(savePath, _settings.metaDataFileName),
                metaDataJson,
                DefaultEncoding);

            return Result.Ok();
        } catch (System.Exception e) {
            return Result.Fail($"Save iþlemi baþarýsýz oldu: {e.Message}");
        }
    }

    public Result Load(string slotName, out GameSaveData saveData) {
        saveData = null;

        if (string.IsNullOrEmpty(slotName)) {
            return Result.Fail("Geçersiz slot adý.");
        }

        var savePath = Path.Combine(SaveDirectory, slotName, _settings.saveDataFileName);
        try {
            if (!File.Exists(savePath)) {
                return Result.Fail("Save dosyasý bulunamadý.");
            }

            var saveDataJson = File.ReadAllText(savePath, DefaultEncoding);
            saveData = JsonConvert.DeserializeObject<GameSaveData>(saveDataJson, _settings.jsonSettings);

            if (saveData == null) {
                return Result.Fail("Save dosyasý bozuk veya uyumsuz.");
            }

            return Result.Ok();
        } catch (JsonException je) {
            return Result.Fail($"Save dosyasý parse edilemedi: {je.Message}");
        } catch (System.Exception e) {
            return Result.Fail($"Load iþlemi baþarýsýz oldu: {e.Message}");
        }
    }

    public Result DeleteSave(string slotName) {
        if (string.IsNullOrEmpty(slotName)) {
            return Result.Fail("Geçersiz slot adý.");
        }

        var savePath = Path.Combine(SaveDirectory, slotName);
        try {
            if (Directory.Exists(savePath)) {
                Directory.Delete(savePath, true);
                return Result.Ok();
            } else {
                return Result.Fail("Silinecek save dosyasý bulunamadý.");
            }
        } catch (System.Exception e) {
            return Result.Fail($"Delete iþlemi baþarýsýz oldu: {e.Message}");
        }
    }
}