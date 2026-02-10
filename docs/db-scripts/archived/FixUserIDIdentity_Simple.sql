-- Eenvoudiger script om 'user' kolom IDENTITY te maken
-- Gebruik dit als het andere script niet werkte

USE ResonanceDB;

-- Verwijder oude backup als die bestaat
IF OBJECT_ID('table1_user_backup', 'U') IS NOT NULL
    DROP TABLE table1_user_backup;

-- Check huidige data
SELECT * FROM table1_user;

-- Stap 1: Zoek en drop alle foreign key constraints die naar table1_user verwijzen
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
               QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('table1_user');

PRINT 'Dropping foreign keys:';
PRINT @sql;
EXEC sp_executesql @sql;

-- Stap 2: Maak backup tabel
SELECT * INTO table1_user_backup FROM table1_user;

-- Stap 3: Drop oude tabel
DROP TABLE table1_user;

-- Stap 4: Maak nieuwe tabel met IDENTITY
CREATE TABLE table1_user (
    [user] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    userID INT NOT NULL,
    [e-mail] VARCHAR(50) NOT NULL UNIQUE,
    wachtwoord VARCHAR(50) NOT NULL,
    rol NUMERIC NULL
);

-- Stap 5: Kopieer data terug (als er data was)
IF EXISTS (SELECT 1 FROM table1_user_backup)
BEGIN
    SET IDENTITY_INSERT table1_user ON;
    
    INSERT INTO table1_user ([user], userID, [e-mail], wachtwoord, rol)
    SELECT [user], userID, [e-mail], wachtwoord, rol
    FROM table1_user_backup;
    
    SET IDENTITY_INSERT table1_user OFF;
END

-- Stap 6: Verwijder backup (optioneel - laat staan voor veiligheid)
-- DROP TABLE table1_user_backup;

PRINT 'Klaar! De user kolom is nu IDENTITY.';
PRINT 'BELANGRIJK: Start de applicatie opnieuw op!';

-- Verifieer
SELECT 
    COLUMN_NAME, 
    DATA_TYPE,
    COLUMNPROPERTY(OBJECT_ID('dbo.table1_user'), COLUMN_NAME, 'IsIdentity') as IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'table1_user';

