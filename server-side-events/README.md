# Server-Side Events Demo

A complete real-time application demonstrating Server-Side Events (SSE) with ASP.NET Core and Vue.js.

## Quick Start

### 1. Start the Server (Terminal 1)
```powershell
# Windows
.\start-server.bat

# Or manually
cd server
dotnet run
```

### 2. Start the Client (Terminal 2)

**Option A: Using npm (if Node.js is installed)**
```powershell
cd client
npm install  # First time only
npm run dev
```

**Option B: Using Python**
```powershell
cd client
python -m http.server 8080
```

**Option C: Using .NET**
```powershell
cd client
dotnet tool install -g dotnet-serve
dotnet serve -p 8080
```

### 3. Open in Browser
Navigate to: http://localhost:8080

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
