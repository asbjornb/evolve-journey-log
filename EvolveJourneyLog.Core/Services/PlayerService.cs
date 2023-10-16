using EvolveJourneyLog.Core.Repositories;

namespace EvolveJourneyLog.Core.Services;

public class PlayerService
{
    private readonly PlayerRepository _playerRepository;

    public PlayerService(PlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<Guid> RegisterPlayerAsync(string playerName)
    {
        return await _playerRepository.SaveAsync(playerName);
    }
}
