-- Seed data voor stemmingType tabel
USE ResonanceDB;
GO

-- Check of er al data is
IF NOT EXISTS (SELECT 1 FROM stemmingType)
BEGIN
    SET IDENTITY_INSERT stemmingType ON;

    INSERT INTO stemmingType (typeID, naam, beschrijving) VALUES
    (1, 'Blij', 'Een gevoel van vreugde en geluk'),
    (2, 'Verdrietig', 'Een gevoel van droefheid of verdriet'),
    (3, 'Energiek', 'Vol energie en enthousiasme'),
    (4, 'Ontspannen', 'Rustig en vredig'),
    (5, 'Angstig', 'Bezorgd of nerveus'),
    (6, 'Boos', 'Geïrriteerd of gefrustreerd'),
    (7, 'Hoopvol', 'Positief over de toekomst'),
    (8, 'Eenzaam', 'Gevoel van isolatie'),
    (9, 'Liefdevol', 'Vol liefde en genegenheid'),
    (10, 'Geconcentreerd', 'Gefocust en alert');

    SET IDENTITY_INSERT stemmingType OFF;

    PRINT 'Stemming types succesvol toegevoegd!';
END
ELSE
BEGIN
    PRINT 'Stemming types bestaan al.';
END
GO

-- Verificatie
SELECT * FROM stemmingType ORDER BY typeID;
GO

