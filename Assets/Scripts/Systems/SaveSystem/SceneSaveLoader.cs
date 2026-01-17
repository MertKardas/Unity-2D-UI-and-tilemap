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
        //Test 
        Invoke(nameof(SaveTest), 5f);
        
    }
    
    
    public void SaveTest()
    {//Test quicksave on F5 !!! Use new input system 
        
        CollectData();
        SaveManager.Instance.QuickSave();
       Debug.Log("Quicksave performed by SceneSaveLoader for testing.");
        
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
                Debug.Log(SaveManager.Instance.CurrentGameSaveData.DataDict.Count 
                + " items collected for autosave."); 
                SaveManager.Instance.QuickSave();
            }
                
        }
    }

    private void DistributeGameData(Scene scene, LoadSceneMode mode) {
        if(scene.name == "MainMenu") 
            return;
        
        var data = SaveManager.Instance.CurrentGameSaveData;
        if(data == null) 
            return;
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
            SaveManager.Instance.CurrentGameSaveData = data;
        }
        data.SceneName = SceneManager.GetActiveScene().name;
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
