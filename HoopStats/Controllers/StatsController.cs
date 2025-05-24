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

        public IActionResult Index()
        {
            try
            {
                // For debugging
                var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
                var username = HttpContext.Session.GetString("Username");
                
                _logger.LogInformation($"Stats/Index accessed. Auth: {isAuthenticated}, Username: {username}");
                
                var stats = _context.GameStats
                    .OrderByDescending(s => s.GameDate)
                    .Take(50)
                    .ToList();

                // Add check for empty results
                if (stats == null || stats.Count == 0)
                {
                    TempData["ErrorMessage"] = "לא נמצאו נתוני משחקים בבסיס הנתונים";
                    _logger.LogWarning("No game stats found in database");
                }
                else
                {
                    _logger.LogInformation($"Found {stats.Count} game stats");
                }

                return View(stats);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה בטעינת נתוני המשחקים: {ex.Message}";
                _logger.LogError(ex, "Error loading game stats");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
            if (string.IsNullOrEmpty(playerName))
            {
                // Get player averages
                var playerStats = _context.GameStats
                    .GroupBy(s => s.Player)
                    .Select(g => new PlayerStatsViewModel
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
            if (string.IsNullOrEmpty(teamName))
            {
                var teams = _context.GameStats
                    .Select(s => s.Team)
                    .Distinct()
                    .ToList();

                var teamStats = new List<TeamStatsViewModel>();

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

                    teamStats.Add(new TeamStatsViewModel
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
                .Select(g => new PlayerStatsViewModel
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
    }

    public class PlayerStatsViewModel
    {
        public string PlayerName { get; set; } = "";
        public int GamesPlayed { get; set; }
        public double AveragePoints { get; set; }
        public double AverageRebounds { get; set; }
        public double AverageAssists { get; set; }
    }

    public class TeamStatsViewModel
    {
        public string TeamName { get; set; } = "";
        public int GamesPlayed { get; set; }
        public double AveragePoints { get; set; }
        public string TopScorerName { get; set; } = "";
        public double TopScorerPPG { get; set; }
    }
}
