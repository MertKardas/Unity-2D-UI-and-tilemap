using System;

[Serializable]
public class DynamicObjectSaveData
{
    public string ItemId;
    public string UniqueId;
    public float[] Position;
    public object ComponentState;
}
