-- Notificatie tabel aanmaken
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'notificatie')
BEGIN
    CREATE TABLE [dbo].[notificatie] (
        [notificatieID] INT IDENTITY(1,1) NOT NULL,
        [userID] INT NOT NULL,
        [titel] NVARCHAR(255) NOT NULL,
        [bericht] NVARCHAR(MAX) NOT NULL,
        [type] NVARCHAR(50) NOT NULL, -- 'Info', 'Success', 'Warning', 'Error'
        [isGelezen] BIT NOT NULL DEFAULT 0,
        [aanmaakDatum] DATETIME NOT NULL DEFAULT GETDATE(),
        [gelezenOp] DATETIME NULL,
        [link] NVARCHAR(500) NULL, -- Optionele link naar gerelateerde pagina
        [icoon] NVARCHAR(50) NULL, -- Bootstrap icon class
        CONSTRAINT [PK_notificatie] PRIMARY KEY CLUSTERED ([notificatieID] ASC),
        CONSTRAINT [FK_notificatie_user] FOREIGN KEY ([userID]) 
            REFERENCES [dbo].[table1_user] ([userID]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_notificatie_userID] ON [dbo].[notificatie] ([userID]);
    CREATE INDEX [IX_notificatie_isGelezen] ON [dbo].[notificatie] ([isGelezen]);
    CREATE INDEX [IX_notificatie_aanmaakDatum] ON [dbo].[notificatie] ([aanmaakDatum] DESC);
    
    PRINT 'Notificatie tabel succesvol aangemaakt';
END
ELSE
BEGIN
    PRINT 'Notificatie tabel bestaat al';
END
GO

-- Seed enkele test notificaties (optioneel)
-- INSERT INTO notificatie (userID, titel, bericht, type, icoon)
-- VALUES 
-- (1, 'Welkom bij Euphonia!', 'Begin met het tracken van je stemmingen en muziek', 'Info', 'bi-info-circle'),
-- (1, 'Eerste stemming toegevoegd', 'Je hebt je eerste stemming succesvol toegevoegd!', 'Success', 'bi-check-circle');
