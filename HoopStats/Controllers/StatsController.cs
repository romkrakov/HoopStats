using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using HoopStats.Models;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace HoopStats.Controllers
{
    // Allow non-authenticated users to view but not edit
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StatsController> _logger;

        public StatsController(ApplicationDbContext context, ILogger<StatsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
        }

        private IActionResult RequireLogin()
        {
            TempData["ErrorMessage"] = "יש להתחבר למערכת כדי לצפות בסטטיסטיקות";
            return RedirectToAction("Login", "User");
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn())
            {
                return RequireLogin();
            }

            try
            {
                var username = HttpContext.Session.GetString("Username");
                _logger.LogInformation($"Stats/Index accessed by user: {username}");
                
                // Retrieve latest game stats using proper date ordering
                var regularStats = _context.GameStats
                    .FromSqlRaw("SELECT * FROM GameStats ORDER BY datetime(GameDate) DESC")
                    .Take(50)
                    .ToList();
                
                if (regularStats.Count == 0)
                {
                    TempData["ErrorMessage"] = "לא נמצאו נתוני משחקים בבסיס הנתונים";
                    _logger.LogWarning("No game stats found in database");
                }
                else
                {
                    _logger.LogInformation($"Found {regularStats.Count} game stats");
                }
                
                return View(regularStats);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה בטעינת נתוני המשחקים: {ex.Message}";
                _logger.LogError(ex, "Error loading game stats");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to import";
                return RedirectToAction("Import");
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<GameStats>();
                    await _context.GameStats.AddRangeAsync(records);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Data imported successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error importing data: " + ex.Message;
                return RedirectToAction("Import");
            }
        }

        [HttpGet]
        public IActionResult PlayerStats(string? playerName)
        {
            if (!IsLoggedIn())
            {
                return RequireLogin();
            }

            if (string.IsNullOrEmpty(playerName))
            {
                // Get player averages
                var playerStats = _context.GameStats
                    .GroupBy(s => s.Player)
                    .Select(g => new HoopStats.Models.PlayerStatsViewModel
                    {
                        PlayerName = g.Key,
                        GamesPlayed = g.Count(),
                        AveragePoints = g.Average(s => s.Points),
                        AverageRebounds = g.Average(s => s.TotalRebounds),
                        AverageAssists = g.Average(s => s.Assists)
                    })
                    .OrderByDescending(p => p.AveragePoints)
                    .Take(20)
                    .ToList();

                return View("PlayersList", playerStats);
            }

            var playerGames = _context.GameStats
                .Where(s => s.Player == playerName)
                .OrderByDescending(s => s.GameDate)
                .ToList();
            
            ViewBag.PlayerName = playerName;
            return View(playerGames);
        }

        public IActionResult TeamStats(string? teamName)
        {
            if (!IsLoggedIn())
            {
                return RequireLogin();
            }

            if (string.IsNullOrEmpty(teamName))
            {
                var teams = _context.GameStats
                    .Select(s => s.Team)
                    .Distinct()
                    .ToList();

                var teamStats = new List<HoopStats.Models.TeamStatsViewModel>();

                foreach (var team in teams)
                {
                    var games = _context.GameStats
                        .Where(s => s.Team == team)
                        .ToList();

                    // Calculate team PPG by first grouping by game date
                    var teamPPG = games
                        .GroupBy(s => s.GameDate)
                        .Select(g => new
                        {
                            GameDate = g.Key,
                            TotalPoints = g.Sum(s => s.Points)
                        })
                        .Average(g => g.TotalPoints);

                    var topScorer = games
                        .GroupBy(s => s.Player)
                        .Select(g => new
                        {
                            Player = g.Key,
                            PPG = g.Average(s => s.Points)
                        })
                        .OrderByDescending(x => x.PPG)
                        .First();

                    teamStats.Add(new HoopStats.Models.TeamStatsViewModel
                    {
                        TeamName = team,
                        GamesPlayed = games.Select(g => g.GameDate).Distinct().Count(),
                        AveragePoints = teamPPG,
                        TopScorerName = topScorer.Player,
                        TopScorerPPG = topScorer.PPG
                    });
                }

                return View("TeamsList", teamStats.OrderBy(t => t.TeamName).ToList());
            }

            var teamPlayers = _context.GameStats
                .Where(s => s.Team == teamName)
                .GroupBy(s => s.Player)
                .Select(g => new HoopStats.Models.PlayerStatsViewModel
                {
                    PlayerName = g.Key,
                    GamesPlayed = g.Count(),
                    AveragePoints = g.Average(s => s.Points),
                    AverageRebounds = g.Average(s => s.TotalRebounds),
                    AverageAssists = g.Average(s => s.Assists)
                })
                .OrderByDescending(s => s.AveragePoints)
                .ToList();

            ViewBag.TeamName = teamName;
            return View(teamPlayers);
        }

        /// <summary>
        /// Displays the latest game results
        /// This action is optimized with database indexes on:
        /// - GameDate: For faster sorting and filtering of recent games
        /// - Team/Opponent: For faster matchup filtering
        /// - Player: For faster player stats lookup
        /// </summary>
        public IActionResult LatestGames()
        {
            if (!IsLoggedIn())
            {
                return RequireLogin();
            }
            
            try
            {
                _logger.LogInformation("Loading latest games");
                
                // Query dates in raw SQL with proper sorting to ensure correct date ordering
                var rawDates = _context.GameStats
                    .FromSqlRaw("SELECT * FROM GameStats ORDER BY datetime(GameDate) DESC")
                    .Select(s => s.GameDate.Date)
                    .Distinct()
                    .Take(20)
                    .ToList();
                
                _logger.LogInformation($"Found {rawDates.Count} distinct game dates - newest date: {rawDates.FirstOrDefault():yyyy-MM-dd}");

                var gameResults = new List<Models.GameResultViewModel>();
                var processedMatchups = new HashSet<string>(); // To track already processed matchups

                foreach (var gameDate in rawDates)
                {
                    // Get all stats for games played on this date
                    var gamesOnDate = _context.GameStats
                        .Where(s => s.GameDate.Date == gameDate)
                        .ToList();
                    
                    _logger.LogInformation($"Processing date {gameDate:yyyy-MM-dd} with {gamesOnDate.Count} player records");

                    // Find unique team combinations for this date
                    var teamCombinations = new List<(string Home, string Away)>();
                    
                    // First, get all unique team combinations from the data
                    var uniqueTeams = gamesOnDate.Select(g => g.Team).Distinct().ToList();
                    
                    foreach (var team in uniqueTeams)
                    {
                        var opponents = gamesOnDate
                            .Where(g => g.Team == team)
                            .Select(g => g.Opponent)
                            .Distinct()
                            .ToList();
                            
                        foreach (var opponent in opponents)
                        {
                            // Make sure we don't add both (A vs B) and (B vs A)
                            var key = string.Join("_", new[] { team, opponent }.OrderBy(t => t));
                            var matchupKey = $"{gameDate:yyyy-MM-dd}_{key}";
                            
                            if (!processedMatchups.Contains(matchupKey))
                            {
                                processedMatchups.Add(matchupKey);
                                teamCombinations.Add((team, opponent));
                            }
                        }
                    }
                    
                    // Process each unique team combination
                    foreach (var combo in teamCombinations)
                    {
                        // Get all player stats for both teams in this matchup
                        var team1Players = gamesOnDate
                            .Where(g => g.Team == combo.Home && g.Opponent == combo.Away)
                            .ToList();
                            
                        var team2Players = gamesOnDate
                            .Where(g => g.Team == combo.Away && g.Opponent == combo.Home)
                            .ToList();

                        if (!team1Players.Any() || !team2Players.Any())
                            continue;

                        // Calculate team scores - sum points by players on each team
                        int team1Score = team1Players.Sum(p => p.Points);
                        int team2Score = team2Players.Sum(p => p.Points);

                        // Determine winner
                        string winner = team1Score > team2Score ? combo.Home : combo.Away;
                        string result = $"{team1Score}-{team2Score}";

                        // Find top scorer in the game
                        var allPlayers = team1Players.Concat(team2Players).ToList();
                        var topScorer = allPlayers.OrderByDescending(p => p.Points).First();

                        gameResults.Add(new Models.GameResultViewModel
                        {
                            GameDate = gameDate,
                            Team1 = combo.Home,
                            Team2 = combo.Away,
                            Result = result,
                            Winner = winner,
                            Team1Score = team1Score,
                            Team2Score = team2Score,
                            TopScorer = topScorer.Player,
                            TopScorerPoints = topScorer.Points
                        });
                    }
                }

                // Ensure results are sorted by proper DateTime comparison
                var sortedResults = gameResults
                    .OrderByDescending(g => g.GameDate.Date.Ticks)
                    .ToList();
                
                _logger.LogInformation($"Returning {sortedResults.Count} game results, ordered by date descending");
                
                return View(sortedResults);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה בטעינת תוצאות המשחקים: {ex.Message}";
                _logger.LogError(ex, "Error loading game results");
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Displays detailed statistics for a specific game between two teams on a given date
        /// </summary>
        /// <param name="date">The date of the game</param>
        /// <param name="team1">The first team in the matchup</param>
        /// <param name="team2">The second team in the matchup</param>
        public IActionResult GameDetails(DateTime date, string team1, string team2)
        {
            if (!IsLoggedIn())
            {
                return RequireLogin();
            }
            
            try
            {
                _logger.LogInformation($"Loading game details for {team1} vs {team2} on {date:yyyy-MM-dd}");
                
                // Round to date to avoid time component comparison issues
                var gameDate = date.Date;
                
                // Get all player stats for both teams in this matchup
                var team1Players = _context.GameStats
                    .Where(g => g.GameDate.Date == gameDate && g.Team == team1 && g.Opponent == team2)
                    .OrderByDescending(p => p.Points)
                    .ToList();
                
                var team2Players = _context.GameStats
                    .Where(g => g.GameDate.Date == gameDate && g.Team == team2 && g.Opponent == team1)
                    .OrderByDescending(p => p.Points)
                    .ToList();

                if (!team1Players.Any() || !team2Players.Any())
                {
                    TempData["ErrorMessage"] = "לא נמצאו נתונים עבור המשחק המבוקש";
                    return RedirectToAction("LatestGames");
                }

                // Calculate team scores
                int team1Score = team1Players.Sum(p => p.Points);
                int team2Score = team2Players.Sum(p => p.Points);

                // Determine winner
                string winner = team1Score > team2Score ? team1 : team2;
                
                // Create the game details view model using the HoopStats.Models namespace
                var gameDetails = new HoopStats.Models.GameDetailsViewModel
                {
                    GameDate = gameDate,
                    Team1 = team1,
                    Team2 = team2,
                    Team1Score = team1Score,
                    Team2Score = team2Score,
                    Winner = winner,
                    Team1Players = team1Players
                        .Select(p => new HoopStats.Models.PlayerGameStatsViewModel
                        {
                            PlayerName = p.Player,
                            Points = p.Points,
                            Rebounds = p.TotalRebounds,
                            Assists = p.Assists,
                            Steals = p.Steals,
                            Blocks = p.Blocks,
                            MinutesPlayed = p.MinutesPlayed,
                            FGM = p.FieldGoalsMade,
                            FGA = p.FieldGoalsAttempted,
                            FGP = p.FieldGoalPercentage,
                            TPM = p.ThreePointersMade,
                            TPA = p.ThreePointersAttempted,
                            TPP = p.ThreePointPercentage,
                            FTM = p.FreeThrowsMade,
                            FTA = p.FreeThrowsAttempted,
                            FTP = p.FreeThrowPercentage,
                            OffRebounds = p.OffensiveRebounds,
                            DefRebounds = p.DefensiveRebounds,
                            Turnovers = p.Turnovers,
                            Fouls = p.PersonalFouls
                        })
                        .ToList(),
                    Team2Players = team2Players
                        .Select(p => new HoopStats.Models.PlayerGameStatsViewModel
                        {
                            PlayerName = p.Player,
                            Points = p.Points,
                            Rebounds = p.TotalRebounds,
                            Assists = p.Assists,
                            Steals = p.Steals,
                            Blocks = p.Blocks,
                            MinutesPlayed = p.MinutesPlayed,
                            FGM = p.FieldGoalsMade,
                            FGA = p.FieldGoalsAttempted,
                            FGP = p.FieldGoalPercentage,
                            TPM = p.ThreePointersMade,
                            TPA = p.ThreePointersAttempted,
                            TPP = p.ThreePointPercentage,
                            FTM = p.FreeThrowsMade,
                            FTA = p.FreeThrowsAttempted,
                            FTP = p.FreeThrowPercentage,
                            OffRebounds = p.OffensiveRebounds,
                            DefRebounds = p.DefensiveRebounds,
                            Turnovers = p.Turnovers,
                            Fouls = p.PersonalFouls
                        })
                        .ToList()
                };
                
                _logger.LogInformation($"Loaded game details with {team1Players.Count} players for {team1} and {team2Players.Count} players for {team2}");
                
                return View(gameDetails);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה בטעינת פרטי המשחק: {ex.Message}";
                _logger.LogError(ex, "Error loading game details");
                return RedirectToAction("LatestGames");
            }
        }
    }

    public class TeamGameViewModel
    {
        public DateTime GameDate { get; set; }
        public string Opponent { get; set; } = "";
        public string Result { get; set; } = "";
        public int TotalPoints { get; set; }
        public List<PlayerGameViewModel> TeamPlayers { get; set; } = new List<PlayerGameViewModel>();
    }
}
