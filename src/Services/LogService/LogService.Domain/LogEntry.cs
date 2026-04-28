using System;

namespace LogService.Domain;

public sealed class LogEntry
{
    private LogEntry()
    {
    }

    private LogEntry(
        Guid id,
        string serviceName,
        LogLevel level,
        string message,
        string? exception,
        string? traceId,
        string? path,
        string? method)
    {
        Id = id;
        ServiceName = serviceName;
        Level = level;
        Message = message;
        Exception = exception;
        TraceId = traceId;
        Path = path;
        Method = method;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string ServiceName { get; private set; } = string.Empty;

    public LogLevel Level { get; private set; }

    public string Message { get; private set; } = string.Empty;

    public string? Exception { get; private set; }

    public string? TraceId { get; private set; }

    public string? Path { get; private set; }

    public string? Method { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public static LogEntry Create(
        string serviceName,
        LogLevel level,
        string message,
        string? exception,
        string? traceId,
        string? path,
        string? method)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Service name is required.", nameof(serviceName));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message is required.", nameof(message));
        }

        return new LogEntry(
            Guid.NewGuid(),
            serviceName.Trim(),
            level,
            message.Trim(),
            exception,
            traceId,
            path,
            method);
    }
}