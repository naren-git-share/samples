using System.Collections.Concurrent;
using System.Threading.Channels;
using ServerSideEventsApi.Models;

namespace ServerSideEventsApi.Services;

public class ChatService
{
    private readonly ConcurrentQueue<ChatMessage> _messageHistory = new();
    private readonly List<Channel<ChatMessage>> _subscribers = new();
    private readonly object _lock = new();
    private const int MaxHistorySize = 100;

    public void AddMessage(ChatMessage message)
    {
        // Add to history
        _messageHistory.Enqueue(message);
        
        // Keep only last 100 messages
        while (_messageHistory.Count > MaxHistorySize)
        {
            _messageHistory.TryDequeue(out _);
        }

        // Broadcast to all subscribers
        lock (_lock)
        {
            foreach (var channel in _subscribers)
            {
                channel.Writer.TryWrite(message);
            }
        }
    }

    public IEnumerable<ChatMessage> GetMessageHistory()
    {
        return _messageHistory.ToArray();
    }

    public Channel<ChatMessage> Subscribe()
    {
        var channel = Channel.CreateUnbounded<ChatMessage>();
        
        lock (_lock)
        {
            _subscribers.Add(channel);
        }

        // Send message history to new subscriber
        foreach (var message in GetMessageHistory())
        {
            channel.Writer.TryWrite(message);
        }

        return channel;
    }

    public void Unsubscribe(Channel<ChatMessage> channel)
    {
        lock (_lock)
        {
            _subscribers.Remove(channel);
        }
    }
}
