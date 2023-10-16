using EvolveJourneyLog.Core.JsonExtraction;
using EvolveJourneyLog.Core.JsonExtraction.JsonExtractors;
using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.Models;

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

    public async Task<ISaveResponse> HandleUserUploadAsync(Guid playerId, string rawSaveData)
    {
        var result = await _gameSaveRepository.SaveAsync(playerId, rawSaveData);

        if (result is SaveSuccess success)
        {
            var decoder = new LZStringDecoder();
            var decodedSaveData = decoder.Decode(rawSaveData);
            var deserializedData = JsonFlattener.DeSerialize<PrestigeResource>(decodedSaveData) ?? throw new InvalidOperationException("Failed to deserialize save data.");

            await _prestigeResourceRepository.SaveAsync(success.SaveId, deserializedData.AICore, deserializedData.AntiPlasmid, deserializedData.Artifact, deserializedData.BloodStone, deserializedData.DarkEnergy, deserializedData.HarmonyCrystal, deserializedData.Phage, deserializedData.Plasmid);
        }

        return result;
    }
}
