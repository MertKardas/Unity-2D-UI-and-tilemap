using System.Collections.Generic;

public class GameSaveData
{
    public string SaveName;
    public string LastSceneName;

    // Key = scene name, Value = that scene's save data
    public Dictionary<string, SceneSaveData> SceneData = new();

    public GameSaveData(string saveName)
    {
        SaveName = saveName;
    }

    public SceneSaveData GetOrCreateSceneData(string sceneName)
    {
        if (!SceneData.ContainsKey(sceneName))
            SceneData[sceneName] = new SceneSaveData();
        return SceneData[sceneName];
    }
}
