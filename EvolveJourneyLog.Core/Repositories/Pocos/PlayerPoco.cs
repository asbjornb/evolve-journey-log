using PetaPoco;

namespace EvolveJourneyLog.Core.Repositories.Pocos;

#nullable disable
[TableName("Player")]
[PrimaryKey("PlayerID")]
internal class PlayerPoco
{
    public int PlayerID { get; set; }
    public Guid EditToken { get; set; }
    public Guid ViewToken { get; set; }
    public string PlayerName { get; set; }
}
