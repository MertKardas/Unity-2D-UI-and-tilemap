using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
public class SceneSaveLoader : MonoBehaviour
{
    bool _isApplicationQuiting;
   
    private void Awake() {
    
        SceneManager.sceneLoaded += DistributeGameData;

    }
    
    
    private void OnApplicationQuit() {
        _isApplicationQuiting = true;
 
        CollectData();
    }
    private void OnDestroy() {
        if (!_isApplicationQuiting) {
            // Oyun bitmiyor
            if(GameManager.Instance.CurrentGameState != GameState.Gameover)
                CollectData();
        }
    }

    private void DistributeGameData(Scene scene, LoadSceneMode mode) {
        if(scene.name == "MainMenu") 
            return;
        var manager = SaveManager.Instance;
        var data = manager.CurrentGameSaveData;

        if (data == null) {
            var newData = new GameSaveData();
            manager.CurrentGameSaveData = newData;
            return;
        }
        //find all ISavable in the scene
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .OfType<ISavable>()
            .ToArray();
        //restore their state
        foreach (var item in savable) {
            if(data.DataDict.TryGetValue(item.UniqueId, out object state)) {
                item.RestoreState(state);
            }
        }
       
    }
    public void CollectData() {

        Debug.Log("Collecting scene data for autosave...");

        var data = SaveManager.Instance.CurrentGameSaveData;
        if(data == null) {
            data = new GameSaveData();
            SaveManager.Instance.CurrentGameSaveData = data;
        }
        //find all ISavable in the scene
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISavable>()
            .ToArray();
        //capture their state
        foreach (var item in savable) {
            data.DataDict[item.UniqueId] = item.CaptureState();
        }
        SaveManager.Instance.QuickSave();
    }
}
