-- Fix PK column in stemmingMuziek table to have IDENTITY
USE EuphoniaDB;
GO

-- Controleer of de kolom al IDENTITY heeft
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns c
    INNER JOIN sys.objects o ON c.object_id = o.object_id
    WHERE o.name = 'stemmingMuziek' 
    AND c.name = 'PK' 
    AND c.is_identity = 1
)
BEGIN
    PRINT 'PK kolom heeft geen IDENTITY property. Gaan fixen...';
    
    -- Bewaar bestaande data
    IF OBJECT_ID('tempdb..#TempStemmingMuziek') IS NOT NULL
        DROP TABLE #TempStemmingMuziek;
    
    SELECT PK, stemmingID, muziekID
    INTO #TempStemmingMuziek
    FROM stemmingMuziek;
    
    PRINT 'Data bewaard in temp tabel';
    
    -- Drop alle foreign keys die verwijzen naar stemmingMuziek
    DECLARE @sql NVARCHAR(MAX) = '';
    
    SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + 
                        '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
                        ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
    FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID('stemmingMuziek');
    
    IF LEN(@sql) > 0
    BEGIN
        PRINT 'Dropping foreign keys:';
        PRINT @sql;
        EXEC sp_executesql @sql;
    END
    
    -- Drop de originele tabel
    DROP TABLE stemmingMuziek;
    PRINT 'Oude tabel verwijderd';
    
    -- Maak nieuwe tabel met IDENTITY
    CREATE TABLE stemmingMuziek (
        PK INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        stemmingID INT NULL,
        muziekID INT NULL,
        CONSTRAINT FK_stemmingMuziek_stemming FOREIGN KEY (stemmingID) 
            REFERENCES stemming(stemmingID) ON DELETE CASCADE,
        CONSTRAINT FK_stemmingMuziek_muziek FOREIGN KEY (muziekID) 
            REFERENCES muziek(muziekID) ON DELETE CASCADE
    );
    
    PRINT 'Nieuwe tabel aangemaakt met IDENTITY';
    
    -- Herstel data (zonder PK waarden, die worden auto-gegenereerd)
    IF EXISTS (SELECT 1 FROM #TempStemmingMuziek)
    BEGIN
        SET IDENTITY_INSERT stemmingMuziek ON;
        
        INSERT INTO stemmingMuziek (PK, stemmingID, muziekID)
        SELECT PK, stemmingID, muziekID
        FROM #TempStemmingMuziek;
        
        SET IDENTITY_INSERT stemmingMuziek OFF;
        
        PRINT CAST(@@ROWCOUNT AS VARCHAR(10)) + ' rijen hersteld';
    END
    ELSE
    BEGIN
        PRINT 'Geen bestaande data om te herstellen';
    END
    
    DROP TABLE #TempStemmingMuziek;
    
    PRINT 'Fix compleet! PK heeft nu IDENTITY property.';
END
ELSE
BEGIN
    PRINT 'PK kolom heeft al IDENTITY property. Geen actie nodig.';
END
GO

-- Verifieer het resultaat
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.is_nullable AS IsNullable,
    c.is_identity AS IsIdentity,
    IDENT_SEED('stemmingMuziek') AS IdentitySeed,
    IDENT_INCR('stemmingMuziek') AS IdentityIncrement
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('stemmingMuziek')
ORDER BY c.column_id;
GO

SELECT COUNT(*) AS TotalRows FROM stemmingMuziek;
GO
