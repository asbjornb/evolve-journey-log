using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.Models;

namespace EvolveJourneyLog.Core.Services;

public class GameSaveService
{
    private readonly GameSaveRepository _gameSaveRepository;

    public GameSaveService(GameSaveRepository gameSaveRepository)
    {
        _gameSaveRepository = gameSaveRepository;
    }

    public async Task<SaveResult> HandleUserUploadAsync(Guid playerId, string rawSaveData)
    {
        var result = await _gameSaveRepository.SaveAsync(playerId, rawSaveData);

        if (result == SaveResult.Success)
        {
            // Extract data, call other services, etc.
        }

        return result;
    }

    public async Task<IEnumerable<SaveResult>> HandleUserUploadsAsync(Guid playerId, IEnumerable<string> rawSaveDatas)
    {
        var results = new List<SaveResult>();

        foreach (var rawData in rawSaveDatas)
        {
            var result = await HandleUserUploadAsync(playerId, rawData);
            results.Add(result);
        }

        return results;
    }
}
