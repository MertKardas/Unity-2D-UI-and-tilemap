using UnityEngine;

public class GameSaveData {
    public string sceneName;
    public PlayerRunTimeData playerData;

    public GameSaveData() {
        playerData = new PlayerRunTimeData();
    }
}
