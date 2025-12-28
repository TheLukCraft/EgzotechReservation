# Check if .env exists

if [ ! -f .env ]; then
    echo -e "File .env not found. Creating from .env.example"
    
    if [ -f .env.example ]; then
        cp .env.example .env
        echo -e "Created .env"
    else
        echo -e "Error: .env.example does not exist! Cannot create configuration."
        exit 1
    fi
else
    echo -e "Configuration (.env) found."
fi

# Run application

echo - e "Building and starting the application."
docker-compose up -d --build

# Check if application started successfully

if [ $? -eq 0 ]; then
    echo -e "Application started successfully."
    read -p "Press ENTER to close."
else
    echo -e "Error: Application failed to start."
    exit 1
fi

read -p "Press ENTER to close."