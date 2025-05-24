import pandas as pd
import sqlite3
import os
from nba_api.stats.endpoints import playergamelogs
from datetime import datetime

# Define the range of seasons
start_year = 2023
current_year = datetime.now().year
seasons = []
for year in range(start_year, current_year + 1):
    seasons.append(f'{year}-{str(year+1)[2:]}')

all_game_logs = pd.DataFrame()

for season_id in seasons:
    try:
        print(f"Fetching data for season: {season_id}")
        # Fetch regular season game logs
        game_logs_reg = playergamelogs.PlayerGameLogs(season_nullable=season_id, season_type_nullable='Regular Season')
        df_reg = game_logs_reg.get_data_frames()[0]
        all_game_logs = pd.concat([all_game_logs, df_reg], ignore_index=True)

        # Fetch playoff game logs
        game_logs_playoffs = playergamelogs.PlayerGameLogs(season_nullable=season_id, season_type_nullable='Playoffs')
        df_playoffs = game_logs_playoffs.get_data_frames()[0]
        all_game_logs = pd.concat([all_game_logs, df_playoffs], ignore_index=True)

    except Exception as e:
        print(f"Error fetching data for season {season_id}: {e}")
        continue

# Define the required columns
required_cols = [
    'PLAYER_NAME', 'TEAM_ABBREVIATION', 'MATCHUP', 'WL', 'MIN', 'FGM', 'FGA',
    'FG_PCT', 'FG3M', 'FG3A', 'FG3_PCT', 'FTM', 'FTA', 'FT_PCT', 'OREB',
    'DREB', 'REB', 'AST', 'STL', 'BLK', 'TOV', 'PF', 'PTS', 'PLUS_MINUS', 'GAME_DATE' # Using PLUS_MINUS as a potential GmSc proxy, or remove if not needed. GAME_DATE for Date.
]

# Rename columns to match the request (adjusting for available NBA API columns)
# Note: 'Res' is 'WL', 'Tm' is 'TEAM_ABBREVIATION', 'Opp' is derived from 'MATCHUP'
# 'GmSc' is not directly available in PlayerGameLogs, using PLUS_MINUS or removing it.
# 'Data' is likely 'GAME_DATE'
column_mapping = {
    'PLAYER_NAME': 'Player',
    'TEAM_ABBREVIATION': 'Tm',
    'MATCHUP': 'Opp_Matchup', # Will extract Opp from this
    'WL': 'Res',
    'MIN': 'MP',
    'FGM': 'FG',
    'FGA': 'FGA',
    'FG_PCT': 'FG%',
    'FG3M': '3P',
    'FG3A': '3PA',
    'FG3_PCT': '3P%',
    'FTM': 'FT',
    'FTA': 'FTA',
    'FT_PCT': 'FT%',
    'OREB': 'ORB',
    'DREB': 'DRB',
    'REB': 'TRB',
    'AST': 'AST',
    'STL': 'STL',
    'BLK': 'BLK',
    'TOV': 'TOV',
    'PF': 'PF',
    'PTS': 'PTS',
    'PLUS_MINUS': 'GmSc_PlusMinus', # Using Plus/Minus as a stand-in or remove
    'GAME_DATE': 'Data'
}


# Select and rename columns
# Ensure all required_cols are in all_game_logs before selecting
actual_cols_available = [col for col in required_cols if col in all_game_logs.columns]
df_final = all_game_logs[actual_cols_available].copy()

# Rename available columns
df_final.rename(columns=column_mapping, inplace=True)

# Extract 'Opp' from 'Opp_Matchup'
# Example: "LAL @ BKN" -> "BKN", "LAL vs BOS" -> "BOS"
if 'Opp_Matchup' in df_final.columns:
    df_final['Opp'] = df_final['Opp_Matchup'].apply(lambda x: x.split(' ')[-1])
    df_final.drop(columns=['Opp_Matchup'], inplace=True)
else:
     df_final['Opp'] = '' # Add an empty 'Opp' column if Matchup isn't available

