# Start both Server and Client in separate windows

Write-Host "Starting Server-Side Events Demo Application..." -ForegroundColor Green
Write-Host ""

# Start Server in new window
Write-Host "Starting Server..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\server'; Write-Host 'Server running on http://localhost:5000' -ForegroundColor Green; dotnet run"

# Wait a bit for server to start
Start-Sleep -Seconds 2

# Start Client in new window
Write-Host "Starting Client..." -ForegroundColor Cyan

# Check which tool is available and use it
$clientCommand = ""
if (Get-Command npm -ErrorAction SilentlyContinue) {
    $clientCommand = "if (!(Test-Path 'node_modules')) { npm install }; npm run dev"
} elseif (Get-Command python -ErrorAction SilentlyContinue) {
    $clientCommand = "python -m http.server 8080"
} elseif (Get-Command dotnet-serve -ErrorAction SilentlyContinue) {
    $clientCommand = "dotnet serve -p 8080"
} else {
    Write-Host "ERROR: No suitable HTTP server found!" -ForegroundColor Red
    Write-Host "Please install Node.js, Python, or run: dotnet tool install -g dotnet-serve" -ForegroundColor Yellow
    exit
}

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\client'; Write-Host 'Client running on http://localhost:8080' -ForegroundColor Green; $clientCommand"

# Wait a bit for client to start
Start-Sleep -Seconds 2

# Open browser
Write-Host "Opening browser..." -ForegroundColor Cyan
Start-Process "http://localhost:8080"

Write-Host ""
Write-Host "Application started successfully!" -ForegroundColor Green
Write-Host "Server: http://localhost:5000" -ForegroundColor Yellow
Write-Host "Client: http://localhost:8080" -ForegroundColor Yellow
Write-Host ""
Write-Host "Close the PowerShell windows to stop the application." -ForegroundColor Gray
