using System.Collections.Generic;

namespace HoopStats.Models;

public static class TeamInfo
{
    public static readonly Dictionary<string, (string Name, string HebrewName, string LogoPath)> Teams = new()
    {   
        { "ATL", ("Atlanta Hawks", "אטלנטה הוקס", "/img/teams/atlanta_hawks_logo.svg") },
        { "BOS", ("Boston Celtics", "בוסטון סלטיקס", "/img/teams/boston_celtics_logo.svg") },
        { "BKN", ("Brooklyn Nets", "ברוקלין נטס", "/img/teams/brooklyn_nets_logo.svg") },
        { "CHA", ("Charlotte Hornets", "שארלוט הורנטס", "/img/teams/charlotte_hornets_logo.svg") },
        { "CHI", ("Chicago Bulls", "שיקגו בולס", "/img/teams/chicago_bulls_logo.svg") },
        { "CLE", ("Cleveland Cavaliers", "קליבלנד קאבלירס", "/img/teams/cleveland_cavaliers_logo.svg") },
        { "DAL", ("Dallas Mavericks", "דאלאס מאבריקס", "/img/teams/dallas_mavericks_logo.svg") },
        { "DEN", ("Denver Nuggets", "דנבר נאגטס", "/img/teams/denver_nuggets_logo.svg") },
        { "DET", ("Detroit Pistons", "דטרויט פיסטונס", "/img/teams/detroit_pistons_logo.svg") },
        { "GSW", ("Golden State Warriors", "גולדן סטייט ווריורס", "/img/teams/golden_state_warriors_logo.svg") },
        { "HOU", ("Houston Rockets", "יוסטון רוקטס", "/img/teams/houston_rockets_logo.svg") },
        { "IND", ("Indiana Pacers", "אינדיאנה פייסרס", "/img/teams/indiana_pacers_logo.svg") },
        { "LAC", ("Los Angeles Clippers", "לוס אנג'לס קליפרס", "/img/teams/la_clippers_logo.svg") },
        { "LAL", ("Los Angeles Lakers", "לוס אנג'לס לייקרס", "/img/teams/los_angeles_lakers_logo.svg") },
        { "MEM", ("Memphis Grizzlies", "ממפיס גריזליס", "/img/teams/memphis_grizzlies_logo.svg") },
        { "MIA", ("Miami Heat", "מיאמי היט", "/img/teams/miami_heat_logo.svg") },
        { "MIL", ("Milwaukee Bucks", "מילווקי באקס", "/img/teams/milwaukee_bucks_logo.svg") },
        { "MIN", ("Minnesota Timberwolves", "מינסוטה טימברוולבס", "/img/teams/minnesota_timberwolves_logo.svg") },
        { "NOP", ("New Orleans Pelicans", "ניו אורלינס פליקנס", "/img/teams/new_orleans_pelicans_logo.svg") },
        { "NYK", ("New York Knicks", "ניו יורק ניקס", "/img/teams/new_york_knicks_logo.svg") },
        { "OKC", ("Oklahoma City Thunder", "אוקלהומה סיטי ת'אנדר", "/img/teams/oklahoma_city_thunder_logo.svg") },
        { "ORL", ("Orlando Magic", "אורלנדו מג'יק", "/img/teams/orlando_magic_logo.svg") },
        { "PHI", ("Philadelphia 76ers", "פילדלפיה 76", "/img/teams/philadelphia_76ers_logo.svg") },
        { "PHX", ("Phoenix Suns", "פיניקס סאנס", "/img/teams/phoenix_suns_logo.svg") },
        { "POR", ("Portland Trail Blazers", "פורטלנד טרייל בלייזרס", "/img/teams/portland_trail_blazers_logo.svg") },
        { "SAC", ("Sacramento Kings", "סקרמנטו קינגס", "/img/teams/sacramento_kings_logo.svg") },
        { "SAS", ("San Antonio Spurs", "סן אנטוניו ספרס", "/img/teams/san_antonio_spurs_logo.svg") },
        { "TOR", ("Toronto Raptors", "טורונטו ראפטורס", "/img/teams/toronto_raptors_logo.svg") },
        { "UTA", ("Utah Jazz", "יוטה ג'אז", "/img/teams/utah_jazz_logo.svg") },
        { "WAS", ("Washington Wizards", "וושינגטון וויזארדס", "/img/teams/washington_wizards_logo.svg") }
    };

    public static string GetTeamFullName(string abbreviation)
    {
        return Teams.TryGetValue(abbreviation, out var team) ? team.Name : abbreviation;
    }

    public static string GetTeamHebrewName(string abbreviation)
    {
        return Teams.TryGetValue(abbreviation, out var team) ? team.HebrewName : abbreviation;
    }

    public static string GetTeamLogoPath(string abbreviation)
    {
        return Teams.TryGetValue(abbreviation, out var team) ? team.LogoPath : "/img/teams/default.png";
    }
}
