using PetaPoco;

namespace EvolveJourneyLog.Core.Repositories.Pocos;

#nullable disable
[TableName("gamedata.Player")]
[PrimaryKey("PlayerID")]
internal class PlayerPoco
{
    public Guid PlayerID { get; set; }
    public Guid ViewToken { get; set; }
    public string PlayerName { get; set; }

    public PlayerPoco(string playerName)
    {
        PlayerName = playerName;
    }

    private PlayerPoco()
    {
    }
}
