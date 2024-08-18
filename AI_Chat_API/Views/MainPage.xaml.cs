using AI_Chat_API.ViewModels;
using Microsoft.Maui.Controls;

namespace AI_Chat_API.Views
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _viewModel;

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadChatSessionsAsync(); // Reload chat sessions when returning to the page
        }
    }
}