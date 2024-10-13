using System;
using UnityEngine;

namespace AwakeComponents.Log
{
    class LogEntry
    {
        public DateTime Time { get; }
        public string Message;
        public string StackTrace { get; }
        public LogType Type { get; }
            
        public LogEntry(DateTime time, string message, string stackTrace, LogType type)
        {
            Time = time;
            Message = message;
            StackTrace = stackTrace;
            Type = type;
        }
    }
}