using UnityEngine;
using System.Linq;
public class SceneSaveLoader : MonoBehaviour
{
    private void Awake() {
        DistributeGameData();
        
    }
    private void Start() {
        InvokeRepeating(nameof(CollectData), 10f, 10f);
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
        var data = SaveManager.Instance.CurrentGameSaveData;
        
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISavable>()
            .ToArray();
        foreach (var item in savable) {
            data.DataDict[item.UniqueId] = item.CaptureState();
        }
        var saveManager = SaveManager.Instance;
        saveManager.CurrentGameSaveData = data;
        saveManager.QuickSave(data);
    }
}
