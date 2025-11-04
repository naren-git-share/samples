using System.Collections.Concurrent;
using System.Threading.Channels;
using ServerSideEventsApi.Models;

namespace ServerSideEventsApi.Services;

public class NumbersService
{
    private readonly Dictionary<char, int> _numbers = new();
    private readonly Random _random = new();
    private readonly object _lock = new();
    private Timer? _timer;
    private readonly List<Channel<Dictionary<char, int>>> _subscribers = new();

    public NumbersService()
    {
        InitializeNumbers();
        StartTimer();
    }

    private void InitializeNumbers()
    {
        lock (_lock)
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                _numbers[c] = _random.Next(100000, 500001);
            }
        }
    }

    private void StartTimer()
    {
        _timer = new Timer(UpdateNumbers, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void UpdateNumbers(object? state)
    {
        Dictionary<char, int> currentNumbers;
        
        lock (_lock)
        {
            foreach (var key in _numbers.Keys.ToList())
            {
                if (_numbers[key] > 0)
                {
                    var decrease = _random.Next(1, 10);
                    _numbers[key] = Math.Max(0, _numbers[key] - decrease);
                }
            }
            
            currentNumbers = new Dictionary<char, int>(_numbers);
        }

        // Notify all subscribers
        lock (_subscribers)
        {
            foreach (var channel in _subscribers)
            {
                channel.Writer.TryWrite(currentNumbers);
            }
        }
    }

    public Dictionary<char, int> GetCurrentNumbers()
    {
        lock (_lock)
        {
            return new Dictionary<char, int>(_numbers);
        }
    }

    public Channel<Dictionary<char, int>> Subscribe()
    {
        var channel = Channel.CreateUnbounded<Dictionary<char, int>>();
        
        lock (_subscribers)
        {
            _subscribers.Add(channel);
        }

        // Send current state immediately
        var currentNumbers = GetCurrentNumbers();
        channel.Writer.TryWrite(currentNumbers);

        return channel;
    }

    public void Unsubscribe(Channel<Dictionary<char, int>> channel)
    {
        lock (_subscribers)
        {
            _subscribers.Remove(channel);
        }
    }
}
