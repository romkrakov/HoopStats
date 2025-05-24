.mode csv
.headers on
.import '/Users/romkrakovsky/Library/CloudStorage/GoogleDrive-romkrakov@gmail.com/My Drive/כיתה י/מחשבים/HTML/HoopStats/HoopStats/database_24_25.csv' temp_import

CREATE TABLE IF NOT EXISTS temp_gamestats (
    Player TEXT,
    Tm TEXT,
    Opp TEXT,
    Res TEXT,
    MP TEXT,
    FG INTEGER,
    FGA INTEGER,
    "FG%" REAL,
    "3P" INTEGER,
    "3PA" INTEGER,
    "3P%" REAL,
    FT INTEGER,
    FTA INTEGER,
    "FT%" REAL,
    ORB INTEGER,
    DRB INTEGER,
    TRB INTEGER,
    AST INTEGER,
    STL INTEGER,
    BLK INTEGER,
    TOV INTEGER,
    PF INTEGER,
    PTS INTEGER,
    GmSc REAL,
    Data TEXT
);

INSERT INTO GameStats (
    Player, Team, Opponent, Result, MinutesPlayed,
    FieldGoalsMade, FieldGoalsAttempted, FieldGoalPercentage,
    ThreePointersMade, ThreePointersAttempted, ThreePointPercentage,
    FreeThrowsMade, FreeThrowsAttempted, FreeThrowPercentage,
    OffensiveRebounds, DefensiveRebounds, TotalRebounds,
    Assists, Steals, Blocks, Turnovers, PersonalFouls,
    Points, GameScore, GameDate
)
SELECT 
    Player, Tm, Opp, Res, 
    CAST(MP AS REAL),
    FG, FGA, "FG%",
    "3P", "3PA", "3P%",
    FT, FTA, "FT%",
    ORB, DRB, TRB,
    AST, STL, BLK, TOV, PF,
    PTS, GmSc,
    date(Data) as GameDate
FROM temp_import
WHERE Player != 'Player';

DROP TABLE temp_import;
