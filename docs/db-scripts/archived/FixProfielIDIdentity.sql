-- Script om profielID kolom te veranderen naar IDENTITY
-- Database: ResonanceDB

USE ResonanceDB;
GO

-- Stap 1: Maak een nieuwe tabel met IDENTITY
CREATE TABLE profiel_new (
    profielID INT IDENTITY(1,1) PRIMARY KEY,
    userID INT NULL,
    voorkeur_genres NVARCHAR(255) NULL,
    stemmingstags NVARCHAR(255) NULL
);
GO

-- Stap 2: Kopieer bestaande data (indien aanwezig)
IF EXISTS (SELECT * FROM profiel)
BEGIN
    SET IDENTITY_INSERT profiel_new ON;
    
    INSERT INTO profiel_new (profielID, userID, voorkeur_genres, stemmingstags)
    SELECT profielID, userID, voorkeur_genres, stemmingstags FROM profiel;
    
    SET IDENTITY_INSERT profiel_new OFF;
END
GO

-- Stap 3: Verwijder oude tabel
DROP TABLE profiel;
GO

-- Stap 4: Hernoem nieuwe tabel naar profiel
EXEC sp_rename 'profiel_new', 'profiel';
GO

-- Verificatie
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMNPROPERTY(OBJECT_ID('profiel'), COLUMN_NAME, 'IsIdentity') AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'profiel';
GO

PRINT 'Klaar! profielID is nu een IDENTITY kolom.';

