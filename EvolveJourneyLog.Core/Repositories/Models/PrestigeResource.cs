using Newtonsoft.Json;

namespace EvolveJourneyLog.Core.Repositories.Models;

public class PrestigeResource
{
    public int AICore { get; set; }
    public int AntiPlasmid { get; set; }
    public int Artifact { get; set; }
    [JsonProperty("Blood_Stone.Count")]
    public int BloodStone { get; set; }
    [JsonProperty("Dark")]
    public int DarkEnergy { get; set; }
    [JsonProperty("Harmony")]
    public int HarmonyCrystal { get; set; }
    public int Phage { get; set; }
    public int Plasmid { get; set; }
}
