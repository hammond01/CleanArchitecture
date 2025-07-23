@echo off
echo =================================
echo Order Management Database Migrator
echo =================================
echo.

if "%1"=="" (
    echo Running migration with default settings...
    dotnet run migrate
) else (
    echo Running command: %1
    dotnet run %1
)

echo.
echo Press any key to exit...
pause > nul
