using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euphonia.BusinessLogicLayer.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(int userId)
        {
            var dashboard = new DashboardDto();

            // Haal alle data op
            var stemmingen = (await _unitOfWork.StemmingRepository.GetStemmingenByUserIdAsync(userId)).ToList();
            var alleMuziek = (await _unitOfWork.MuziekRepository.GetAllAsync()).ToList();
            var stemmingTypes = (await _unitOfWork.StemmingTypeRepository.GetAllTypesAsync()).ToList();
            
            // 🎯 Haal ACTIEF profiel op
            var profiel = await _unitOfWork.ProfielRepository.GetActiveByUserIdAsync(userId);
            dashboard.Profiel = profiel != null ? MapProfielToDto(profiel) : null;
            dashboard.HeeftProfiel = profiel != null;

            // Algemene statistieken
            dashboard.TotaalStemmingen = stemmingen.Count;
            dashboard.TotaalMuziek = alleMuziek.Count;
            dashboard.LaatsteStemmingDatum = stemmingen.OrderByDescending(s => s.DatumTijd).FirstOrDefault()?.DatumTijd;

            // Tel gekoppelde muziek
            int totaalKoppelingen = 0;
            foreach (var stemming in stemmingen)
            {
                var koppelingen = await _unitOfWork.StemmingMuziekRepository.GetMuziekByStemmingIdAsync(stemming.StemmingID);
                totaalKoppelingen += koppelingen.Count();
            }
            dashboard.TotaalGekoppeldeMuziek = totaalKoppelingen;

            // Stemming type statistieken
            if (stemmingen.Any())
            {
                var stemmingGroups = stemmingen
                    .Where(s => s.TypeID.HasValue)
                    .GroupBy(s => s.TypeID)
                    .Select(g => new
                    {
                        TypeID = g.Key!.Value,
                        Aantal = g.Count()
                    })
                    .ToList();

                foreach (var group in stemmingGroups)
                {
                    var type = stemmingTypes.FirstOrDefault(t => t.TypeID == group.TypeID);
                    dashboard.StemmingTypeStatistieken.Add(new StemmingTypeStatistiekDto
                    {
                        TypeID = group.TypeID,
                        TypeNaam = type?.Naam,
                        Aantal = group.Aantal,
                        Percentage = Math.Round((double)group.Aantal / stemmingen.Count * 100, 1)
                    });
                }

                // Meest voorkomende stemming
                var meestVoorkomend = dashboard.StemmingTypeStatistieken
                    .OrderByDescending(s => s.Aantal)
                    .FirstOrDefault();
                
                if (meestVoorkomend != null)
                {
                    var type = stemmingTypes.FirstOrDefault(t => t.TypeID == meestVoorkomend.TypeID);
                    dashboard.MeestVoorkomendeeStemming = new StemmingTypeDto
                    {
                        TypeID = type?.TypeID ?? 0,
                        Naam = type?.Naam,
                        Beschrijving = type?.Beschrijving
                    };
                }
            }

            // Stemming trend per dag van de week
            if (stemmingen.Any())
            {
                var dagGroups = stemmingen
                    .Where(s => s.DatumTijd.HasValue && s.TypeID.HasValue)
                    .GroupBy(s => s.DatumTijd!.Value.DayOfWeek)
                    .ToList();

                foreach (var dagGroup in dagGroups)
                {
                    var meestVoorkomendeType = dagGroup
                        .GroupBy(s => s.TypeID)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault();

                    if (meestVoorkomendeType != null)
                    {
                        var type = stemmingTypes.FirstOrDefault(t => t.TypeID == meestVoorkomendeType.Key);
                        dashboard.StemmingTrendPerDag.Add(new StemmingTrendDto
                        {
                            DagVanWeek = dagGroup.Key,
                            MeestVoorkomendeeStemming = type?.Naam,
                            Aantal = dagGroup.Count()
                        });
                    }
                }

                // Sorteer op dag van de week
                dashboard.StemmingTrendPerDag = dashboard.StemmingTrendPerDag
                    .OrderBy(t => (int)t.DagVanWeek)
                    .ToList();
            }

            // Top artiesten statistieken
            var artistenData = new Dictionary<string, (int liedjes, List<int> stemmingIds)>();
            
            foreach (var stemming in stemmingen)
            {
                var gekoppeldeMuziek = await _unitOfWork.StemmingMuziekRepository.GetMuziekByStemmingIdAsync(stemming.StemmingID);
                
                foreach (var koppeling in gekoppeldeMuziek.Where(k => k.Muziek != null))
                {
                    var artiest = koppeling.Muziek!.Artiest ?? "Onbekend";
                    
                    if (!artistenData.ContainsKey(artiest))
                    {
                        artistenData[artiest] = (0, new List<int>());
                    }
                    
                    var current = artistenData[artiest];
                    current.liedjes++;
                    if (!current.stemmingIds.Contains(stemming.StemmingID))
                    {
                        current.stemmingIds.Add(stemming.StemmingID);
                    }
                    artistenData[artiest] = current;
                }
            }

            dashboard.TopArtiesten = artistenData
                .OrderByDescending(kvp => kvp.Value.liedjes)
                .Take(5)
                .Select(kvp =>
                {
                    // Bepaal bij welke stemmingen deze artiest het meest voorkomt
                    var stemmingTypesVoorArtiest = stemmingen
                        .Where(s => kvp.Value.stemmingIds.Contains(s.StemmingID) && s.TypeID.HasValue)
                        .GroupBy(s => s.TypeID)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g =>
                        {
                            var type = stemmingTypes.FirstOrDefault(t => t.TypeID == g.Key);
                            return type?.Naam ?? "Onbekend";
                        })
                        .ToList();

                    return new ArtiestStatistiekDto
                    {
                        Artiest = kvp.Key,
                        AantalLiedjes = kvp.Value.liedjes,
                        AantalKoppelingen = kvp.Value.stemmingIds.Count,
                        MeestBijStemmingen = stemmingTypesVoorArtiest
                    };
                })
                .ToList();

            // Muziek-stemming correlaties
            var muziekStemmingData = new List<(string titel, string artiest, int typeId, int count)>();
            
            foreach (var stemming in stemmingen.Where(s => s.TypeID.HasValue))
            {
                var gekoppeldeMuziek = await _unitOfWork.StemmingMuziekRepository.GetMuziekByStemmingIdAsync(stemming.StemmingID);
                
                foreach (var koppeling in gekoppeldeMuziek.Where(k => k.Muziek != null))
                {
                    muziekStemmingData.Add((
                        koppeling.Muziek!.Titel ?? "Onbekend",
                        koppeling.Muziek!.Artiest ?? "Onbekend",
                        stemming.TypeID!.Value,
                        1
                    ));
                }
            }

            dashboard.MuziekStemmingCorrelaties = muziekStemmingData
                .GroupBy(m => new { m.titel, m.artiest, m.typeId })
                .Select(g =>
                {
                    var type = stemmingTypes.FirstOrDefault(t => t.TypeID == g.Key.typeId);
                    return new MuziekStemmingCorrelatie
                    {
                        MuziekTitel = g.Key.titel,
                        Artiest = g.Key.artiest,
                        StemmingType = type?.Naam,
                        AantalKeerGekoppeld = g.Count()
                    };
                })
                .OrderByDescending(c => c.AantalKeerGekoppeld)
                .Take(10)
                .ToList();

            // Meest gekoppelde muziek
            var muziekKoppelingCount = new Dictionary<int, int>();
            
            foreach (var stemming in stemmingen)
            {
                var gekoppeldeMuziek = await _unitOfWork.StemmingMuziekRepository.GetMuziekByStemmingIdAsync(stemming.StemmingID);
                
                foreach (var koppeling in gekoppeldeMuziek.Where(k => k.MuziekID.HasValue))
                {
                    var muziekId = koppeling.MuziekID!.Value;
                    muziekKoppelingCount[muziekId] = muziekKoppelingCount.GetValueOrDefault(muziekId, 0) + 1;
                }
            }

            var topMuziekIds = muziekKoppelingCount
                .OrderByDescending(kvp => kvp.Value)
                .Take(5)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var muziekId in topMuziekIds)
            {
                var muziek = alleMuziek.FirstOrDefault(m => m.MuziekID == muziekId);
                if (muziek != null)
                {
                    dashboard.MeestGekoppeldeMuziek.Add(new MuziekDto
                    {
                        MuziekID = muziek.MuziekID,
                        Titel = muziek.Titel,
                        Artiest = muziek.Artiest,
                        Bron = muziek.Bron
                    });
                }
            }

            // Genereer inzichten
            dashboard.Inzichten = GenereerInzichten(dashboard, stemmingen);
            
            // 🎯 Genereer profiel-based inzichten
            if (dashboard.HeeftProfiel && dashboard.Profiel != null)
            {
                dashboard.ProfielInzichten = GenereerProfielInzichten(dashboard, alleMuziek, stemmingen);
                
                // 🎵 Genereer muziekaanbevelingen op basis van actief profiel
                dashboard.AanbevolenMuziek = await GenereerMuziekAanbevelingen(dashboard.Profiel, alleMuziek, stemmingen, stemmingTypes);
            }

            return dashboard;
        }

        // 🎯 Nieuwe method voor profiel mapping
        private ProfielDto MapProfielToDto(DataAccessLayer.Models.Profiel profiel)
        {
            return new ProfielDto
            {
                ProfielID = profiel.ProfielID,
                UserID = profiel.UserID,
                VoorkeurGenres = profiel.VoorkeurGenres,
                Stemmingstags = profiel.Stemmingstags,
                IsActive = profiel.IsActive
            };
        }

        // 🎵 Nieuwe method voor muziekaanbevelingen op basis van profiel
        private async Task<List<MuziekDto>> GenereerMuziekAanbevelingen(
            ProfielDto profiel,
            List<DataAccessLayer.Models.Muziek> alleMuziek,
            List<DataAccessLayer.Models.Stemming> stemmingen,
            List<DataAccessLayer.Models.StemmingType> stemmingTypes)
        {
            var aanbevelingen = new List<MuziekDto>();
            
            if (string.IsNullOrWhiteSpace(profiel.Stemmingstags))
            {
                return aanbevelingen; // Geen tags = geen aanbevelingen
            }

            // Parse profiel tags
            var profielTags = profiel.Stemmingstags
                .Split(',')
                .Select(t => t.Trim().ToLower())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            if (!profielTags.Any())
            {
                return aanbevelingen;
            }

            // Vind stemmingen die matchen met profiel tags
            var matchingStemmingen = stemmingen
                .Where(s => s.StemmingType != null && 
                            s.StemmingType.Naam != null &&
                            profielTags.Any(tag => s.StemmingType.Naam.ToLower().Contains(tag)))
                .ToList();

            if (!matchingStemmingen.Any())
            {
                return aanbevelingen;
            }

            // Verzamel muziek IDs van matching stemmingen met score
            var muziekScores = new Dictionary<int, int>();
            
            foreach (var stemming in matchingStemmingen)
            {
                var gekoppeldeMuziek = await _unitOfWork.StemmingMuziekRepository.GetMuziekByStemmingIdAsync(stemming.StemmingID);
                
                foreach (var koppeling in gekoppeldeMuziek.Where(k => k.MuziekID.HasValue))
                {
                    var muziekId = koppeling.MuziekID!.Value;
                    muziekScores[muziekId] = muziekScores.GetValueOrDefault(muziekId, 0) + 1;
                }
            }

            // Sorteer op score en neem top 20
            var topMuziekIds = muziekScores
                .OrderByDescending(kvp => kvp.Value)
                .Take(20)
                .Select(kvp => kvp.Key)
                .ToList();

            // Map naar DTOs
            foreach (var muziekId in topMuziekIds)
            {
                var muziek = alleMuziek.FirstOrDefault(m => m.MuziekID == muziekId);
                if (muziek != null)
                {
                    aanbevelingen.Add(new MuziekDto
                    {
                        MuziekID = muziek.MuziekID,
                        Titel = muziek.Titel,
                        Artiest = muziek.Artiest,
                        Bron = muziek.Bron
                    });
                }
            }

            return aanbevelingen;
        }

        // 🎯 Nieuwe method voor profiel-based inzichten
        private List<string> GenereerProfielInzichten(DashboardDto dashboard, List<DataAccessLayer.Models.Muziek> alleMuziek, List<DataAccessLayer.Models.Stemming> stemmingen)
        {
            var inzichten = new List<string>();
            var profiel = dashboard.Profiel!;

            // Inzicht 1: Genre matching
            if (!string.IsNullOrWhiteSpace(profiel.VoorkeurGenres))
            {
                var voorkeurGenres = profiel.VoorkeurGenres.Split(',').Select(g => g.Trim()).ToList();
                var muziekMetBron = alleMuziek.Where(m => !string.IsNullOrWhiteSpace(m.Bron)).ToList();
                
                if (muziekMetBron.Any())
                {
                    // Simuleer genre matching (in werkelijkheid zou je MuziekAnalyse gebruiken)
                    var matchingMuziek = muziekMetBron.Take(5).ToList();
                    inzichten.Add($"Je hebt {voorkeurGenres.Count} voorkeur genre(s) ingesteld: {string.Join(", ", voorkeurGenres)}");
                }
            }

            // Inzicht 2: Stemmingstags matching
            if (!string.IsNullOrWhiteSpace(profiel.Stemmingstags))
            {
                var tags = profiel.Stemmingstags.Split(',').Select(t => t.Trim()).ToList();
                var matchingStemmingen = stemmingen
                    .Where(s => s.StemmingType != null && tags.Any(tag => 
                        s.StemmingType.Naam != null && 
                        s.StemmingType.Naam.Contains(tag, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (matchingStemmingen.Any())
                {
                    var percentage = Math.Round((double)matchingStemmingen.Count / stemmingen.Count * 100, 1);
                    inzichten.Add($"{percentage}% van je stemmingen matchen je focus tags ({string.Join(", ", tags)})");
                }
                else
                {
                    inzichten.Add($"💡 Tip: Registreer meer stemmingen met je focus tags: {string.Join(", ", tags)}");
                }
            }

            // Inzicht 3: Profiel volledigheid
            if (string.IsNullOrWhiteSpace(profiel.VoorkeurGenres))
            {
                inzichten.Add("⚠️ Je profiel heeft nog geen voorkeur genres. Voeg ze toe voor betere aanbevelingen!");
            }

            if (string.IsNullOrWhiteSpace(profiel.Stemmingstags))
            {
                inzichten.Add("⚠️ Je profiel heeft nog geen stemmingstags. Voeg ze toe om je favoriete stemmingen te tracken!");
            }

            return inzichten;
        }

        private List<string> GenereerInzichten(DashboardDto dashboard, List<Euphonia.DataAccessLayer.Models.Stemming> stemmingen)
        {
            var inzichten = new List<string>();

            // Inzicht 1: Meest voorkomende stemming
            if (dashboard.MeestVoorkomendeeStemming != null)
            {
                var percentage = dashboard.StemmingTypeStatistieken
                    .FirstOrDefault(s => s.TypeID == dashboard.MeestVoorkomendeeStemming.TypeID)
                    ?.Percentage ?? 0;
                
                inzichten.Add($"Je voelt je het vaakst '{dashboard.MeestVoorkomendeeStemming.Naam}' ({percentage}% van de tijd)");
            }

            // Inzicht 2: Favoriete artiest
            if (dashboard.TopArtiesten.Any())
            {
                var topArtiest = dashboard.TopArtiesten.First();
                inzichten.Add($"Je luistert het meest naar {topArtiest.Artiest} ({topArtiest.AantalKoppelingen} keer gekoppeld)");
                
                if (topArtiest.MeestBijStemmingen.Any())
                {
                    inzichten.Add($"{topArtiest.Artiest} helpt je vooral als je {string.Join(", ", topArtiest.MeestBijStemmingen)} bent");
                }
            }

            // Inzicht 3: Dag patroon
            if (dashboard.StemmingTrendPerDag.Any())
            {
                var dagNamen = new Dictionary<DayOfWeek, string>
                {
                    { DayOfWeek.Monday, "maandag" },
                    { DayOfWeek.Tuesday, "dinsdag" },
                    { DayOfWeek.Wednesday, "woensdag" },
                    { DayOfWeek.Thursday, "donderdag" },
                    { DayOfWeek.Friday, "vrijdag" },
                    { DayOfWeek.Saturday, "zaterdag" },
                    { DayOfWeek.Sunday, "zondag" }
                };

                var meestActieveDag = dashboard.StemmingTrendPerDag
                    .OrderByDescending(t => t.Aantal)
                    .FirstOrDefault();
                
                if (meestActieveDag != null && dagNamen.ContainsKey(meestActieveDag.DagVanWeek))
                {
                    inzichten.Add($"Je registreert je stemmingen het meest op {dagNamen[meestActieveDag.DagVanWeek]}");
                }
            }

            // Inzicht 4: Muziek correlatie
            if (dashboard.MuziekStemmingCorrelaties.Any())
            {
                var sterkeCorrelatie = dashboard.MuziekStemmingCorrelaties.First();
                if (sterkeCorrelatie.AantalKeerGekoppeld >= 3)
                {
                    inzichten.Add($"'{sterkeCorrelatie.MuziekTitel}' lijkt een go-to nummer als je {sterkeCorrelatie.StemmingType} bent");
                }
            }

            // Inzicht 5: Algemeen gebruik
            if (dashboard.TotaalStemmingen > 0)
            {
                var gemiddeldMuziekPerStemming = dashboard.TotaalGekoppeldeMuziek > 0 
                    ? Math.Round((double)dashboard.TotaalGekoppeldeMuziek / dashboard.TotaalStemmingen, 1)
                    : 0;
                
                if (gemiddeldMuziekPerStemming > 0)
                {
                    inzichten.Add($"Je koppelt gemiddeld {gemiddeldMuziekPerStemming} liedje(s) per stemming");
                }
            }

            // Inzicht 6: Recente activiteit
            if (dashboard.LaatsteStemmingDatum.HasValue)
            {
                var dagenGeleden = (DateTime.Now - dashboard.LaatsteStemmingDatum.Value).Days;
                if (dagenGeleden == 0)
                {
                    inzichten.Add("Je hebt vandaag je stemming geregistreerd - goed bezig! 🎉");
                }
                else if (dagenGeleden == 1)
                {
                    inzichten.Add("Je laatste stemming was gisteren geregistreerd");
                }
                else if (dagenGeleden <= 7)
                {
                    inzichten.Add($"Je laatste stemming was {dagenGeleden} dagen geleden");
                }
            }

            return inzichten;
        }
    }
}
