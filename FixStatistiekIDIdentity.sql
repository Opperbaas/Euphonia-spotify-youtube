-- Fix script voor statistiek.statistiekID IDENTITY property
USE EuphoniaDB;
GO

-- Verwijder oude backup als die bestaat
IF OBJECT_ID('statistiek_new', 'U') IS NOT NULL
    DROP TABLE statistiek_new;
GO

-- Stap 1: Drop foreign key constraints die naar statistiek verwijzen
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
               QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('statistiek');
EXEC sp_executesql @sql;
GO

-- Stap 2: Create new table with IDENTITY
CREATE TABLE statistiek_new (
    statistiekID INT IDENTITY(1,1) PRIMARY KEY,
    userID INT,
    trend_type NVARCHAR(255),
    resultaat NVARCHAR(255)
);
GO

-- Stap 3: Copy data (als er data is)
IF EXISTS (SELECT 1 FROM statistiek)
BEGIN
    SET IDENTITY_INSERT statistiek_new ON;
    
    INSERT INTO statistiek_new (statistiekID, userID, trend_type, resultaat)
    SELECT statistiekID, userID, trend_type, resultaat
    FROM statistiek;
    
    SET IDENTITY_INSERT statistiek_new OFF;
END
GO

-- Stap 4: Drop old table
DROP TABLE statistiek;
GO

-- Stap 5: Rename new table
EXEC sp_rename 'statistiek_new', 'statistiek';
GO

PRINT 'Klaar! statistiekID is nu een IDENTITY kolom.';

-- Verificatie
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMNPROPERTY(OBJECT_ID('statistiek'), COLUMN_NAME, 'IsIdentity') AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'statistiek';
GO
