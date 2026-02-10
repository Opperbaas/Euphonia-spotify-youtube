-- Fix script voor notificatie.notificatieID IDENTITY property
USE ResonanceDB;
GO

-- Verwijder oude backup als die bestaat
IF OBJECT_ID('notificatie_new', 'U') IS NOT NULL
    DROP TABLE notificatie_new;
GO

-- Stap 1: Drop foreign key constraints die naar notificatie verwijzen
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
               QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('notificatie');
EXEC sp_executesql @sql;
GO

-- Stap 2: Create new table with IDENTITY
CREATE TABLE notificatie_new (
    notificatieID INT IDENTITY(1,1) PRIMARY KEY,
    userID INT,
    tekst NVARCHAR(255),
    datum_tijd DATETIME
);
GO

-- Stap 3: Copy data (als er data is)
IF EXISTS (SELECT 1 FROM notificatie)
BEGIN
    SET IDENTITY_INSERT notificatie_new ON;
    
    INSERT INTO notificatie_new (notificatieID, userID, tekst, datum_tijd)
    SELECT notificatieID, userID, tekst, datum_tijd
    FROM notificatie;
    
    SET IDENTITY_INSERT notificatie_new OFF;
END
GO

-- Stap 4: Drop old table
DROP TABLE notificatie;
GO

-- Stap 5: Rename new table
EXEC sp_rename 'notificatie_new', 'notificatie';
GO

PRINT 'Klaar! notificatieID is nu een IDENTITY kolom.';

-- Verificatie
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMNPROPERTY(OBJECT_ID('notificatie'), COLUMN_NAME, 'IsIdentity') AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'notificatie';
GO

