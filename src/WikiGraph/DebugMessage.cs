using System;

namespace WikiGraph
{
    public enum MessageType
    {
        Informational,
        Error
    }

    public class DebugMessage
    {
        public MessageType Type { get; private set; }
        public string Message { get; private set; }

        public DebugMessage(string message, MessageType type)
        {
            Message = message;
            Type = type;
        }
    }
}