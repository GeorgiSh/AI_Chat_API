using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AI_Chat_API.Models;
using AI_Chat_API.Services;

namespace AI_Chat_API.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private APIConfiguration _selectedApiConfig;
        private bool _isApiDetailsVisible;
        private bool _isDeleteButtonVisible;

        public ObservableCollection<APIConfiguration> ApiConfigurations { get; }
        public APIConfiguration SelectedApiConfig
        {
            get => _selectedApiConfig;
            set
            {
                _selectedApiConfig = value;
                OnPropertyChanged();
            }
        }

        public bool IsApiDetailsVisible
        {
            get => _isApiDetailsVisible;
            set
            {
                _isApiDetailsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsDeleteButtonVisible
        {
            get => _isDeleteButtonVisible;
            set
            {
                _isDeleteButtonVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddApiCommand { get; }
        public ICommand SaveApiCommand { get; }
        public ICommand DeleteApiCommand { get; }
        public ICommand EditApiConfigurationCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectApiCommand { get; }

        public SettingsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            ApiConfigurations = new ObservableCollection<APIConfiguration>();

            AddApiCommand = new Command(AddApiConfiguration);
            SaveApiCommand = new Command(async () => await SaveApiConfigurationAsync());
            DeleteApiCommand = new Command<APIConfiguration>(async (apiConfig) => await DeleteApiConfigurationAsync(apiConfig));
            EditApiConfigurationCommand = new Command<APIConfiguration>(EditApiConfiguration);
            CancelCommand = new Command(Cancel);
            SelectApiCommand = new Command<APIConfiguration>(OnApiSelected);

            LoadApiConfigurationsAsync();
        }

        private async Task LoadApiConfigurationsAsync()
        {
            var configs = await _databaseService.GetAllApiConfigurationsAsync();
            foreach (var config in configs)
            {
                ApiConfigurations.Add(config);
            }
        }

        private void AddApiConfiguration()
        {
            SelectedApiConfig = new APIConfiguration();
            IsApiDetailsVisible = true;
            IsDeleteButtonVisible = false;
        }

        private void EditApiConfiguration(APIConfiguration config)
        {
            SelectedApiConfig = config;
            IsApiDetailsVisible = true;
            IsDeleteButtonVisible = true;
        }

        private async Task SaveApiConfigurationAsync()
        {
            if (SelectedApiConfig == null)
                return;

            if (SelectedApiConfig.Id == 0)
            {
                await _databaseService.AddApiConfigurationAsync(SelectedApiConfig);
                ApiConfigurations.Add(SelectedApiConfig);
            }
            else
            {
                await _databaseService.UpdateApiConfigurationAsync(SelectedApiConfig);
                var index = ApiConfigurations.IndexOf(SelectedApiConfig);
                ApiConfigurations[index] = SelectedApiConfig;
            }

            IsApiDetailsVisible = false;
        }

        private async Task DeleteApiConfigurationAsync(APIConfiguration config)
        {
            if (config == null || config.Id == 0)
                return;

            await _databaseService.DeleteApiConfigurationAsync(config.Id);
            ApiConfigurations.Remove(config);

            IsApiDetailsVisible = false;
        }

        private void Cancel()
        {
            IsApiDetailsVisible = false;
        }

        private void OnApiSelected(APIConfiguration config)
        {
            EditApiConfiguration(config);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
