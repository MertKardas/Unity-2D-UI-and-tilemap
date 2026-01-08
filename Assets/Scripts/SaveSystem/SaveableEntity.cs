using UnityEngine;

public class SaveableEntity : MonoBehaviour {
    [SerializeField] string uniqueId;
    public string UniqueId => uniqueId;
#region UNITY_EDITOR
    private void OnValidate() {
        if (string.IsNullOrEmpty(UniqueId)) {
            uniqueId = System.Guid.NewGuid().ToString();
        }
    }
#endregion
}
