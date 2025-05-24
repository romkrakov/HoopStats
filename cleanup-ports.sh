#!/bin/zsh

# Script to clean up any processes using the ports needed by the application
# Handles paths with spaces and special characters

# Configuration - add any ports your application uses
PORTS=(5162 5163 7165 7166)

echo "HoopStats Port Cleanup Utility"
echo "=============================="
echo "Checking for processes using application ports..."

for PORT in "${PORTS[@]}"; do
    # Get the PID of any process using this port
    PID=$(lsof -ti :$PORT)
    
    if [ ! -z "$PID" ]; then
        echo "Found process $PID using port $PORT. Stopping it..."
        kill -9 $PID
        echo "Process on port $PORT stopped."
    else
        echo "No process is using port $PORT."
    fi
done

echo "Port cleanup complete."
echo "You can now run the application."
