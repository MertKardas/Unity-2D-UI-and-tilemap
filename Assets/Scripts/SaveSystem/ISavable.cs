using UnityEngine;

public interface ISavable {
    public string UniqueId { get; }
    object CaptureState();
    //restore to a previous state
    void RestoreState(object data);
}
