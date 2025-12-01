-- Script om muziekID kolom te veranderen naar IDENTITY
-- Voer dit uit in SQL Server Management Studio of Azure Data Studio
-- Database: EuphoniaDB

USE EuphoniaDB;
GO

-- Verwijder oude backup als die bestaat
IF OBJECT_ID('muziek_new', 'U') IS NOT NULL
    DROP TABLE muziek_new;
GO

-- Stap 1: Drop foreign key constraints die naar muziek verwijzen
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
               QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('muziek');

PRINT 'Dropping foreign keys:';
PRINT @sql;
EXEC sp_executesql @sql;
GO

-- Stap 2: Maak een nieuwe tabel met IDENTITY
CREATE TABLE muziek_new (
    muziekID INT IDENTITY(1,1) PRIMARY KEY,
    titel NVARCHAR(255) NULL,
    artiest NVARCHAR(255) NULL,
    bron NVARCHAR(255) NULL
);
GO

-- Stap 3: Kopieer bestaande data (indien aanwezig)
IF EXISTS (SELECT * FROM muziek)
BEGIN
    SET IDENTITY_INSERT muziek_new ON;
    
    INSERT INTO muziek_new (muziekID, titel, artiest, bron)
    SELECT muziekID, titel, artiest, bron FROM muziek;
    
    SET IDENTITY_INSERT muziek_new OFF;
END
GO

-- Stap 4: Verwijder oude tabel
DROP TABLE muziek;
GO

-- Stap 5: Hernoem nieuwe tabel naar muziek
EXEC sp_rename 'muziek_new', 'muziek';
GO

PRINT 'Klaar! muziekID is nu een IDENTITY kolom.';

-- Verificatie
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMNPROPERTY(OBJECT_ID('muziek'), COLUMN_NAME, 'IsIdentity') AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'muziek';
GO
