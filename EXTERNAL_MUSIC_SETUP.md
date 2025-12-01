# Euphonia - Externe Muziek Integratie Setup

## 🎵 Spotify API Setup

### Stap 1: Maak een Spotify Developer Account
1. Ga naar [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
2. Log in met je Spotify account (maak er een aan als je die nog niet hebt)

### Stap 2: Maak een App aan
1. Klik op "Create app"
2. Vul in:
   - **App name**: `Euphonia` (of een naam naar keuze)
   - **App description**: `Music mood tracking application`
   - **Redirect URIs**: `http://localhost:5000/callback` (niet strikt nodig voor onze use case, maar verplicht veld)
   - **Which API/SDKs are you planning to use?**: Selecteer "Web API"
3. Accepteer de voorwaarden en klik op "Save"

### Stap 3: Verkrijg je Credentials
1. Klik op je nieuwe app in het dashboard
2. Klik op "Settings"
3. Je ziet nu:
   - **Client ID**: Kopieer deze
   - **Client Secret**: Klik op "View client secret" en kopieer deze

### Stap 4: Voeg Credentials toe aan appsettings.json
Open `PresentationLayer/appsettings.json` en vervang:
```json
"Spotify": {
  "ClientId": "YOUR_SPOTIFY_CLIENT_ID",    ← Plak hier je Client ID
  "ClientSecret": "YOUR_SPOTIFY_CLIENT_SECRET"  ← Plak hier je Client Secret
}
```

---

## 🎥 YouTube Data API Setup

### Stap 1: Maak een Google Cloud Project
1. Ga naar [Google Cloud Console](https://console.cloud.google.com)
2. Log in met je Google account
3. Klik op "Select a project" → "New Project"
4. Geef je project een naam (bijv. `Euphonia`) en klik op "Create"

### Stap 2: Activeer de YouTube Data API v3
1. Zorg dat je nieuwe project geselecteerd is
2. Ga naar [YouTube Data API v3](https://console.cloud.google.com/apis/library/youtube.googleapis.com)
3. Klik op "Enable"

### Stap 3: Maak een API Key
1. Ga naar [Credentials](https://console.cloud.google.com/apis/credentials)
2. Klik op "+ CREATE CREDENTIALS" → "API key"
3. Je nieuwe API key wordt getoond - **kopieer deze**
4. (Optioneel) Klik op "Edit API key" om de key te beperken:
   - **Application restrictions**: HTTP referrers (websites)
   - **Website restrictions**: `http://localhost:5000/*`
   - **API restrictions**: Selecteer alleen "YouTube Data API v3"
   - Klik op "Save"

### Stap 4: Voeg API Key toe aan appsettings.json
Open `PresentationLayer/appsettings.json` en vervang:
```json
"YouTube": {
  "ApiKey": "YOUR_YOUTUBE_API_KEY"  ← Plak hier je API key
}
```

---

## ✅ Testen

### Spotify Testen
1. Start de applicatie: `dotnet run --project PresentationLayer`
2. Ga naar http://localhost:5000/MuziekView/Create
3. Klik op het "Spotify" tabblad
4. Zoek naar een artiest of nummer (bijv. "Bohemian Rhapsody")
5. Klik op een resultaat om het te selecteren

### YouTube Testen
1. Ga naar http://localhost:5000/MuziekView/Create
2. Klik op het "YouTube" tabblad
3. Plak een YouTube URL (bijv. `https://www.youtube.com/watch?v=dQw4w9WgXcQ`)
4. Klik op "Ophalen"
5. Controleer of de metadata correct wordt ingevuld

---

## 🚨 Troubleshooting

### "Spotify API credentials zijn niet ingesteld"
- Controleer of je `ClientId` en `ClientSecret` correct hebt ingevuld in `appsettings.json`
- Zorg dat de waarden geen aanhalingstekens bevatten (alleen de waarde zelf)
- Herstart de applicatie na het wijzigen van appsettings.json

### "YouTube API key is niet ingesteld"
- Controleer of je API key correct is ingevuld
- Zorg dat de YouTube Data API v3 is geactiveerd in je Google Cloud project
- Check of je API key geen restrictions heeft die localhost blokkeren

### Geen zoekresultaten
- Controleer je internetverbinding
- Open de browser console (F12) om eventuele JavaScript errors te zien
- Check de terminal output voor API errors

### Rate Limits
- **Spotify**: Client Credentials flow heeft geen rate limit voor normale gebruik
- **YouTube**: Gratis tier heeft een quota van 10,000 units per dag (1 video metadata request = 1 unit)
  - Als je dit overschrijdt, krijg je een 403 Forbidden error
  - Check je quota op https://console.cloud.google.com/apis/api/youtube.googleapis.com/quotas

---

## 📝 Notities

### Beveiliging
- **LET OP**: De `ClientSecret` en `ApiKey` in `appsettings.json` zijn gevoelige gegevens
- Voeg `appsettings.json` toe aan `.gitignore` als je de code naar een public repository pusht
- Voor productie: gebruik Azure Key Vault of vergelijkbare secret management oplossingen

### API Limieten
- **Spotify**: Ongelimiteerd voor Client Credentials flow
- **YouTube**: 10,000 quota units per dag (gratis tier)
  - Overweeg caching voor frequently accessed videos
  - Voor hogere limieten: upgrade naar een betaald plan

### Kosten
- **Spotify Web API**: Volledig gratis
- **YouTube Data API v3**: Gratis tot 10,000 units/dag, daarna betaald

---

## 🎯 Volgende Stappen

Nu de basis staat, kun je overwegen om:
1. **Optie D te implementeren**: "Now Playing" feature met Spotify Authorization Code Flow
2. **Caching** toe te voegen voor YouTube metadata
3. **Error logging** te verbeteren met Serilog of Application Insights
4. **Preview playback** te implementeren met Spotify's audio previews

Veel plezier met Euphonia! 🎵
