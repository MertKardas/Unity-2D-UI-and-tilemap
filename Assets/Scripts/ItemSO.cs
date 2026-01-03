using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class ItemSO: ScriptableObject {
    public string id; 
    public string itemName;
    public string description;
    public Sprite icon;
    public GameObject prefab;
}