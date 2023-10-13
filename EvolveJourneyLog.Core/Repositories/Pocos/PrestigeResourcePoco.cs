using PetaPoco;

namespace EvolveJourneyLog.Core.Repositories.Pocos;

#nullable disable
[TableName("gamedata.PrestigeResources")]
[PrimaryKey("SaveId", AutoIncrement = false)] // Since SaveId is not auto-incremented here
internal class PrestigeResourcePoco
{
    public int SaveId { get; set; }
    public int AICore { get; set; }
    public int AntiPlasmid { get; set; }
    public int Artifact { get; set; }
    public int BloodStone { get; set; }
    public double DarkEnergy { get; set; }
    public int HarmonyCrystal { get; set; }
    public int Phage { get; set; }
    public int Plasmid { get; set; }

    public PrestigeResourcePoco(int saveId, int aiCore, int antiPlasmid, int artifact, int bloodStone, double darkEnergy, int harmonyCrystal, int phage, int plasmid)
    {
        SaveId = saveId;
        AICore = aiCore;
        AntiPlasmid = antiPlasmid;
        Artifact = artifact;
        BloodStone = bloodStone;
        DarkEnergy = darkEnergy;
        HarmonyCrystal = harmonyCrystal;
        Phage = phage;
        Plasmid = plasmid;
    }

    private PrestigeResourcePoco()
    {
    }
}
