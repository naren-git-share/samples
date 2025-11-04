# Client Application

This is a simple Vue.js client for the Server-Side Events demo.

## Files
- `index.html` - Login page where users enter their username
- `details.html` - Main dashboard with live tiles and chat

## Running the Client

### Option 1: Using npm (Recommended)
```bash
# Install dependencies (first time only)
npm install

# Start development server
npm run dev
```

### Option 2: Using Python
```bash
python -m http.server 8080
```

### Option 3: Using .NET dotnet-serve
```bash
# Install dotnet-serve (first time only)
dotnet tool install -g dotnet-serve

# Start server
dotnet serve -p 8080
```

The client will be available at http://localhost:8080

## Configuration

If your server runs on a different port, update the `API_BASE_URL` in `details.html`:

```javascript
const API_BASE_URL = 'http://localhost:YOUR_PORT/api';
```

## Usage
1. Open http://localhost:8080
2. Enter your username
3. Click "Enter Dashboard"
4. View live countdown numbers and chat with other users
