-- Fix script voor stemmingType.typeID IDENTITY property
USE EuphoniaDB;
GO

-- Verwijder oude backup als die bestaat
IF OBJECT_ID('stemmingType_new', 'U') IS NOT NULL
    DROP TABLE stemmingType_new;
GO

-- Stap 1: Drop foreign key constraints die naar stemmingType verwijzen
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
               QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('stemmingType');
EXEC sp_executesql @sql;
GO

-- Stap 2: Create new table with IDENTITY
CREATE TABLE stemmingType_new (
    typeID INT IDENTITY(1,1) PRIMARY KEY,
    naam NVARCHAR(100),
    beschrijving NVARCHAR(500)
);
GO

-- Stap 3: Copy data (als er data is)
IF EXISTS (SELECT 1 FROM stemmingType)
BEGIN
    SET IDENTITY_INSERT stemmingType_new ON;
    
    INSERT INTO stemmingType_new (typeID, naam, beschrijving)
    SELECT typeID, naam, beschrijving
    FROM stemmingType;
    
    SET IDENTITY_INSERT stemmingType_new OFF;
END
GO

-- Stap 4: Drop old table
DROP TABLE stemmingType;
GO

-- Stap 5: Rename new table
EXEC sp_rename 'stemmingType_new', 'stemmingType';
GO

PRINT 'Klaar! typeID is nu een IDENTITY kolom.';

-- Verificatie
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMNPROPERTY(OBJECT_ID('stemmingType'), COLUMN_NAME, 'IsIdentity') AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'stemmingType';
GO
