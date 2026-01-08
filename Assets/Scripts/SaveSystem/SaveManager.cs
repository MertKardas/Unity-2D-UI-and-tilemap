using MyUtility;
using UnityEngine;
using Newtonsoft.Json;
public class SaveManager : Singleton<SaveManager> {
    private SaveService _saveService;
    private SaveSystemSettings _settings;
    private GameSaveData currentGameSaveData;
    public GameSaveData CurrentGameSaveData {
        get { return currentGameSaveData; }
        set { currentGameSaveData = value; }
    }
    protected override void Awake() {
        base.Awake();

        _settings = new SaveSystemSettings(null);// default settings
        _saveService = new SaveService(_settings);

    }
    protected override void OnDestroy() {
        base.OnDestroy();
    }
    public MetaData[] GetAllMetaData() {
        var metaDataList = _saveService.GetAllMetaData();
        return metaDataList.ToArray();
    }
    public Result QuickSave(GameSaveData saveData) {
        string quickSaveSlotName = _settings.quickSaveFileName;
        string displayName = _settings.quickSaveDisplayName;
        return _saveService.Save(saveData, quickSaveSlotName, displayName);
    }
    public Result LoadGame(string slotName) {
         var result = _saveService.Load(slotName, out GameSaveData data);
         if (result.Success) {
             CurrentGameSaveData = data;
         }
         return result;
    }
}
