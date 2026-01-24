using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SceneSaveLoader
{
    public void DistributeGameData(GameSaveData saveData, string sceneName)
    {
        if (saveData == null)
            return;

        // Get data for this specific scene
        if (!saveData.SceneData.TryGetValue(sceneName, out var sceneData))
            return; // No saved data for this scene

        // 1. Spawn dynamic objects for this scene
        foreach (var id in sceneData.DynamicObjectIds ?? new List<string>())
        {
            if (!sceneData.SaveObjects.TryGetValue(id, out var raw)) continue;
            var data = raw as DynamicObjectSaveData;
            if (data == null) continue;

            var item = ItemDataBase.Instance.GetItem(data.ItemId);
            if (item?.Prefab == null) continue;

            var pos = new Vector3(data.Position[0], data.Position[1], data.Position[2]);
            var obj = Object.Instantiate(item.Prefab, pos, Quaternion.identity);

            obj.GetComponent<SaveableEntity>().SetUniqueId(data.UniqueId);
            obj.GetComponent<DropObject>().InitializeAsDynamic(item, 1);
        }

        // 2. Restore all states for this scene
        foreach (var savable in FindAllSavables())
        {
            if (sceneData.SaveObjects.TryGetValue(savable.UniqueId, out var state))
            {
                if (state is DynamicObjectSaveData dyn)
                    savable.RestoreState(dyn.ComponentState);
                else
                    savable.RestoreState(state);
            }
        }
    }

    public SceneSaveData CollectSceneData()
    {
        var sceneData = new SceneSaveData();

        foreach (var savable in FindAllSavables())
        {
            var drop = (savable as MonoBehaviour)?.GetComponent<DropObject>();

            if (drop != null && drop.IsDynamic)
            {
                sceneData.DynamicObjectIds.Add(savable.UniqueId);
                sceneData.SaveObjects[savable.UniqueId] = new DynamicObjectSaveData
                {
                    ItemId = drop.ItemId,
                    UniqueId = savable.UniqueId,
                    Position = new[] { drop.transform.position.x, drop.transform.position.y, drop.transform.position.z },
                    ComponentState = savable.CaptureState()
                };
            }
            else
            {
                sceneData.SaveObjects[savable.UniqueId] = savable.CaptureState();
            }
        }

        return sceneData;
    }

    private ISavable[] FindAllSavables()
    {
        return Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISavable>()
            .ToArray();
    }
}
