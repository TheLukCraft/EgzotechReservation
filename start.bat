@echo off

:: Check if .env exists
if not exist .env (
    echo File .env not found. Creating from .env.example
    
    if exist .env.example (
        copy .env.example .env >nul
        echo Created .env
    ) else (
        echo Error: .env.example does not exist! Cannot create configuration.
        pause
        exit /b 1
    )
) else (
    echo Configuration ^(.env^) found.
)

:: Run application
echo Building and starting the application.
docker-compose up -d --build

:: Check if application started successfully
if %ERRORLEVEL% EQU 0 (
    echo Application started successfully.
) else (
    echo Error: Application failed to start.
    pause
    exit /b 1
)