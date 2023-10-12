CREATE TABLE gamedata.PrestigeResources
(
    SaveId INT NOT NULL,
    AICore INT NOT NULL,
    AntiPlasmid INT NOT NULL,
    Artifact INT NOT NULL,
    BloodStone INT NOT NULL,
    DarkEnergy INT NOT NULL,
    HarmonyCrystal INT NOT NULL,
    Phage INT NOT NULL,
    Plasmid INT NOT NULL,

    CONSTRAINT FK_PrestigeResources_SaveId FOREIGN KEY (SaveId) REFERENCES gamedata.GameSave(SaveId)
);
GO
