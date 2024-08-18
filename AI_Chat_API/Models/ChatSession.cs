using LiteDB;

namespace AI_Chat_API.Models
{
    public class ChatSession
    {
        [BsonId]
        public int Id { get; set; }

        public string SessionName { get; set; } // A name to identify the chat session
        public DateTime CreatedAt { get; set; } // Timestamp for when the session was created
        public int ApiConfigurationId { get; set; } // The associated API configuration ID
        public string llmDirections { get; set; } = string.Empty; // Directions for the LLM

    }
}
