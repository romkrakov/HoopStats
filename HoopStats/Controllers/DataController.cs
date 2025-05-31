using Microsoft.AspNetCore.Mvc;
using HoopStats.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Net.Http;
using Newtonsoft.Json;
using System.Globalization;

namespace HoopStats.Controllers;

public class DataController : Controller
{
    private readonly ILogger<DataController> _logger;
    private readonly ApplicationDbContext _context;

    public DataController(ILogger<DataController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult UpdateData()
    {
        if (!IsAdmin())
        {
            return Forbid();
        }
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateData(int startYear)
    {
        _logger.LogInformation($"UpdateData POST action called with startYear: {startYear}");
        
        if (!IsAdmin())
        {
            _logger.LogWarning("UpdateData called by non-admin user");
            return Forbid();
        }

        if (startYear < 2000 || startYear > DateTime.Now.Year)
        {
            _logger.LogWarning($"Invalid startYear: {startYear}");
            ModelState.AddModelError("", "שנת התחלה חייבת להיות בין 2000 לשנה הנוכחית");
            return View();
        }

        try
        {
            var currentYear = DateTime.Now.Year;
            _logger.LogInformation($"Starting NBA data update from {startYear} to {currentYear}");
            TempData["InfoMessage"] = $"תהליך עדכון הנתונים החל משנת {startYear} עד {currentYear}. זה עשוי לקחת מספר דקות...";
            
            var result = await UpdateNBADataAsync(startYear);
            
            if (result)
            {
                TempData["SuccessMessage"] = "עדכון הנתונים הושלם בהצלחה! הנתונים החדשים זמינים כעת במערכת.";
            }
            else
            {
                TempData["ErrorMessage"] = "אירעה שגיאה בעדכון הנתונים. אנא בדוק את היומנים למידע נוסף.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"שגיאה בעדכון הנתונים: {ex.Message}";
            _logger.LogError(ex, "Error in UpdateData action");
        }

        return RedirectToAction("UpdateData");
    }

    [HttpPost]
    public IActionResult TestPost(string testValue)
    {
        _logger.LogInformation($"TestPost called with value: {testValue}");
        TempData["SuccessMessage"] = $"Test successful! Received: {testValue}";
        return RedirectToAction("UpdateData");
    }

    private async Task<bool> UpdateNBADataAsync(int startYear)
    {
        try
        {
            _logger.LogInformation($"Starting NBA data update from year {startYear}");
            
            _context.GameStats.RemoveRange(_context.GameStats);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Cleared existing game stats data");

            var currentYear = DateTime.Now.Year;
            var seasons = new List<string>();
            for (int year = startYear; year <= currentYear; year++)
            {
                seasons.Add($"{year}-{(year + 1).ToString().Substring(2)}");
            }

            _logger.LogInformation($"Processing {seasons.Count} seasons: {string.Join(", ", seasons)}");

            using (var httpClient = new HttpClient(new HttpClientHandler() 
            { 
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate 
            }))
            {
                httpClient.Timeout = TimeSpan.FromMinutes(2);
                
                var allGameStats = new List<GameStats>();
                
                foreach (var season in seasons)
                {
                    _logger.LogInformation($"Fetching data for season: {season}");
                    
                    var regularSeasonStats = await FetchPlayerGameLogs(httpClient, season, "Regular Season");
                    if (regularSeasonStats != null)
                    {
                        allGameStats.AddRange(regularSeasonStats);
                        _logger.LogInformation($"Added {regularSeasonStats.Count} regular season records for {season}");
                    }
                    
                    var playoffStats = await FetchPlayerGameLogs(httpClient, season, "Playoffs");
                    if (playoffStats != null)
                    {
                        allGameStats.AddRange(playoffStats);
                        _logger.LogInformation($"Added {playoffStats.Count} playoff records for {season}");
                    }
                    
                    await Task.Delay(1000);
                }
                
                if (allGameStats.Any())
                {
                    const int batchSize = 10000;
                    for (int i = 0; i < allGameStats.Count; i += batchSize)
                    {
                        var batch = allGameStats.Skip(i).Take(batchSize);
                        await _context.GameStats.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Saved batch {(i / batchSize) + 1} ({batch.Count()} records)");
                    }
                    
                    _logger.LogInformation($"Successfully updated NBA data with {allGameStats.Count} total records from API");
                    return true;
                }
                else
                {
                    _logger.LogWarning("No data was fetched from NBA API");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating NBA data");
            return false;
        }
    }

    private async Task<List<GameStats>?> FetchPlayerGameLogs(HttpClient httpClient, string season, string seasonType)
    {
        try
        {
            var url = $"https://stats.nba.com/stats/playergamelogs?DateFrom=&DateTo=&GameSegment=&LastNGames=0&LeagueID=00&Location=&MeasureType=Base&Month=0&OpponentTeamID=0&Outcome=&PORound=0&PaceAdjust=N&PerMode=Totals&Period=0&PlayerID=&PlusMinus=N&Rank=N&Season={season}&SeasonSegment=&SeasonType={Uri.EscapeDataString(seasonType)}&ShotClockRange=&TeamID=0&VsConference=&VsDivision=";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Host", "stats.nba.com");
            request.Headers.Add("Origin", "https://www.nba.com");
            request.Headers.Add("Referer", "https://www.nba.com/");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            request.Headers.Add("x-nba-stats-origin", "stats");
            request.Headers.Add("x-nba-stats-token", "true");

            var response = await httpClient.SendAsync(request);
            
            _logger.LogInformation($"NBA API Response Status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                
                var preview = jsonContent.Length > 200 ? jsonContent.Substring(0, 200) + "..." : jsonContent;
                _logger.LogInformation($"NBA API Response Preview: {preview}");
                
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _logger.LogWarning("Empty response received from NBA API");
                    return null;
                }
                
                if (!jsonContent.TrimStart().StartsWith("{") && !jsonContent.TrimStart().StartsWith("["))
                {
                    _logger.LogWarning($"Non-JSON response received. Content appears to be compressed or invalid. Length: {jsonContent.Length}");
                    return null;
                }
                
                var nbaResponse = JsonConvert.DeserializeObject<NBAApiResponse>(jsonContent);
                
                if (nbaResponse?.ResultSets?.Any() == true)
                {
                    var resultSet = nbaResponse.ResultSets[0];
                    var gameStats = new List<GameStats>();
                    
                    foreach (var row in resultSet.RowSet)
                    {
                        var gameStat = ParseGameStatRow(row, resultSet.Headers);
                        if (gameStat != null)
                        {
                            gameStats.Add(gameStat);
                        }
                    }
                    
                    return gameStats;
                }
            }
            else
            {
                _logger.LogWarning($"Failed to fetch data for {season} {seasonType}: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching player game logs for {season} {seasonType}");
        }
        
        return null;
    }

    private GameStats? ParseGameStatRow(object[] row, string[] headers)
    {
        try
        {
            var gameStat = new GameStats
            {
                Player = "",
                Team = "",
                Opponent = "",
                Result = ""
            };
            
            for (int i = 0; i < headers.Length && i < row.Length; i++)
            {
                var header = headers[i];
                var value = row[i];
                
                switch (header)
                {
                    case "PLAYER_NAME":
                        gameStat.Player = value?.ToString() ?? "";
                        break;
                    case "TEAM_ABBREVIATION":
                        gameStat.Team = value?.ToString() ?? "";
                        break;
                    case "MATCHUP":
                        var matchup = value?.ToString() ?? "";
                        if (matchup.Contains(" @ "))
                        {
                            gameStat.Opponent = matchup.Split(" @ ")[1];
                        }
                        else if (matchup.Contains(" vs. "))
                        {
                            gameStat.Opponent = matchup.Split(" vs. ")[1];
                        }
                        break;
                    case "WL":
                        gameStat.Result = value?.ToString() ?? "";
                        break;
                    case "MIN":
                        if (double.TryParse(value?.ToString(), out double minutes))
                            gameStat.MinutesPlayed = minutes;
                        break;
                    case "FGM":
                        if (int.TryParse(value?.ToString(), out int fgm))
                            gameStat.FieldGoalsMade = fgm;
                        break;
                    case "FGA":
                        if (int.TryParse(value?.ToString(), out int fga))
                            gameStat.FieldGoalsAttempted = fga;
                        break;
                    case "FG_PCT":
                        if (double.TryParse(value?.ToString(), out double fgPct))
                            gameStat.FieldGoalPercentage = fgPct;
                        break;
                    case "FG3M":
                        if (int.TryParse(value?.ToString(), out int fg3m))
                            gameStat.ThreePointersMade = fg3m;
                        break;
                    case "FG3A":
                        if (int.TryParse(value?.ToString(), out int fg3a))
                            gameStat.ThreePointersAttempted = fg3a;
                        break;
                    case "FG3_PCT":
                        if (double.TryParse(value?.ToString(), out double fg3Pct))
                            gameStat.ThreePointPercentage = fg3Pct;
                        break;
                    case "FTM":
                        if (int.TryParse(value?.ToString(), out int ftm))
                            gameStat.FreeThrowsMade = ftm;
                        break;
                    case "FTA":
                        if (int.TryParse(value?.ToString(), out int fta))
                            gameStat.FreeThrowsAttempted = fta;
                        break;
                    case "FT_PCT":
                        if (double.TryParse(value?.ToString(), out double ftPct))
                            gameStat.FreeThrowPercentage = ftPct;
                        break;
                    case "OREB":
                        if (int.TryParse(value?.ToString(), out int oreb))
                            gameStat.OffensiveRebounds = oreb;
                        break;
                    case "DREB":
                        if (int.TryParse(value?.ToString(), out int dreb))
                            gameStat.DefensiveRebounds = dreb;
                        break;
                    case "REB":
                        if (int.TryParse(value?.ToString(), out int reb))
                            gameStat.TotalRebounds = reb;
                        break;
                    case "AST":
                        if (int.TryParse(value?.ToString(), out int ast))
                            gameStat.Assists = ast;
                        break;
                    case "STL":
                        if (int.TryParse(value?.ToString(), out int stl))
                            gameStat.Steals = stl;
                        break;
                    case "BLK":
                        if (int.TryParse(value?.ToString(), out int blk))
                            gameStat.Blocks = blk;
                        break;
                    case "TOV":
                        if (int.TryParse(value?.ToString(), out int tov))
                            gameStat.Turnovers = tov;
                        break;
                    case "PF":
                        if (int.TryParse(value?.ToString(), out int pf))
                            gameStat.PersonalFouls = pf;
                        break;
                    case "PTS":
                        if (int.TryParse(value?.ToString(), out int pts))
                            gameStat.Points = pts;
                        break;
                    case "PLUS_MINUS":
                        if (double.TryParse(value?.ToString(), out double plusMinus))
                            gameStat.GameScore = plusMinus;
                        break;
                    case "GAME_DATE":
                        var dateString = value?.ToString();
                        if (!string.IsNullOrEmpty(dateString))
                        {
                            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime gameDate))
                            {
                                gameStat.GameDate = gameDate.Date;
                            }
                            else if (DateTime.TryParse(dateString, out gameDate))
                            {
                                gameStat.GameDate = gameDate.Date;
                            }
                            else
                            {
                                _logger.LogWarning($"Could not parse API date: {dateString}");
                                gameStat.GameDate = DateTime.Now.Date;
                            }
                        }
                        break;
                }
            }
            
            return gameStat;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing game stat row");
            return null;
        }
    }

    private bool IsAdmin()
    {
        var isAdminString = HttpContext.Session.GetString("IsAdmin");
        return bool.TryParse(isAdminString, out bool isAdmin) && isAdmin;
    }
}