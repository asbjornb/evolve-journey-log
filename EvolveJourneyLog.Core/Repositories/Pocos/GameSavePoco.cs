using PetaPoco;

namespace EvolveJourneyLog.Core.Repositories.Pocos;

#nullable disable
[TableName("gamedata.GameSave")]
[PrimaryKey("SaveID")]
internal class GameSavePoco
{
    public int SaveID { get; set; }
    public Guid PlayerId { get; set; }
    public DateTime WriteTime { get; set; }
    public string RawSaveData { get; set; }
    [ResultColumn]
    public string SaveHash { get; set; }

    public GameSavePoco(Guid playerId, string rawSaveData)
    {
        PlayerId = playerId;
        RawSaveData = rawSaveData;
    }

    private GameSavePoco()
    {
    }
}
