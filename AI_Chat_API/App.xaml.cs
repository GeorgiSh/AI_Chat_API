using AI_Chat_API.Services;
using AI_Chat_API.Views;
using AI_Chat_API.ViewModels;

namespace AI_Chat_API
{
    public partial class App : Application
    {
        public App(DatabaseService databaseService)
        {
            InitializeComponent();

            // Wrap the MainPage in a NavigationPage
            MainPage = new NavigationPage(new MainPage(new MainPageViewModel(databaseService)));
        }
    }
}
