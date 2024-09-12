using Microsoft.Maui.Controls;
using AI_Chat_API.ViewModels;

namespace AI_Chat_API.Views
{
    public partial class ChatSessionPage : ContentPage
    {
        public ChatSessionPage(ChatSessionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
