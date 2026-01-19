using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneSaveLoader : MonoBehaviour
{
    public static SceneSaveLoader Instance;
    bool _isApplicationQuiting;
    string currentSceneName;
    List<string> dynamicObjectIds = new();
    List<string> removedDynamicObjectIds = new();
    
    private void Awake()
    {
        //Scene singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += DistributeGameData;
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuiting = true;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= DistributeGameData;
        
        if (!_isApplicationQuiting)
        {
            if (GameManager.Instance.CurrentGameState != GameState.Gameover)
            {
                CollectData();
                Debug.Log(SaveManager.Instance.CurrentGameSaveData.DataDict.Count
                + " items collected for autosave.");
                SaveManager.Instance.QuickSave();
            }
        }
        
        // ✅ Instance temizliği
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void DistributeGameData(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
            return;

        var data = SaveManager.Instance.CurrentGameSaveData;
        if (data == null)
            return;
            
        // ✅ Scene adını güncelle
        currentSceneName = scene.name;
        
        if (data.DynamicObjectRecord.TryGetValue(currentSceneName, out var ids))
        {
            dynamicObjectIds = ids;
        }
        else
        {
            dynamicObjectIds = new List<string>();
        }
        
        // ✅ Önceki scene'den kalan removed listesini temizle
        removedDynamicObjectIds.Clear();
        
        InstanstiateDynamicObjectsForScene();
        
        //find all ISavable in the scene
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISavable>()
            .ToArray();
            
        //restore their state
        foreach (var item in savable)
        {
            if (data.DataDict.TryGetValue(item.UniqueId, out object state))
            {
                item.RestoreState(state);
            }
        }
    }
    
    public void CollectData()
    {
        Debug.Log("Collecting scene data for autosave...");

        var data = SaveManager.Instance.CurrentGameSaveData;
        if (data == null)
        {
            data = new GameSaveData();
            SaveManager.Instance.CurrentGameSaveData = data;
        }
        
        data.SceneName = currentSceneName;
        
        // ✅ Daha temiz null check
        if (!data.DynamicObjectRecord.ContainsKey(currentSceneName))
        {
            data.DynamicObjectRecord[currentSceneName] = new List<string>();
        }
        dynamicObjectIds = data.DynamicObjectRecord[currentSceneName];
        
        //find all ISavable in the scene
        var savable = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISavable>()
            .ToArray();
            
        //capture their state
        foreach (var item in savable)
        {
            data.DataDict[item.UniqueId] = item.CaptureState();
        }
        
        // ✅ Removed objeleri temizle
        foreach (var id in removedDynamicObjectIds)
        {
            if (dynamicObjectIds.Contains(id))
            {
                dynamicObjectIds.Remove(id);
            }
            
            // ✅ DataDict'ten de sil
            if (data.DataDict.ContainsKey(id))
            {
                data.DataDict.Remove(id);
            }
        }
        
        data.DynamicObjectRecord[currentSceneName] = dynamicObjectIds;
        
        // ✅ Liste temizle
        removedDynamicObjectIds.Clear();
    }
    
    private void InstanstiateDynamicObjectsForScene()
    {
        foreach (var id in dynamicObjectIds)
        {
            SaveManager.Instance.CurrentGameSaveData.DataDict.TryGetValue(id, out object state);
            if (state is DropObjectSaveData dynamicObjectData)
            {
                var itemID = dynamicObjectData.itemID;
                var itemData = ItemDataBase.Instance.GetItem(itemID);
                
                // ✅ Null check ÖNCE
                if (itemData == null || itemData.Prefab == null)
                {
                    Debug.LogWarning($"Item data with ID {itemID} not found in database.");
                    continue;
                }
                
                var dropPrefab = itemData.Prefab;
                var dropObject = Instantiate(dropPrefab);
            }
        }
    }

    public void RegisterDynamicObject(string uniqueId)
    {
        if (!dynamicObjectIds.Contains(uniqueId))
        {
            dynamicObjectIds.Add(uniqueId);
        }
        
        // ✅ Removed listesinden kaldır
        if (removedDynamicObjectIds.Contains(uniqueId))
        {
            removedDynamicObjectIds.Remove(uniqueId);
        }
    }
    
    public void UnregisterDynamicObject(string uniqueId)
    {
        if (dynamicObjectIds.Contains(uniqueId))
        {
            dynamicObjectIds.Remove(uniqueId);
            if (!removedDynamicObjectIds.Contains(uniqueId))
                removedDynamicObjectIds.Add(uniqueId);
        }   
    }
}