using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using NaughtyAttributes;
public class SceneSaveLoader : MonoBehaviour
{
    bool _isApplicationQuiting;
   
    private void Awake() {
    
        SceneManager.sceneLoaded += DistributeGameData;

    }
    [Button]
    public void SaveTest()
    {//Test quicksave on F5 !!! Use new input system 
        
        Debug.Log("F5 pressed, quicksaving...");
        CollectData();
        SaveManager.Instance.QuickSave();
       
        
    }


    private void OnApplicationQuit() {
        _isApplicationQuiting = true;
 
    }
    private void OnDestroy() {
        if (! _isApplicationQuiting) {  
            // Oyun bitmiyor
            if(GameManager.Instance.CurrentGameState != GameState.Gameover)
            {
                
                CollectData();
                SaveManager.Instance.QuickSave();
            }
                
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
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
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
        }
        //find all ISavable in the scene
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None)
            .OfType<ISavable>()
            .ToArray();
        //capture their state
        foreach (var item in savable) {
            data.DataDict[item.UniqueId] = item.CaptureState();
        }
        
    }
}
