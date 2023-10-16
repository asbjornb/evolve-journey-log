using System.Data.SqlClient;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Models;
using EvolveJourneyLog.Core.Repositories.Pocos;

namespace EvolveJourneyLog.Core.Repositories;

public class GameSaveRepository
{
    private readonly IDatabaseFactory _databaseFactory;

    public GameSaveRepository(IDatabaseFactory dbFactory)
    {
        _databaseFactory = dbFactory;
    }

    public async Task<ISaveResponse> SaveAsync(Guid playerId, string rawSaveData)
    {
        var gameSave = new GameSavePoco(playerId, rawSaveData);
        using var database = _databaseFactory.GetDatabase();

        try
        {
            await database.InsertAsync(gameSave);
            return new SaveSuccess(gameSave.SaveId); //Do I need the Id back?
        }
        catch (SqlException ex)
        {
            if (IsDuplicateSaveError(ex))
            {
                return new SaveFailure(SaveResult.DuplicateSave);
            }
            if (IsForeignKeyPlayerViolationError(ex))
            {
                return new SaveFailure(SaveResult.PlayerNotFound);
            }
            throw;
        }
    }

    private static bool IsForeignKeyPlayerViolationError(SqlException ex)
    {
        return ex.Message.Contains("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_GameSave_PlayerId\"");
    }

    private static bool IsDuplicateSaveError(SqlException ex)
    {
        return ex.Message.Contains("Cannot insert duplicate key row in object 'gamedata.GameSave' with unique index 'NCIX_SaveHash'");
    }
}
