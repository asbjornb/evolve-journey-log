using PetaPoco;

namespace EvolveJourneyLog.Core.Repositories.Pocos;

#nullable disable
[TableName("gamedata.Player")]
[PrimaryKey("PlayerId")]
internal class PlayerPoco
{
    public Guid PlayerId { get; set; }
    [Ignore]
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
