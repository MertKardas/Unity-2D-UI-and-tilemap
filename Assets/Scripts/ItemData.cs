using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor;
using NaughtyAttributes; 
[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class ItemData: ScriptableObject {

    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string ItemName { get; private set;  }
    [field: SerializeField, TextArea(3, 10)] public string Description { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }
    [field: SerializeField, ShowAssetPreview] public GameObject Prefab { get; private set; }
    [field: SerializeField] public bool IsStackable { get; private set; }
    [field: SerializeField, ShowIf("IsStackable")] public int StackSize { get; private set; }

#if UNITY_EDITOR   
    private void OnValidate() {
        if(string.IsNullOrEmpty(ID)) {
            ID = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
        }
        if (!IsStackable) {
            StackSize = 1;
        }
        if (StackSize < 1) {
            StackSize = 1;
        }
    }
    #endif
}