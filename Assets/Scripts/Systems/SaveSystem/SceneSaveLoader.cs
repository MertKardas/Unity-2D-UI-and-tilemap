using UnityEngine;
using System.Linq;
public class SceneSaveLoader : MonoBehaviour
{
    bool _isApplicationQuiting;
    private void Awake() {
        DistributeGameData();
        
    }
    private void Start() {
        
    }
    private void OnApplicationQuit() {
        _isApplicationQuiting = true;
        CollectData();
    }
    private void OnDestroy() {
        if (!_isApplicationQuiting) {
            CollectData();
        }
    }

    private void DistributeGameData() {
        var manager = SaveManager.Instance;
        var data = manager.CurrentGameSaveData;

        if (data == null) {
            var newData = new GameSaveData();
            manager.CurrentGameSaveData = newData;
            return;
        }
        //find all ISavable in the scene
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
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
        var isGameOver = GameManager.Instance.CurrentGameState == GameState.Gameover;
        if (isGameOver) return; 

        Debug.Log("Collecting scene data for autosave...");
        var saveManager = SaveManager.Instance;
        var data = saveManager.CurrentGameSaveData;

        //find all ISavable in the scene
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISavable>()
            .ToArray();
        //capture their state
        foreach (var item in savable) {
            data.DataDict[item.UniqueId] = item.CaptureState();
        }

        //update scene name       
        data.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        saveManager.CurrentGameSaveData = data;
        saveManager.QuickSave();
    }
}
