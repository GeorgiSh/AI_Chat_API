using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AI_Chat_API.Models;
using AI_Chat_API.Services;
using Microsoft.Maui.Controls;

namespace AI_Chat_API.ViewModels
{
    public class ChatSessionViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly ApiService _apiService;
        private string _sessionName;
        private string _newMessage;
        private string _llmDirections;
        private APIConfiguration _selectedApiConfig;
        private string _selectedModel;

        public ObservableCollection<ChatMessage> ChatMessages { get; }
        public ObservableCollection<APIConfiguration> ApiConfigurations { get; }
        public ObservableCollection<string> AvailableModels { get; }

        public ChatSession Session { get; }
        public string SessionName
        {
            get => _sessionName;
            set
            {
                _sessionName = value;
                OnPropertyChanged();
            }
        }

        public string NewMessage
        {
            get => _newMessage;
            set
            {
                _newMessage = value;
                OnPropertyChanged();
            }
        }

        public string llmDirections
        {
            get => _llmDirections;
            set
            {
                _llmDirections = value;
                OnPropertyChanged();
            }
        }

        public APIConfiguration SelectedApiConfig
        {
            get => _selectedApiConfig;
            set
            {
                _selectedApiConfig = value;
                OnPropertyChanged();
                LoadModelsForApiAsync();
            }
        }

        public string SelectedModel
        {
            get => _selectedModel;
            set
            {
                _selectedModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand EditSessionNameCommand { get; }
        public ICommand EditDirectionsCommand { get; }
        public ICommand DeleteSessionCommand { get; }

        public ChatSessionViewModel(ChatSession session, DatabaseService databaseService)
        {
            Session = session;
            _databaseService = databaseService;
            _apiService = new ApiService();
            _sessionName = session.SessionName;
            _llmDirections = session.llmDirections ?? string.Empty; // Initialize as empty string if null
            ChatMessages = new ObservableCollection<ChatMessage>();
            ApiConfigurations = new ObservableCollection<APIConfiguration>();
            AvailableModels = new ObservableCollection<string>();

            SendMessageCommand = new Command(async () => await SendMessageAsync());
            EditSessionNameCommand = new Command(async () => await EditSessionNameAsync());
            EditDirectionsCommand = new Command(async () => await EditDirectionsAsync());
            DeleteSessionCommand = new Command(async () => await DeleteSessionAsync());

            LoadChatMessagesAsync();
            LoadApiConfigurationsAsync();
        }

        private async Task DeleteSessionAsync()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Delete Session", "Are you sure you want to delete this session?", "Yes", "No");
            if (confirm)
            {
                await _databaseService.DeleteChatSessionAsync(Session.Id);

                // Navigate back to the main page after deletion
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        private async Task LoadChatMessagesAsync()
        {
            var messages = await _databaseService.GetChatMessagesForChatAsync(Session.Id);
            foreach (var message in messages)
            {
                ChatMessages.Add(message);
            }
        }

        private async Task LoadApiConfigurationsAsync()
        {
            var configs = await _databaseService.GetAllApiConfigurationsAsync();
            foreach (var config in configs)
            {
                ApiConfigurations.Add(config);
            }
        }

        private async Task LoadModelsForApiAsync()
        {
            AvailableModels.Clear();
            if (SelectedApiConfig != null)
            {
                var models = await _apiService.FetchModelsAsync(SelectedApiConfig.Url, SelectedApiConfig.ApiKey);
                foreach (var model in models)
                {
                    AvailableModels.Add(model);
                }
            }
        }

        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(NewMessage) || SelectedApiConfig == null || string.IsNullOrWhiteSpace(SelectedModel))
                return;

            var userMessage = new ChatMessage
            {
                Sender = "User",
                Message = NewMessage,
                Timestamp = DateTime.Now,
                ChatSessionId = Session.Id
            };
            await _databaseService.AddChatMessageAsync(userMessage);
            ChatMessages.Add(userMessage);

            NewMessage = string.Empty;

            var botResponse = await _apiService.SendMessageAsync(userMessage.Message, llmDirections, SelectedModel, SelectedApiConfig);

            var botMessage = new ChatMessage
            {
                Sender = "Bot",
                Message = botResponse,
                Timestamp = DateTime.Now,
                ChatSessionId = Session.Id
            };

            await _databaseService.AddChatMessageAsync(botMessage);
            ChatMessages.Add(botMessage);
        }

        private async Task EditSessionNameAsync()
        {
            string result = await App.Current.MainPage.DisplayPromptAsync("Edit Session Name", "Enter new session name:", initialValue: SessionName);
            if (!string.IsNullOrWhiteSpace(result))
            {
                SessionName = result;
                Session.SessionName = result;
                await _databaseService.UpdateChatSessionAsync(Session);
            }
        }

        private async Task EditDirectionsAsync()
        {
            string result = await App.Current.MainPage.DisplayPromptAsync("Edit LLM Directions", "Enter LLM directions:", initialValue: llmDirections);
            if (!string.IsNullOrWhiteSpace(result))
            {
                llmDirections = result;
                Session.llmDirections = result;
                await _databaseService.UpdateChatSessionAsync(Session);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
