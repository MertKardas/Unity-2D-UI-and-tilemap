using MyUtility;
using System.Linq;
using UnityEngine.SceneManagement;

//Notes: Metadata is cached in the beginning for quick access.
public class SaveManager : Singleton<SaveManager>
{
    private SaveService _saveService;
    private SaveSystemSettings _settings;
    private SceneSaveLoader _sceneDataLoader;

    //Cached current game data and metadata
    private GameSaveData _currentGameSaveData;

    public GameSaveData CurrentGameSaveData
    {
        get { return _currentGameSaveData; }
        set { _currentGameSaveData = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        _sceneDataLoader = new();
        _settings = new SaveSystemSettings(null); // default settings
        _saveService = new SaveService(_settings);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if(scene.name == SceneManager.GetSceneByBuildIndex(0).name)
            {
                CurrentGameSaveData = null;
                return; 
            }
            if(CurrentGameSaveData != null)
            DistributeSaveData();
        };
    }

    public void DistributeSaveData()
    {
        if (CurrentGameSaveData != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            _sceneDataLoader.DistributeGameData(CurrentGameSaveData, currentScene);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public Result QuickSave()
    {
        string quickSaveSlotName = _settings.quickSaveFileName;
        string displayName = _settings.quickSaveDisplayName;
        MetaData metaData = new MetaData(quickSaveSlotName, displayName);

        if (CurrentGameSaveData == null)
        {
            CurrentGameSaveData = new GameSaveData(quickSaveSlotName);
        }

        string currentScene = SceneManager.GetActiveScene().name;
        CurrentGameSaveData.LastSceneName = currentScene;

        // Get or create scene-specific data and collect current scene's objects
        var sceneData = _sceneDataLoader.CollectSceneData();
        CurrentGameSaveData.SceneData[currentScene] = sceneData;

        return _saveService.Save(_currentGameSaveData, metaData);
    }

    public Result QuickLoad()
    {
        var sortedMetadatas = _saveService.GetAllMetaData()
            .OrderByDescending(meta => meta.SaveTime)
            .ToArray();

        if (sortedMetadatas.Length == 0)
        {
            return Result.Fail("No saved games found.");
        }

        var metadataToLoad = sortedMetadatas[0];
        var loadResult = LoadGame(metadataToLoad.SaveName);

        if (loadResult.Success)
        {
            return Result.Ok();
        }
        else
        {
            return Result.Fail(loadResult.ErrorMessage);
        }
    }

    public Result LoadGame(string slotName)
    {
        //adjust current game save data
        var result = _saveService.Load(slotName, out GameSaveData data);
        if (result.Success)
        {
            CurrentGameSaveData = data;
        }
        return result;
    }

    public Result LoadAllMetadata(out MetaData[] metadatas)
    {
        metadatas = _saveService.GetAllMetaData().ToArray();
        if (metadatas != null)
        {
            return Result.Ok();
        }
        else
        {
            return Result.Fail("Failed to load metadata.");
        }
    }

    /// <summary>
    /// Gets the last scene the player was in when they saved
    /// </summary>
    public string GetLastSavedSceneName()
    {
        return CurrentGameSaveData?.LastSceneName;
    }
}
