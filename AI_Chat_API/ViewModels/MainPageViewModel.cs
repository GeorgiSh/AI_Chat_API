using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AI_Chat_API.Models;
using AI_Chat_API.Services;
using AI_Chat_API.Views;
using Microsoft.Maui.Controls;

namespace AI_Chat_API.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private ChatSession _selectedSession;

        public ObservableCollection<ChatSession> ChatSessions { get; }
        public ChatSession SelectedSession
        {
            get => _selectedSession;
            set
            {
                _selectedSession = value;
                OnPropertyChanged();
                OnSessionSelected(_selectedSession);
            }
        }


        public ICommand AddSessionCommand { get; }
        public ICommand SelectSessionCommand { get; }
        public ICommand GoToSettingsCommand { get; }

        public MainPageViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            ChatSessions = new ObservableCollection<ChatSession>();

            AddSessionCommand = new Command(async () => await AddChatSessionAsync());
            SelectSessionCommand = new Command<ChatSession>(OnSessionSelected);
            GoToSettingsCommand = new Command(async () => await GoToSettingsAsync());
        }

        public async Task LoadChatSessionsAsync()
        {
            ChatSessions.Clear(); // Clear the current list to avoid duplication
            var sessions = await _databaseService.GetAllChatSessionsAsync();
            foreach (var session in sessions)
            {
                ChatSessions.Add(session);
            }
        }

        private async Task AddChatSessionAsync()
        {
            var newSession = new ChatSession { SessionName = "New Session" };
            await _databaseService.AddChatSessionAsync(newSession);
            ChatSessions.Add(newSession);
        }

        private async void OnSessionSelected(ChatSession session)
        {
            if (session == null)
                return;

            var navigation = Application.Current?.MainPage?.Navigation;
            if (navigation != null)
            {
                var chatSessionPage = new ChatSessionPage(new ChatSessionViewModel(session, _databaseService));
                await navigation.PushAsync(chatSessionPage);
            }
        }

        private async Task GoToSettingsAsync()
        {
            var navigation = Application.Current?.MainPage?.Navigation;
            if (navigation != null)
            {
                await navigation.PushAsync(new SettingsPage(new SettingsViewModel(_databaseService)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
