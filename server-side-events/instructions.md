# Server-Side Events (SSE) Demo Application

A real-time web application demonstrating Server-Side Events (SSE) using ASP.NET Core Web API (.NET 9) and Vue.js.

## 🎯 Features

### Server (ASP.NET Core Web API)
- **Real-time Number Streaming**: 26 alphabet letters (A-Z), each with a random starting value (100,000-500,000) that decreases by 1-9 every second via SSE
- **Real-time Chat**: Broadcast messages to all connected clients instantly
- **In-Memory Storage**: Maintains last 100 chat messages (no database required)
- **CORS Enabled**: Allows cross-origin requests from the client

### Client (Vue.js)
- **Username Entry**: Simple login page to capture username
- **Live Dashboard**: 26 animated tiles displaying real-time countdown numbers
- **Real-time Chat Panel**: Side panel for posting and receiving messages
- **Auto-reconnection**: Automatically reconnects if SSE connection drops
- **Responsive Design**: Clean, minimal CSS with gradient backgrounds

## 📁 Project Structure

```
server-side-events/
├── server/                          # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── NumbersController.cs    # SSE stream for numbers + current state endpoint
│   │   └── ChatController.cs       # SSE stream for chat + POST message endpoint
│   ├── Models/
│   │   ├── AlphabetNumber.cs       # Model for alphabet-number pairs
│   │   └── ChatMessage.cs          # Model for chat messages
│   ├── Services/
│   │   ├── NumbersService.cs       # Singleton service managing number countdown
│   │   └── ChatService.cs          # Singleton service managing chat broadcasts
│   ├── Program.cs                  # Application configuration & startup
│   └── ServerSideEventsApi.csproj  # Project file
│
├── client/                          # Vue.js Client
│   ├── index.html                  # Login page (username entry)
│   └── details.html                # Dashboard page (tiles + chat)
│
└── instructions.md                  # This file

```

## 🏗️ Implementation Details

### Server Architecture

#### 1. **NumbersService.cs**
- Singleton service that maintains 26 alphabet-number pairs
- Uses `System.Threading.Timer` to update numbers every second
- Implements publisher-subscriber pattern using `System.Threading.Channels`
- Each number decreases by random value (1-9) and stops at 0
- Broadcasts updates to all connected SSE clients

#### 2. **ChatService.cs**
- Singleton service managing chat message broadcasting
- Maintains `ConcurrentQueue` of last 100 messages
- Uses `System.Threading.Channels` for real-time message distribution
- New subscribers receive message history immediately

#### 3. **Controllers**
- **NumbersController**:
  - `GET /api/numbers/stream` - SSE endpoint streaming number updates
  - `GET /api/numbers/current` - Returns current snapshot of all numbers
  
- **ChatController**:
  - `GET /api/chat/stream` - SSE endpoint streaming chat messages
  - `POST /api/chat/message` - Accepts new messages to broadcast
  - `GET /api/chat/history` - Returns last 100 messages

#### 4. **SSE Implementation**
```csharp
// Set SSE headers
Response.Headers.Append("Content-Type", "text/event-stream");
Response.Headers.Append("Cache-Control", "no-cache");
Response.Headers.Append("Connection", "keep-alive");

// Stream data in SSE format
var data = $"data: {json}\n\n";
await Response.Body.WriteAsync(bytes, cancellationToken);
await Response.Body.FlushAsync(cancellationToken);
```

### Client Architecture

#### 1. **index.html** (Login Page)
- Simple Vue.js app with username input validation
- Stores username in `localStorage`
- Redirects to dashboard after validation

#### 2. **details.html** (Dashboard)
- **Tiles Section**: 
  - Grid layout displaying 26 alphabet tiles
  - Reactive updates when new data arrives via SSE
  - Visual indicator when value reaches 0
  
- **Chat Panel**:
  - Displays incoming messages with username and timestamp
  - Input field for sending new messages
  - Auto-scrolls to latest message
  - Connection status indicator

