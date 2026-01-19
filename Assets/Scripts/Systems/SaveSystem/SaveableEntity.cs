using UnityEngine;

public class SaveableEntity : MonoBehaviour {
    [SerializeField] string uniqueId;
    public string UniqueId => uniqueId;
    public void SetUniqueId(string id) {
        uniqueId = id;
    }
    public void GenerateNewId() {
        uniqueId = System.Guid.NewGuid().ToString();
    }
#region UNITY_EDITOR
    private void OnValidate() {
        if (string.IsNullOrEmpty(UniqueId)) {
            uniqueId = System.Guid.NewGuid().ToString();
        }
    }
#endregion
}
