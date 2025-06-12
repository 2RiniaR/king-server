# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build and Run
- `dotnet run` - Run the application locally
- `make run` - Alternative way to run the application using Makefile

### Database Migrations
- `make migrate name=MigrationName` - Create and apply a new Entity Framework migration
- `dotnet ef migrations add <name>` - Create a new migration
- `dotnet ef database update` - Apply pending migrations

### Docker Deployment
- `make push app_name=<docker_user_id>/king-server` - Build and push Docker image
- `docker build . -t <docker_user_id>/king-server` - Build Docker image
- `docker push <docker_user_id>/king-server` - Push Docker image to registry

## Architecture Overview

This is a Discord bot application built with .NET 8 and Entity Framework Core that manages gacha (lottery) and slot game systems.

### Core Components

**Program.cs**: Main entry point that initializes all modules, registers Discord event handlers, and runs the application indefinitely.

**AppService**: Entity Framework DbContext that manages database operations for Users, Gachas, Slots, and GachaItems. Uses SQLite as the database.

**Manager Classes** (in Common/):
- `DiscordManager`: Handles Discord client initialization and message processing
- `MasterManager`: Manages master data tables (settings, trigger phrases, slot items, etc.)
- `SchedulerManager`: Handles scheduled tasks (daily/monthly resets, slot condition refresh)
- `TimeManager`: Manages time-related operations and reset schedules

### Event System

Events are organized by feature in the `Events/` directory:
- **Gacha/**: Gacha commands, rankings, rare replies, and interactions
- **Slot/**: Slot execution, rankings, and condition refresh
- **Admin/**: Administrative commands like master data reload
- **Marugame/**: Special "Marugame" trigger functionality
- **DailyReset/MonthlyReset/**: Scheduled reset operations

### Data Models

Located in `Common/Models/`:
- `User`: Discord user data and game statistics
- `Gacha`: Gacha configuration and probability settings
- `GachaItem`: Individual items available in gacha
- `Slot`: Slot game configuration and state

### Message Processing Flow

1. Discord message received
2. Check if bot is mentioned or trigger phrases match
3. Route to appropriate presenter based on content
4. Execute business logic and respond via Discord

### Configuration

- Environment-specific settings in `Environment/appsettings*.json`
- Google Sheets integration for master data
- SQLite database with Entity Framework migrations
- Docker containerization with timezone set to Asia/Tokyo