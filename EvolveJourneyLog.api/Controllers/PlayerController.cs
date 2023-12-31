﻿using EvolveJourneyLog.Api.Controllers.Requests;
using EvolveJourneyLog.Core.Repositories.Models;
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
    public async Task<IActionResult> Register([FromBody] PlayerRegistrationRequest request)
    {
        var playerName = request.PlayerName;
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return BadRequest("Player name cannot be empty.");
        }
        try
        {
            var playerId = await _playerService.RegisterPlayerAsync(playerName);
            return Ok(playerId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex); //TODO: Use proper logger
            return StatusCode(500, "An error occurred while registering the player.");
        }
    }

    [HttpPost("{playerId}/uploadSave")]
    public async Task<IActionResult> UploadSave(Guid playerId, [FromBody] UploadSaveRequest request)
    {
        var rawSaveData = request.RawSave;
        if (string.IsNullOrWhiteSpace(rawSaveData))
        {
            return BadRequest("Save data cannot be empty.");
        }
        try
        {
            var result = await _gameSaveService.HandleUserUploadAsync(playerId, rawSaveData);
            if(result is SaveFailure failure)
            {
                return BadRequest(failure.Result.ToString());
            }
            return Ok(new { message = "Success" });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex); //TODO: Use proper logger
            return StatusCode(500, "An error occurred while writing save data.");
        }
    }
}
