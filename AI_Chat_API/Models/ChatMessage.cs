using LiteDB;

namespace AI_Chat_API.Models
{
    public class ChatMessage
    {
        [BsonId]
        public int Id { get; set; }

        public int ChatSessionId { get; set; } // ID of the chat session this message belongs to
        public string Sender { get; set; } // Could be "User" or "Bot"
        public string Message { get; set; } // The content of the message
        public DateTime Timestamp { get; set; } // When the message was sent

    }
}
