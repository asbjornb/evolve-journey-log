CREATE TABLE gamedata.GameSave
(
    SaveId INT NOT NULL CONSTRAINT PK_GameSave_SaveId PRIMARY KEY IDENTITY(1,1),
    PlayerId UNIQUEIDENTIFIER NOT NULL, 
    WriteTime DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    RawSaveData VARCHAR(MAX) NOT NULL,
    SaveHash AS (CONVERT(CHAR(64), HASHBYTES('SHA2_256', LEFT(RawSaveData, 8000)))) PERSISTED,
    RunId INT NULL, --Fill out after first saving raw data

    CONSTRAINT FK_GameSave_PlayerId FOREIGN KEY (PlayerId) REFERENCES gamedata.Player(PlayerId),
    CONSTRAINT FK_GameSave_RunId FOREIGN KEY (RunId) REFERENCES gamedata.Run(RunId)
);
GO

CREATE UNIQUE INDEX NCIX_SaveHash ON gamedata.GameSave(SaveHash);
GO
