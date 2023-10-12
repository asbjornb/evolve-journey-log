using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.Models;
using Newtonsoft.Json;

namespace EvolveJourneyLog.Core.Services;

public class GameSaveService
{
    private readonly GameSaveRepository _gameSaveRepository;
    private readonly PrestigeResourceRepository _prestigeResourceRepository;

    public GameSaveService(GameSaveRepository gameSaveRepository, PrestigeResourceRepository prestigeResourceRepository)
    {
        _gameSaveRepository = gameSaveRepository;
        _prestigeResourceRepository = prestigeResourceRepository;
    }

    public async Task<SaveResult> HandleUserUploadAsync(Guid playerId, string rawSaveData)
    {
        var result = await _gameSaveRepository.SaveAsync(playerId, rawSaveData);

        if (result is SaveSuccess success)
        {
            var decoder = new LZStringDecoder();
            var decodedSaveData = decoder.Decode(rawSaveData);
            var deserializedData = JsonConvert.DeserializeObject<PrestigeResource>(decodedSaveData) ?? throw new InvalidOperationException("Failed to deserialize save data.");

            await _prestigeResourceRepository.SaveAsync(success.SaveId, deserializedData.AICore, deserializedData.AntiPlasmid, deserializedData.Artifact, deserializedData.BloodStone, deserializedData.DarkEnergy, deserializedData.HarmonyCrystal, deserializedData.Phage, deserializedData.Plasmid);
        }

        return result is SaveFailure failure ? failure.Result : SaveResult.Success;
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
