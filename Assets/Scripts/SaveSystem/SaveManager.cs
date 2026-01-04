
#region Manager
using MyUtility;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#region Manager
public class SaveManager :  Singleton<SaveManager> {


    private SaveService _saveService;
    private CancellationTokenSource _cts;

    private string _currentSaveID;
    private string _currentDisplayName;

    private const string QuickSaveID = "QuickSave"; // Predefined ID for quick save

    protected override void Awake() {
        base.Awake();
        _saveService = new SaveService(new SaveSystemSettings());
        _cts = new CancellationTokenSource();
    }

    private void OnDestroy() {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    // Quick Save: Save the current game state to a predefined slot
    public async Task QuickSave(GameSaveData data) {
        _currentSaveID = QuickSaveID;
        _currentDisplayName = "Quick Save";

        await _saveService.SaveGameAsync(_currentSaveID, _currentDisplayName, data, _cts.Token);
    }

    // Resume Game: Load the last quick saved game state
    public async Task<GameSaveData> ResumeGame() {
        var (data, result) = await _saveService.LoadGameAsync(QuickSaveID, _cts.Token);
        if (result.Success) {
            _currentSaveID = QuickSaveID;
            _currentDisplayName = "Quick Save";
            return data;
        }
        Debug.LogError($"Failed to resume game: {result.ErrorMessage}");
        return null;
    }

    // New Game: Create a new save slot
    public async Task CreateNewGame(string visibleName, GameSaveData initialData) {
        _currentSaveID = Guid.NewGuid().ToString();
        _currentDisplayName = visibleName;

        await _saveService.SaveGameAsync(_currentSaveID, _currentDisplayName, initialData, _cts.Token);
    }

    // Save Current Game: Overwrite the current save slot
    public async Task SaveCurrentGame(GameSaveData data) {
        if (string.IsNullOrEmpty(_currentSaveID)) {
            Debug.LogError("No active save slot!");
            return;
        }
        await _saveService.SaveGameAsync(_currentSaveID, _currentDisplayName, data, _cts.Token);
    }

    // Load Game: Load a specific save slot
    public async Task<GameSaveData> LoadGame(string id) {
        var (data, result) = await _saveService.LoadGameAsync(id, _cts.Token);
        if (result.Success) {
            _currentSaveID = id;
            return data;
        }
        return null;
    }

    public async Task<List<MetaData>> GetSaveList() => await _saveService.GetAllSavesAsync(_cts.Token);
}
#endregion
#endregion