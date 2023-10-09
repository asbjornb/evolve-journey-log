﻿CREATE TABLE gamedata.GameSave
(
    SaveID INT NOT NULL CONSTRAINT PK_GameSave_SaveID PRIMARY KEY IDENTITY(1,1),
    PlayerID INT NOT NULL, 
    WriteTime DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    RawSaveData VARCHAR(MAX) NOT NULL,

    CONSTRAINT FK_PlayerID FOREIGN KEY (PlayerID) REFERENCES gamedata.Player(PlayerID)
);