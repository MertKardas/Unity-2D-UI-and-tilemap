using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface ISaveService {
    Task<SaveResult> SaveGameAsync(string uniqueId, string displayName, GameSaveData data, CancellationToken ct);
    Task<(GameSaveData data, SaveResult result)> LoadGameAsync(string uniqueId, CancellationToken ct);
    Task<List<MetaData>> GetAllSavesAsync(CancellationToken ct);
    Task<SaveResult> DeleteSaveAsync(string uniqueId, CancellationToken ct);
}
