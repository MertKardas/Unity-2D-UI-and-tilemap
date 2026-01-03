using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#region Manager
public class SaveManager : MonoBehaviour {
    public static SaveManager Instance { get; private set; }

    private SaveService _saveService;
    private CancellationTokenSource _cts;

    // Þu an yüklü olan oyunun ID'si ve Görünen Ýsmi
    private string _currentSaveID;
    private string _currentDisplayName;

    private void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _saveService = new SaveService(new SaveSystemSettings());
        _cts = new CancellationTokenSource();
    }

    private void OnDestroy() {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    // YENÝ OYUN: Yeni bir ID (Guid) üretir
    public async Task CreateNewGame(string visibleName, GameSaveData initialData) {
        _currentSaveID = Guid.NewGuid().ToString(); // "550e8400-e29b..."
        _currentDisplayName = visibleName;          // "Karakter 1"

        await _saveService.SaveGameAsync(_currentSaveID, _currentDisplayName, initialData, _cts.Token);
    }

    // VAROLAN OYUNU KAYDET (Overwrite)
    public async Task SaveCurrentGame(GameSaveData data) {
        if (string.IsNullOrEmpty(_currentSaveID)) {
            Debug.LogError("No active save slot!");
            return;
        }
        await _saveService.SaveGameAsync(_currentSaveID, _currentDisplayName, data, _cts.Token);
    }

    // OYUN YÜKLE
    public async Task<GameSaveData> LoadGame(string id) {
        var (data, result) = await _saveService.LoadGameAsync(id, _cts.Token);
        if (result.Success) {
            _currentSaveID = id;
            // MetaData'dan display name'i de çekmek gerekebilir ama basitleþtirdim
            return data;
        }
        return null;
    }

    public async Task<List<MetaData>> GetSaveList() => await _saveService.GetAllSavesAsync(_cts.Token);
}
#endregion