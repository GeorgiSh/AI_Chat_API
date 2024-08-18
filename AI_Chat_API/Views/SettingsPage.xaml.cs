using AI_Chat_API.ViewModels;
using Microsoft.Maui.Controls;

namespace AI_Chat_API.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
