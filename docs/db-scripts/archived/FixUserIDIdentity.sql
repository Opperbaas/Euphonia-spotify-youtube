-- Script om de 'user' kolom in table1_user om te zetten naar IDENTITY
-- Voer dit uit in Azure Data Studio of SQL Server Management Studio

USE ResonanceDB;
GO

-- Stap 1: Maak een nieuwe tabel met IDENTITY op de 'user' kolom
CREATE TABLE table1_user_new (
    [user] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    userID INT NOT NULL,
    [e-mail] VARCHAR(50) NOT NULL,
    wachtwoord VARCHAR(50) NOT NULL,
    rol NUMERIC NULL
);
GO

-- Stap 2: Kopieer alle data van oude naar nieuwe tabel
SET IDENTITY_INSERT table1_user_new ON;
GO

INSERT INTO table1_user_new ([user], userID, [e-mail], wachtwoord, rol)
SELECT [user], userID, [e-mail], wachtwoord, rol
FROM table1_user;
GO

SET IDENTITY_INSERT table1_user_new OFF;
GO

-- Stap 3: Drop de oude tabel
DROP TABLE table1_user;
GO

-- Stap 4: Hernoem de nieuwe tabel
EXEC sp_rename 'table1_user_new', 'table1_user';
GO

-- Stap 5: Voeg unique constraint toe op email
ALTER TABLE table1_user 
ADD CONSTRAINT UQ_table1_user_email UNIQUE ([e-mail]);
GO

PRINT 'Klaar! De user kolom is nu IDENTITY en begint bij het hoogste bestaande nummer + 1';
PRINT 'Nieuwe gebruikers krijgen automatisch een uniek user nummer.';