#### 3. **EventSource API**
```javascript
// Connect to SSE streams
const eventSource = new EventSource('http://localhost:5000/api/numbers/stream');

eventSource.onmessage = (event) => {
    const data = JSON.parse(event.data);
    // Update Vue reactive state
};

eventSource.onerror = (error) => {
    // Handle reconnection
};
```

## 🚀 How to Run

### Prerequisites
- .NET 9 SDK
- Modern web browser (Chrome, Firefox, Edge)
- **One of the following for client HTTP server**:
  - Node.js with npm (recommended for best dev experience)
  - Python 3.x
  - .NET dotnet-serve tool

### Step 1: Start the Server

```powershell
# Navigate to server folder
cd server

# Restore dependencies (if needed)
dotnet restore

# Run the API (will run on http://localhost:5000)
dotnet run
```

The API will start on `http://localhost:5000` (or `https://localhost:5001` for HTTPS).

**Note**: If the API runs on a different port, update the `API_BASE_URL` in `client/details.html`:
```javascript
const API_BASE_URL = 'http://localhost:5000/api';
```

### Step 2: Start the Client

**Option A: Using npm (Recommended - requires Node.js)**
```powershell
# Navigate to client folder
cd client

# Install dependencies (first time only)
npm install

# Start development server with Vite
npm run dev
```

**Option B: Using Python**
```powershell
cd client
python -m http.server 8080
```

**Option C: Using .NET dotnet-serve**
```powershell
cd client
dotnet tool install -g dotnet-serve  # First time only
dotnet serve -p 8080
```

The client will be available at `http://localhost:8080`.

### Step 3: Use the Application

1. Open browser to `http://localhost:8080` (or the file directly)
2. Enter your username and click "Enter Dashboard"
3. View the live countdown of 26 numbers
4. Open multiple browser windows/tabs to test real-time chat
5. Send messages and see them appear instantly in all windows

## 🔧 Configuration

### Server Configuration

**CORS Policy** (`Program.cs`):
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**Port Configuration** (`Properties/launchSettings.json`):
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Client Configuration

Update API endpoint in `details.html` if server port changes:
```javascript
const API_BASE_URL = 'http://localhost:5000/api';
```

## 📊 API Endpoints

### Numbers API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/numbers/stream` | SSE stream of number updates (every 1 second) |
| GET | `/api/numbers/current` | Current snapshot of all 26 numbers |

### Chat API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/chat/stream` | SSE stream of chat messages |
| POST | `/api/chat/message` | Post a new message |
| GET | `/api/chat/history` | Get last 100 messages |

**POST /api/chat/message Request Body:**
```json
{
  "userName": "John",
  "message": "Hello World!"
}
```

## 🎨 Features Implemented

✅ ASP.NET Core Web API on .NET 9  
✅ Server-Side Events (SSE) for real-time updates  
✅ 26 alphabet numbers with random starting values (100,000-500,000)  
✅ Numbers decrease by random value (1-9) every second  
✅ Numbers stop at 0 (don't go negative)  
✅ Real-time chat with broadcast to all users  
✅ Last 100 messages stored in memory  
✅ Vue.js client with username entry  
✅ Dashboard with 26 animated tiles  
✅ Side chat panel with message input  
✅ Auto-reconnection on connection loss  
✅ Clean, minimal CSS design  
✅ CORS configured for cross-origin requests  

## 🐛 Troubleshooting

**Issue**: Client can't connect to server
- Ensure server is running on `http://localhost:5000`
- Check that CORS is enabled in `Program.cs`
- Verify `API_BASE_URL` in `details.html` matches server URL

**Issue**: SSE connection drops
- Normal behavior; client will auto-reconnect after 5 seconds
- Check browser console for error messages

**Issue**: Messages not appearing
- Verify POST request succeeds (check Network tab)
- Ensure multiple windows are open to test broadcasting
- Check server console for any errors

## 📝 Notes

- No authentication implemented (username stored in localStorage only)
- Messages are not persisted (lost on server restart)
- Numbers reset on server restart
- Designed for development/demo purposes
- For production, consider adding authentication, database persistence, and rate limiting


