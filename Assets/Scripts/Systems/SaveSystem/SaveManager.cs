using MyUtility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//Notes: Metadata is cached in the beginning for quick access.
public class SaveManager : Singleton<SaveManager> {
    
    private SaveService _saveService;
    private SaveSystemSettings _settings;

    //Cached current game data and metadata
    private GameSaveData _currentGameSaveData;
    private List<MetaData> _cachedMetaData = new List<MetaData>();
    private MetaData _currentMetaData; 
    public GameSaveData CurrentGameSaveData {
        get { return _currentGameSaveData; }
        set { _currentGameSaveData = value; }
    }
    protected override void Awake() {
        base.Awake();

        _settings = new SaveSystemSettings(null);// default settings
        _saveService = new SaveService(_settings);
        //Get all metadata at start
        _cachedMetaData = _saveService.GetAllMetaData().ToList();
    }
    protected override void OnDestroy() {
        base.OnDestroy();
    }
    public MetaData[] GetAllMetaData() {
        _cachedMetaData = _saveService.GetAllMetaData();
        return _cachedMetaData.ToArray();
    }
    public Result QuickSave() {
        string quickSaveSlotName = _settings.quickSaveFileName;
        string displayName = _settings.quickSaveDisplayName;
        return _saveService.Save(_currentGameSaveData, quickSaveSlotName, displayName);
    }
    public Result QuickLoad() {

        var sortedMetadatas = _cachedMetaData
            .OrderByDescending(meta => meta.SaveTime)
            .ToArray();
        if (sortedMetadatas.Length == 0) {
            return Result.Fail("No saved games found.");
        }
        _currentMetaData = sortedMetadatas[0];
        var loadResult = LoadGame(_currentMetaData.SavePath);
        if (loadResult.Success) {
            return Result.Ok();
        } else {
            return Result.Fail(loadResult.ErrorMessage);
        }
    }
    public Result LoadGame(string slotName) {
         var result = _saveService.Load(slotName, out GameSaveData data);
         if (result.Success) {
             CurrentGameSaveData = data;
         }
         return result;
    }
}
