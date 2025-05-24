using System.Collections.Generic;

namespace HoopStats.Models;

public static class TeamInfo
{
    public static readonly Dictionary<string, (string Name, string LogoPath)> Teams = new()
    {   { "ATL", ("Atlanta Hawks", "/img/teams/atlanta_hawks_logo.svg") },
        { "BOS", ("Boston Celtics", "/img/teams/boston_celtics_logo.svg") },
        { "BKN", ("Brooklyn Nets", "/img/teams/brooklyn_nets_logo.svg") },
        { "CHA", ("Charlotte Hornets", "/img/teams/charlotte_hornets_logo.svg") },
        { "CHI", ("Chicago Bulls", "/img/teams/chicago_bulls_logo.svg") },
        { "CLE", ("Cleveland Cavaliers", "/img/teams/cleveland_cavaliers_logo.svg") },
        { "DAL", ("Dallas Mavericks", "/img/teams/dallas_mavericks_logo.svg") },
        { "DEN", ("Denver Nuggets", "/img/teams/denver_nuggets_logo.svg") },
        { "DET", ("Detroit Pistons", "/img/teams/detroit_pistons_logo.svg") },
        { "GSW", ("Golden State Warriors", "/img/teams/golden_state_warriors_logo.svg") },
        { "HOU", ("Houston Rockets", "/img/teams/houston_rockets_logo.svg") },
        { "IND", ("Indiana Pacers", "/img/teams/indiana_pacers_logo.svg") },
        { "LAC", ("Los Angeles Clippers", "/img/teams/la_clippers_logo.svg") },
        { "LAL", ("Los Angeles Lakers", "/img/teams/los_angeles_lakers_logo.svg") },
        { "MEM", ("Memphis Grizzlies", "/img/teams/memphis_grizzlies_logo.svg") },
        { "MIA", ("Miami Heat", "/img/teams/miami_heat_logo.svg") },
        { "MIL", ("Milwaukee Bucks", "/img/teams/milwaukee_bucks_logo.svg") },
        { "MIN", ("Minnesota Timberwolves", "/img/teams/minnesota_timberwolves_logo.svg") },
        { "NOP", ("New Orleans Pelicans", "/img/teams/new_orleans_pelicans_logo.svg") },
        { "NYK", ("New York Knicks", "/img/teams/new_york_knicks_logo.svg") },
        { "OKC", ("Oklahoma City Thunder", "/img/teams/oklahoma_city_thunder_logo.svg") },
        { "ORL", ("Orlando Magic", "/img/teams/orlando_magic_logo.svg") },
        { "PHI", ("Philadelphia 76ers", "/img/teams/philadelphia_76ers_logo.svg") },
        { "PHX", ("Phoenix Suns", "/img/teams/phoenix_suns_logo.svg") },
        { "POR", ("Portland Trail Blazers", "/img/teams/portland_trail_blazers_logo.svg") },
        { "SAC", ("Sacramento Kings", "/img/teams/sacramento_kings_logo.svg") },
        { "SAS", ("San Antonio Spurs", "/img/teams/san_antonio_spurs_logo.svg") },
        { "TOR", ("Toronto Raptors", "/img/teams/toronto_raptors_logo.svg") },
        { "UTA", ("Utah Jazz", "/img/teams/utah_jazz_logo.svg") },
        { "WAS", ("Washington Wizards", "/img/teams/washington_wizards_logo.svg") }
    };

    public static string GetTeamFullName(string abbreviation)
    {
        return Teams.TryGetValue(abbreviation, out var team) ? team.Name : abbreviation;
    }

    public static string GetTeamLogoPath(string abbreviation)
    {
        return Teams.TryGetValue(abbreviation, out var team) ? team.LogoPath : "/img/teams/default.png";
    }
}
