-- Voeg test notificaties toe voor gebruiker met ID 0
-- Pas het userID aan naar jouw gebruiker ID

USE ResonanceDB;
GO

-- Controleer of er gebruikers zijn
IF EXISTS (SELECT 1 FROM table1_user WHERE userID = 0)
BEGIN
    -- Welkomst notificatie
    INSERT INTO notificatie (userID, tekst, type, icoon, isGelezen, datum_tijd, link)
    VALUES 
    (0, 'Welkom bij Resonance! Begin met het tracken van je stemmingen en ontdek patronen in je muziek.', 'Info', 'bi-star', 0, GETDATE(), '/Dashboard');

    -- Stemming toegevoegd notificatie
    INSERT INTO notificatie (userID, tekst, type, icoon, isGelezen, datum_tijd, link)
    VALUES 
    (0, 'Nieuwe stemming ''Blij'' toegevoegd! Koppel er muziek aan om je ervaring compleet te maken.', 'Success', 'bi-emoji-smile', 0, DATEADD(MINUTE, -5, GETDATE()), '/Stemming');

    -- Muziek gekoppeld notificatie
    INSERT INTO notificatie (userID, tekst, type, icoon, isGelezen, datum_tijd, link)
    VALUES 
    (0, 'Muziek ''Bohemian Rhapsody'' succesvol gekoppeld aan je stemming!', 'Success', 'bi-music-note-beamed', 0, DATEADD(MINUTE, -10, GETDATE()), '/MuziekView');

    -- Oude gelezen notificatie
    INSERT INTO notificatie (userID, tekst, type, icoon, isGelezen, datum_tijd, link)
    VALUES 
    (0, 'Je hebt je eerste stemming succesvol toegevoegd!', 'Success', 'bi-check-circle', 1, DATEADD(DAY, -1, GETDATE()), NULL);

    -- Warning notificatie
    INSERT INTO notificatie (userID, tekst, type, icoon, isGelezen, datum_tijd, link)
    VALUES 
    (0, 'Je hebt deze week nog geen stemmingen toegevoegd. Vergeet niet om je stemming bij te houden!', 'Warning', 'bi-exclamation-triangle', 0, DATEADD(HOUR, -2, GETDATE()), '/Stemming/Create');

    PRINT 'Test notificaties succesvol toegevoegd!';
END
ELSE
BEGIN
    PRINT 'Geen gebruiker gevonden met ID 0. Pas het script aan naar jouw gebruiker ID.';
END
GO