# Drop the Plus/Minus column if 'GmSc' specifically is required and this isn't suitable
if 'GmSc_PlusMinus' in df_final.columns:
    # Decide if you want to keep Plus/Minus as 'GmSc' or drop it
    df_final.rename(columns={'GmSc_PlusMinus': 'GmSc'}, inplace=True)
    # Or uncomment the line below to drop it if GmSc is strictly Game Score
    # df_final.drop(columns=['GmSc_PlusMinus'], inplace=True)


# Reorder columns to match the request
final_column_order = [
    'Player', 'Tm', 'Opp', 'Res', 'MP', 'FG', 'FGA', 'FG%', '3P', '3PA', '3P%',
    'FT', 'FTA', 'FT%', 'ORB', 'DRB', 'TRB', 'AST', 'STL', 'BLK', 'TOV', 'PF',
    'PTS', 'GmSc', 'Data' # Include 'GmSc' and 'Data' if they exist after processing
]

# Filter the final DataFrame to only include the columns in final_column_order that exist
df_final = df_final[[col for col in final_column_order if col in df_final.columns]]


# Save to CSV
output_filename = 'nba_player_stats_2023_present.csv'
df_final.to_csv(output_filename, index=False)

print(f"Data saved to {output_filename}")

# =========================================================================
# Database update functionality (from replace_game_stats.sql)
# =========================================================================

def update_database():
    """
    Update the database with the new CSV data using SQL commands from replace_game_stats.sql.
    """
    # Define paths
    current_dir = os.path.dirname(os.path.abspath(__file__))
    db_path = os.path.join(current_dir, 'db', 'site.db')
    csv_path = os.path.join(current_dir, output_filename)
    
    # Check if the database file exists
    if not os.path.exists(db_path):
        print(f"Error: Database file not found at {db_path}")
        return False
    
    # Check if the CSV file exists
    if not os.path.exists(csv_path):
        print(f"Error: CSV file not found at {csv_path}")
        return False
    
    try:
        # Connect to the SQLite database
        conn = sqlite3.connect(db_path)
        cursor = conn.cursor()
        
        print("Connected to database. Updating GameStats table...")
        
        # Step 1: Delete all existing data from GameStats
        cursor.execute("DELETE FROM GameStats;")
        
        # Step 2: Reset the autoincrement counter
        cursor.execute("DELETE FROM sqlite_sequence WHERE name='GameStats';")
        
        # Step 3: Create a temporary table for the import
        cursor.execute("""
        CREATE TABLE IF NOT EXISTS temp_import (
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
        """)
        
        # Step 4: Import the CSV data directly using pandas
        df = pd.read_csv(csv_path)
        df.to_sql('temp_import', conn, if_exists='replace', index=False)
        
        # Step 5: Insert data from temp_import into GameStats
        cursor.execute("""
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
            Player, Tm AS Team, Opp AS Opponent, Res AS Result, 
            CAST(MP AS REAL) AS MinutesPlayed,
            FG AS FieldGoalsMade, FGA AS FieldGoalsAttempted, "FG%" AS FieldGoalPercentage,
            "3P" AS ThreePointersMade, "3PA" AS ThreePointersAttempted, "3P%" AS ThreePointPercentage,
            FT AS FreeThrowsMade, FTA AS FreeThrowsAttempted, "FT%" AS FreeThrowPercentage,
            ORB AS OffensiveRebounds, DRB AS DefensiveRebounds, TRB AS TotalRebounds,
            AST AS Assists, STL AS Steals, BLK AS Blocks, TOV AS Turnovers, PF AS PersonalFouls,
            PTS AS Points, GmSc AS GameScore,
            date(Data) as GameDate
        FROM temp_import
        WHERE Player != 'Player';  -- Skip header row if it was imported
        """)
        
        # Step 6: Drop the temporary table
        cursor.execute("DROP TABLE IF EXISTS temp_import;")
        
        # Commit the transaction before VACUUM
        conn.commit()
        
        # Step 7: Vacuum the database to reclaim space (must be outside a transaction)
        conn.execute("VACUUM;")
        
        print("Database successfully updated with new game stats!")
        conn.close()
        return True
        
    except Exception as e:
        print(f"Error updating database: {e}")
        # Make sure to close the connection even if there's an error
        if 'conn' in locals():
            conn.close()
        return False

# Call the database update function after generating the CSV
if __name__ == "__main__":
    print("Starting database update process...")
    update_database()



