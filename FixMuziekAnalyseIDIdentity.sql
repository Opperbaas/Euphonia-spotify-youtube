-- Fix script voor muziekAnalyse.analyseID IDENTITY property
USE EuphoniaDB;
GO

-- Verwijder oude backup als die bestaat
IF OBJECT_ID('muziekAnalyse_new', 'U') IS NOT NULL
    DROP TABLE muziekAnalyse_new;
GO

-- Stap 1: Drop foreign key constraints die naar muziekAnalyse verwijzen
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
               QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('muziekAnalyse');
EXEC sp_executesql @sql;
GO

-- Stap 2: Create new table with IDENTITY
CREATE TABLE muziekAnalyse_new (
    analyseID INT IDENTITY(1,1) PRIMARY KEY,
    muziekID INT,
    stemmingType NVARCHAR(255),
    energieLevel NVARCHAR(255),
    valence INT,
    tempo NVARCHAR(255),
    bron NVARCHAR(255)
);
GO

-- Stap 3: Copy data (als er data is)
IF EXISTS (SELECT 1 FROM muziekAnalyse)
BEGIN
    SET IDENTITY_INSERT muziekAnalyse_new ON;
    
    INSERT INTO muziekAnalyse_new (analyseID, muziekID, stemmingType, energieLevel, valence, tempo, bron)
    SELECT analyseID, muziekID, stemmingType, energieLevel, valence, tempo, bron
    FROM muziekAnalyse;
    
    SET IDENTITY_INSERT muziekAnalyse_new OFF;
END
GO

-- Stap 4: Drop old table
DROP TABLE muziekAnalyse;
GO

-- Stap 5: Rename new table
EXEC sp_rename 'muziekAnalyse_new', 'muziekAnalyse';
GO

PRINT 'Klaar! analyseID is nu een IDENTITY kolom.';

-- Verificatie
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMNPROPERTY(OBJECT_ID('muziekAnalyse'), COLUMN_NAME, 'IsIdentity') AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'muziekAnalyse';
GO
