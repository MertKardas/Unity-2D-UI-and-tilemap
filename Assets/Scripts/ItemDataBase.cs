using UnityEngine;
using MyUtility;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//SerializableDictionary  reference
using AYellowpaper.SerializedCollections; 
public class ItemDataBase : Singleton<ItemDataBase>
{

    [SerializeField]private SerializedDictionary<string, ItemData> _itemDataDictionary;

    protected override void Awake() {
        base.Awake();
        //Load Scriptable objects labed "Item".
        LoadItemData("Item"); 

    }
    private void LoadItemData(string label) {
        var handle = Addressables.LoadAssetsAsync<ItemData>(label,null ); 
        handle.WaitForCompletion();
        if(handle.Status == AsyncOperationStatus.Succeeded) {
            _itemDataDictionary = new SerializedDictionary<string, ItemData>();
            foreach (var itemData in handle.Result) {
                _itemDataDictionary.Add(itemData.ID, itemData);
            }
        }
        
    }
    public ItemData GetItem(string id) {
        _itemDataDictionary.TryGetValue(id, out var item);
        return item;
    }

}
    

    

    