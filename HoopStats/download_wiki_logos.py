import os
import requests

# Directory where logos will be saved
SAVE_DIR = "wwwroot/img/teams"

# NBA teams and their official NBA.com IDs
# Team names are used for filenames, IDs are used for constructing the logo URL
NBA_TEAMS = {
    'Atlanta Hawks': '1610612737',
    'Boston Celtics': '1610612738',
    'Brooklyn Nets': '1610612751',
    'Charlotte Hornets': '1610612766',
    'Chicago Bulls': '1610612741',
    'Cleveland Cavaliers': '1610612739',
    'Dallas Mavericks': '1610612742',
    'Denver Nuggets': '1610612743',
    'Detroit Pistons': '1610612765',
    'Golden State Warriors': '1610612744',
    'Houston Rockets': '1610612745',
    'Indiana Pacers': '1610612754',
    'LA Clippers': '1610612746',
    'Los Angeles Lakers': '1610612747',
    'Memphis Grizzlies': '1610612763',
    'Miami Heat': '1610612748',
    'Milwaukee Bucks': '1610612749',
    'Minnesota Timberwolves': '1610612750',
    'New Orleans Pelicans': '1610612740',
    'New York Knicks': '1610612752',
    'Oklahoma City Thunder': '1610612760',
    'Orlando Magic': '1610612753',
    'Philadelphia 76ers': '1610612755',
    'Phoenix Suns': '1610612756',
    'Portland Trail Blazers': '1610612757',
    'Sacramento Kings': '1610612758',
    'San Antonio Spurs': '1610612759',
    'Toronto Raptors': '1610612761',
    'Utah Jazz': '1610612762',
    'Washington Wizards': '1610612764',
}

# Base URL for the primary SVG logos on NBA.com
# The structure is usually https://cdn.nba.com/logos/nba/{TEAM_ID}/primary/L/logo.svg
LOGO_URL_TEMPLATE = "https://cdn.nba.com/logos/nba/{team_id}/primary/L/logo.svg"

def download_nba_logos():
    """
    Downloads NBA team logos and saves them to the specified directory.
    """
    # Create the save directory if it doesn't exist
    # The exist_ok=True argument prevents an error if the directory already exists
    try:
        os.makedirs(SAVE_DIR, exist_ok=True)
        print(f"Ensured directory exists: {SAVE_DIR}")
    except OSError as e:
        print(f"Error creating directory {SAVE_DIR}: {e}")
        return # Exit if directory creation fails

    print(f"\nStarting download of {len(NBA_TEAMS)} NBA team logos...")

    for team_name, team_id in NBA_TEAMS.items():
        logo_url = LOGO_URL_TEMPLATE.format(team_id=team_id)
        
        # Sanitize team name for use in filename (replace spaces with underscores, lowercase)
        # Example: "Atlanta Hawks" -> "atlanta_hawks_logo.svg"
        file_name = f"{team_name.replace(' ', '_').lower()}_logo.svg"
        file_path = os.path.join(SAVE_DIR, file_name)

        print(f"\nAttempting to download logo for: {team_name}")
        print(f"  URL: {logo_url}")
        print(f"  Saving to: {file_path}")

        try:
            # Send an HTTP GET request to the URL
            response = requests.get(logo_url, timeout=10) # Added timeout
            response.raise_for_status()  # Raise an exception for HTTP errors (4xx or 5xx)

            # Write the content of the response (the image) to a file
            with open(file_path, 'wb') as f:
                f.write(response.content)
            print(f"  SUCCESS: Logo for {team_name} downloaded to {file_path}")

        except requests.exceptions.HTTPError as http_err:
            print(f"  ERROR: HTTP error occurred while downloading {team_name} logo: {http_err}")
        except requests.exceptions.ConnectionError as conn_err:
            print(f"  ERROR: Connection error occurred while downloading {team_name} logo: {conn_err}")
        except requests.exceptions.Timeout as timeout_err:
            print(f"  ERROR: Timeout occurred while downloading {team_name} logo: {timeout_err}")
        except requests.exceptions.RequestException as req_err:
            print(f"  ERROR: An unexpected error occurred while downloading {team_name} logo: {req_err}")
        except IOError as io_err:
            print(f"  ERROR: Could not write file {file_path}: {io_err}")

    print("\nFinished downloading all available logos.")

if __name__ == "__main__":
    # This block ensures the download_nba_logos function is called only when the script is executed directly
    # (not when imported as a module).
    download_nba_logos()
