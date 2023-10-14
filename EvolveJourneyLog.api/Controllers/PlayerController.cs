using EvolveJourneyLog.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvolveJourneyLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly PlayerService _playerService;
    private readonly GameSaveService _gameSaveService;

    public PlayerController(PlayerService playerService, GameSaveService gameSaveService)
    {
        _playerService = playerService;
        _gameSaveService = gameSaveService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string? playerName)
    {
        var playerId = await _playerService.RegisterPlayerAsync(playerName);
        return Ok(playerId);
    }

    [HttpPost("{playerId}/uploadSave")]
    public async Task<IActionResult> UploadSave(Guid playerId, [FromBody] string rawSaveData)
    {
        var result = await _gameSaveService.HandleUserUploadAsync(playerId, rawSaveData);
        return Ok(result);
    }
}
