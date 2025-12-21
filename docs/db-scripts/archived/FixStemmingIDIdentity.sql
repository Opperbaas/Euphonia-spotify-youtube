-- Fix script voor stemming.stemmingID IDENTITY property
USE EuphoniaDB;
GO

-- Verwijder oude backup als die bestaat
IF OBJECT_ID('stemming_new', 'U') IS NOT NULL
    DROP TABLE stemming_new;
GO

-- Stap 1: Drop foreign key constraints die naar stemming verwijzen
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
               QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('stemming');
EXEC sp_executesql @sql;
GO

-- Stap 2: Create new table with IDENTITY
CREATE TABLE stemming_new (
    stemmingID INT IDENTITY(1,1) PRIMARY KEY,
    userID INT,
    typeID INT,
    datum_tijd DATETIME,
    beschrijving NVARCHAR(255)
);
GO

-- Stap 3: Copy data (als er data is)
IF EXISTS (SELECT 1 FROM stemming)
BEGIN
    SET IDENTITY_INSERT stemming_new ON;
    
    INSERT INTO stemming_new (stemmingID, userID, typeID, datum_tijd, beschrijving)
    SELECT stemmingID, userID, typeID, datum_tijd, beschrijving
    FROM stemming;
    
    SET IDENTITY_INSERT stemming_new OFF;
END
GO

-- Stap 4: Drop old table
DROP TABLE stemming;
GO

-- Stap 5: Rename new table
EXEC sp_rename 'stemming_new', 'stemming';
GO

PRINT 'Klaar! stemmingID is nu een IDENTITY kolom.';

-- Verificatie
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMNPROPERTY(OBJECT_ID('stemming'), COLUMN_NAME, 'IsIdentity') AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'stemming';
GO
