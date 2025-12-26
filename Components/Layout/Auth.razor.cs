using Microsoft.AspNetCore.Components;

namespace TaskTrackerWeb.Components.Layout
{
    
    public partial class Auth
    {
        [Parameter]
        public EventCallback OnClose { get; set; }
        [Parameter]
        public AuthModel AuthOption { get; set; } = AuthModel.Login;
        private async Task CloseForm()
        {
            await OnClose.InvokeAsync();
        }
        private void HandleFormModel(AuthModel option) => AuthOption = option; 
    }
}
