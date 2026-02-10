-- Uitbreiden van notificatie tabel met extra velden
USE ResonanceDB;
GO

-- Voeg type kolom toe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('notificatie') AND name = 'type')
BEGIN
    ALTER TABLE notificatie ADD [type] NVARCHAR(50) NULL;
    PRINT 'Kolom "type" toegevoegd';
END
GO

-- Voeg isGelezen kolom toe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('notificatie') AND name = 'isGelezen')
BEGIN
    ALTER TABLE notificatie ADD [isGelezen] BIT NOT NULL DEFAULT 0;
    PRINT 'Kolom "isGelezen" toegevoegd';
END
GO

-- Voeg link kolom toe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('notificatie') AND name = 'link')
BEGIN
    ALTER TABLE notificatie ADD [link] NVARCHAR(500) NULL;
    PRINT 'Kolom "link" toegevoegd';
END
GO

-- Voeg icoon kolom toe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('notificatie') AND name = 'icoon')
BEGIN
    ALTER TABLE notificatie ADD [icoon] NVARCHAR(50) NULL;
    PRINT 'Kolom "icoon" toegevoegd';
END
GO

-- Update bestaande records
UPDATE notificatie SET [type] = 'Info' WHERE [type] IS NULL;
UPDATE notificatie SET [icoon] = 'bi-bell' WHERE [icoon] IS NULL;
GO

-- Maak indexes aan voor betere performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_notificatie_userID' AND object_id = OBJECT_ID('notificatie'))
BEGIN
    CREATE INDEX IX_notificatie_userID ON notificatie(userID);
    PRINT 'Index IX_notificatie_userID aangemaakt';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_notificatie_isGelezen' AND object_id = OBJECT_ID('notificatie'))
BEGIN
    CREATE INDEX IX_notificatie_isGelezen ON notificatie(isGelezen);
    PRINT 'Index IX_notificatie_isGelezen aangemaakt';
END

PRINT 'Notificatie tabel succesvol uitgebreid!';
GO

