using LiteDB;

namespace AI_Chat_API.Models
{
    public class APIConfiguration
    {
        [BsonId]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ApiKey { get; set; }

        public string Model { get; set; }
    }
}
