using LiteDB;
using AI_Chat_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_Chat_API.Services
{
    public class DatabaseService
    {
        private readonly LiteDatabase _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AI_Chat_API.db");
            _database = new LiteDatabase(dbPath);

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            _database.GetCollection<APIConfiguration>("apiConfigurations");
            _database.GetCollection<ChatSession>("chatSessions");
            _database.GetCollection<ChatMessage>("chatMessages");
        }

        // API Configuration CRUD
        public async Task AddApiConfigurationAsync(APIConfiguration config) => await Task.Run(() => _database.GetCollection<APIConfiguration>("apiConfigurations").Insert(config));
        public async Task UpdateApiConfigurationAsync(APIConfiguration config) => await Task.Run(() => _database.GetCollection<APIConfiguration>("apiConfigurations").Update(config));
        public async Task DeleteApiConfigurationAsync(int id) => await Task.Run(() => _database.GetCollection<APIConfiguration>("apiConfigurations").Delete(id));
        public async Task<IEnumerable<APIConfiguration>> GetAllApiConfigurationsAsync() => await Task.Run(() => _database.GetCollection<APIConfiguration>("apiConfigurations").FindAll());

        // Chat Session CRUD
        public async Task AddChatSessionAsync(ChatSession session) => await Task.Run(() => _database.GetCollection<ChatSession>("chatSessions").Insert(session));
        public async Task<ChatSession> GetChatSessionByIdAsync(int id) => await Task.Run(() => _database.GetCollection<ChatSession>("chatSessions").FindById(id));
        public async Task<IEnumerable<ChatSession>> GetAllChatSessionsAsync() => await Task.Run(() => _database.GetCollection<ChatSession>("chatSessions").FindAll());
        public async Task DeleteChatSessionAsync(int id) => await Task.Run(() => _database.GetCollection<ChatSession>("chatSessions").Delete(id));
        public async Task UpdateChatSessionAsync(ChatSession session) => await Task.Run(() => _database.GetCollection<ChatSession>("chatSessions").Update(session));



        // Chat Message CRUD
        public async Task AddChatMessageAsync(ChatMessage message) => await Task.Run(() => _database.GetCollection<ChatMessage>("chatMessages").Insert(message));
        public async Task<IEnumerable<ChatMessage>> GetChatMessagesForChatAsync(int chatSessionId) => await Task.Run(() => _database.GetCollection<ChatMessage>("chatMessages").Find(x => x.ChatSessionId == chatSessionId));
    }
}
