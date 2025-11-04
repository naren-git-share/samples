# Server-Side Events Demo

A complete real-time application demonstrating Server-Side Events (SSE) with ASP.NET Core and Vue.js.

## Quick Start

### 1. Run the Application
```powershell
cd server
dotnet run
```

The application serves both the API and the client from a single server.

### 2. Open in Browser
Navigate to: http://localhost:5105

Both the client UI and API are served from the same application.

## What You'll See

1. **Login Page**: Enter your username
2. **Dashboard**: 
   - 26 tiles (A-Z) with numbers counting down in real-time
   - Live chat panel to communicate with other users
   - Connection status indicator

## Technology Stack

- **Backend**: ASP.NET Core Web API (.NET 9)
- **Frontend**: Vue.js 3 (via CDN)
- **Real-time**: Server-Side Events (SSE)
- **Styling**: Custom CSS with gradients

## Features

✅ Real-time number countdown (26 letters A-Z)  
✅ Live chat with instant broadcasting  
✅ Last 100 messages stored in memory  
✅ Auto-reconnection on disconnect  
✅ Multiple simultaneous users  
✅ Clean, responsive UI  

## Documentation

See [instructions.md](./instructions.md) for detailed implementation notes, architecture, and API documentation.

## Demo

Open multiple browser windows to see real-time synchronization of:
- Number countdowns across all clients
- Chat messages broadcasting to all users instantly

## Deploy to Azure

### 🚀 Automated Deployment with GitHub Actions

This repository includes a GitHub Actions workflow for automated deployment.

**Recommended Approach:**

1. Push your code to GitHub
2. Create an App Service in Azure Portal
3. Use **Deployment Center** in Azure Portal to connect to GitHub
4. Azure will automatically set up the GitHub Actions workflow
5. Every push to main branch triggers automatic deployment!

**See [GITHUB_ACTIONS_SETUP.md](./GITHUB_ACTIONS_SETUP.md) for detailed step-by-step instructions.**

### Requirements
- Azure subscription  
- GitHub repository
- Globally unique app name for Azure App Service
