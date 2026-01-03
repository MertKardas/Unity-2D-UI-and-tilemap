using System;

[Serializable]
public class MetaData {
    public string id;      // Dosya adý için Unique ID (GUID)
    public string displayName;  // Kullanýcýnýn göreceði isim (Örn: "Karanlýk Orman")
    public DateTime saveTime;
    public string gameVersion;

    public MetaData(string uniqueId, string visibleName) {
        id = uniqueId;
        displayName = visibleName;
        saveTime = DateTime.Now;
        gameVersion = Application.version;
    }
}
