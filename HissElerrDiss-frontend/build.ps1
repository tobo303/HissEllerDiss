Write-Host "Starting build"
$ErrorActionPreference = "Stop"
docker build -t tobo303/hissellerdiss-frontend:latest .
