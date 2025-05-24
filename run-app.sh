#!/bin/zsh

# Script to run the HoopStats application
# Handles paths with spaces and special characters
# Kills any existing process using the same port and starts the application

# Get the absolute path of the script's directory
SCRIPT_DIR=$(cd "$(dirname "$0")" && pwd)
PROJECT_DIR="${SCRIPT_DIR}/HoopStats"

# Configuration
HTTP_PORT=5163
HTTPS_PORT=7166

# Function to check and kill processes using a specific port
kill_process_on_port() {
    local port=$1
    echo "Checking for processes using port $port..."
    
    # Get the PID of any process using this port
    local pid=$(lsof -ti :$port)
    
    if [ ! -z "$pid" ]; then
        echo "Found process $pid using port $port. Stopping it..."
        kill -9 $pid
        echo "Process stopped."
    else
        echo "No process is using port $port."
    fi
}

# Kill any existing processes using our ports
kill_process_on_port $HTTP_PORT
kill_process_on_port $HTTPS_PORT

# Navigate to the project directory
echo "Changing to project directory: ${PROJECT_DIR}"
cd "${PROJECT_DIR}"

# Run the application
echo "Starting HoopStats application..."
echo "HTTP: http://localhost:$HTTP_PORT"
echo "HTTPS: https://localhost:$HTTPS_PORT"
echo ""
echo "Press Ctrl+C to stop the application"

# Use environment variables to override the ports from launchSettings.json
export ASPNETCORE_URLS="http://localhost:$HTTP_PORT;https://localhost:$HTTPS_PORT"

# Start the application with hot reload
dotnet watch run
