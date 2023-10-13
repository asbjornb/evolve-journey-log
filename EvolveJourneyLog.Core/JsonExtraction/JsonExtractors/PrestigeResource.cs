namespace EvolveJourneyLog.Core.JsonExtraction.JsonExtractors;

public class PrestigeResource
{
    [Path("prestige/AICore/count", IsRequired = true)]
    public int AICore { get; set; }

    [Path("prestige/AntiPlasmid/count", IsRequired = true)]
    public int AntiPlasmid { get; set; }

    [Path("prestige/Artifact/count", IsRequired = true)]
    public int Artifact { get; set; }

    [Path("prestige/Blood_Stone/count", IsRequired = true)]
    public int BloodStone { get; set; }

    [Path("prestige/Dark/count", IsRequired = true)]
    public double DarkEnergy { get; set; }

    [Path("prestige/Harmony/count", IsRequired = true)]
    public int HarmonyCrystal { get; set; }

    [Path("prestige/Phage/count", IsRequired = true)]
    public int Phage { get; set; }

    [Path("prestige/Plasmid/count", IsRequired = true)]
    public int Plasmid { get; set; }
}
