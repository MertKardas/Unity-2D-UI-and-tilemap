using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SaveService : ISaveService {
    private readonly SaveSystemSettings _settings;
    private readonly IEncryptionService _encryptionService;
    private readonly SemaphoreSlim _saveLock = new SemaphoreSlim(1, 1);

    // Anahtar Yönetimi (Not: Prodüksiyonda daha güvenli saklanmalý)
    private string EncryptionKey {
        get {
            if (!PlayerPrefs.HasKey("SaveKey")) {
                using (var rng = new RNGCryptoServiceProvider()) {
                    byte[] key = new byte[32];
                    rng.GetBytes(key);
                    PlayerPrefs.SetString("SaveKey", Convert.ToBase64String(key));
                }
            }
            return PlayerPrefs.GetString("SaveKey");
        }
    }

    private string SaveDirectory => Path.Combine(Application.persistentDataPath, _settings.saveDirectory);
    private string MetaDataPath => Path.Combine(SaveDirectory, "MetaData.json");

    public SaveService(SaveSystemSettings settings) {
        _settings = settings ?? new SaveSystemSettings();
        _encryptionService = new AESEncryptionService(); // Basit implementasyon aþaðýda

        if (!Directory.Exists(SaveDirectory)) Directory.CreateDirectory(SaveDirectory);
    }

    public async Task<SaveResult> SaveGameAsync(string uniqueId, string displayName, GameSaveData data, CancellationToken ct) {
        // DÜZELTME: Spam engellemek için WaitAsync(0) kontrolü
        if (!await _saveLock.WaitAsync(0, ct)) {
            Debug.LogWarning("Save system is busy.");
            return SaveResult.Fail("System busy");
        }

        try {
            string savePath = GetSavePath(uniqueId);

            // 1. Yedekle (Backup)
            if (_settings.enableBackup && File.Exists(savePath)) {
                await RotateBackupsAsync(uniqueId, ct);
            }

            // 2. Serialize & Encrypt
            string json = await Task.Run(() => JsonConvert.SerializeObject(data, _settings.jsonSettings), ct);
            if (_settings.enableEncryption) {
                json = await Task.Run(() => _encryptionService.Encrypt(json, EncryptionKey), ct);
            }

            // 3. Atomic Write (Temp -> Move)
            string tempPath = savePath + ".tmp";
            await File.WriteAllTextAsync(tempPath, json, ct);

            if (File.Exists(savePath)) File.Delete(savePath);
            File.Move(tempPath, savePath);

            // 4. Metadata Güncelle
            await UpdateMetaDataAsync(uniqueId, displayName, ct);

            Debug.Log($"Saved: {uniqueId} ({displayName})");
            return SaveResult.Ok();
        } catch (Exception e) {
            Debug.LogError($"Save Failed: {e.Message}");
            return SaveResult.Fail(e.Message);
        } finally {
            _saveLock.Release();
        }
    }

    public async Task<(GameSaveData data, SaveResult result)> LoadGameAsync(string uniqueId, CancellationToken ct) {
        if (!await _saveLock.WaitAsync(0, ct)) return (null, SaveResult.Fail("System busy"));

        try {
            string savePath = GetSavePath(uniqueId);
            if (!File.Exists(savePath)) return (null, SaveResult.Fail("File not found"));

            string json = await File.ReadAllTextAsync(savePath, ct);

            if (_settings.enableEncryption) {
                json = await Task.Run(() => _encryptionService.Decrypt(json, EncryptionKey), ct);
            }

            var data = await Task.Run(() => JsonConvert.DeserializeObject<GameSaveData>(json, _settings.jsonSettings), ct);
            return (data, SaveResult.Ok());
        } catch (Exception e) {
            // Burada backup'tan restore denenebilir (Basitlik için çýkardým)
            Debug.LogError($"Load Failed: {e.Message}");
            return (null, SaveResult.Fail(e.Message));
        } finally {
            _saveLock.Release();
        }
    }

    public async Task<List<MetaData>> GetAllSavesAsync(CancellationToken ct) {
        try {
            if (!File.Exists(MetaDataPath)) return new List<MetaData>();
            string json = await File.ReadAllTextAsync(MetaDataPath, ct);
            return JsonConvert.DeserializeObject<List<MetaData>>(json) ?? new List<MetaData>();
        } catch { return new List<MetaData>(); }
    }

    public async Task<SaveResult> DeleteSaveAsync(string uniqueId, CancellationToken ct) {
        if (!await _saveLock.WaitAsync(0, ct)) return SaveResult.Fail("System busy");
        try {
            // Dosyayý sil
            string path = GetSavePath(uniqueId);
            if (File.Exists(path)) File.Delete(path);

            // Metadata'dan sil
            var allMeta = await GetAllSavesAsync(ct);
            allMeta.RemoveAll(x => x.id == uniqueId);

            // Metadata'yý tekrar kaydet
            string json = JsonConvert.SerializeObject(allMeta, _settings.jsonSettings);
            await File.WriteAllTextAsync(MetaDataPath, json, ct);

            return SaveResult.Ok();
        } finally { _saveLock.Release(); }
    }

    // Helper: Backup Döndürme
    private async Task RotateBackupsAsync(string uniqueId, CancellationToken ct) {
        string savePath = GetSavePath(uniqueId);
        // .bak2 -> .bak3, .bak1 -> .bak2 mantýðý
        for (int i = _settings.maxBackupCount - 1; i > 0; i--) {
            string oldB = savePath + $".bak{i - 1}";
            string newB = savePath + $".bak{i}";
            if (File.Exists(oldB)) {
                if (File.Exists(newB)) File.Delete(newB);
                File.Move(oldB, newB);
            }
        }
        // Mevcut dosyanýn yedeðini al (.bak0)
        byte[] content = await File.ReadAllBytesAsync(savePath, ct);
        await File.WriteAllBytesAsync(savePath + ".bak0", content, ct);
    }

    private async Task UpdateMetaDataAsync(string uniqueId, string displayName, CancellationToken ct) {
        var list = await GetAllSavesAsync(ct);
        var existing = list.Find(x => x.id == uniqueId);

        if (existing != null) {
            existing.saveTime = DateTime.Now;
            existing.displayName = displayName; // Ýsim güncellenmiþ olabilir
        } else {
            list.Add(new MetaData(uniqueId, displayName));
        }

        string json = JsonConvert.SerializeObject(list, _settings.jsonSettings);
        await File.WriteAllTextAsync(MetaDataPath, json, ct);
    }

    private string GetSavePath(string id) => Path.Combine(SaveDirectory, $"Save_{id}.json");
}
