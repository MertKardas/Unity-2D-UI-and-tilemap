using MyUtility;
using System.Linq;
using UnityEngine.SceneManagement;

//Notes: Metadata is cached in the beginning for quick access.
public class SaveManager : Singleton<SaveManager> {
    
    private SaveService _saveService;
    private SaveSystemSettings _settings;

    //Cached current game data and metadata
    private GameSaveData _currentGameSaveData;

    public GameSaveData CurrentGameSaveData {
        get { return _currentGameSaveData; }
        set { _currentGameSaveData = value; }
    }
    protected override void Awake() {
        base.Awake();

        _settings = new SaveSystemSettings(null);// default settings
        _saveService = new SaveService(_settings);
        
    }
    protected override void OnDestroy() {
        base.OnDestroy();
    }
    
    public Result QuickSave() {
        string quickSaveSlotName = _settings.quickSaveFileName;
        string displayName = _settings.quickSaveDisplayName;
        string sceneName = SceneManager.GetActiveScene().name;
        MetaData metaData = new MetaData(quickSaveSlotName, displayName, sceneName);    
        _currentGameSaveData.SceneName = sceneName;
        return _saveService.Save(_currentGameSaveData, metaData);
    }
    public Result QuickLoad() {

        var sortedMetadatas = _saveService.GetAllMetaData()
            .OrderByDescending(meta => meta.SaveTime)
            .ToArray();
        if (sortedMetadatas.Length == 0) {
            return Result.Fail("No saved games found.");
        }
        var metadataToLoad = sortedMetadatas[0];
        var loadResult = LoadGame(metadataToLoad.SaveName);
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
    public Result LoadAllMetadata(out MetaData[] metadatas) {
        metadatas = _saveService.GetAllMetaData().ToArray();
        if (metadatas != null) {
            return Result.Ok();
        } else {
            return Result.Fail("Failed to load metadata.");
        }
    }
}
