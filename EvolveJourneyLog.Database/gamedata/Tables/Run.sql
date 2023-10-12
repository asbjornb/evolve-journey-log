CREATE TABLE gamedata.Run
(
    RunId INT NOT NULL CONSTRAINT PK_Run PRIMARY KEY IDENTITY(1,1),
    PlayerId UNIQUEIDENTIFIER NOT NULL,
    RunNumber int NOT NULL,
    StartedOnDay int NOT NULL,
    EndedBeforeDay int NOT NULL,
    
    CONSTRAINT FK_Run_PlayerId FOREIGN KEY (PlayerId) REFERENCES gamedata.Player(PlayerId)
);
