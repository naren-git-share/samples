@echo off
echo Starting Client Application...
echo.
echo Checking for available tools...
echo.

cd client

REM Check if npm is available
where npm >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Using npm...
    if not exist "node_modules" (
        echo Installing dependencies...
        call npm install
    )
    npm run dev
    goto :end
)

REM Check if python is available
where python >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Using Python HTTP server...
    echo Opening client on http://localhost:8080
    python -m http.server 8080
    goto :end
)

REM Check if dotnet-serve is available
where dotnet-serve >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Using dotnet-serve...
    dotnet serve -p 8080
    goto :end
)

echo ERROR: No suitable HTTP server found!
echo Please install one of the following:
echo   - Node.js (npm)
echo   - Python
echo   - .NET dotnet-serve tool: dotnet tool install -g dotnet-serve
pause

:end
