using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using TaskTrackerWeb.Entities;
using TaskTrackerWeb.Models;
using TaskTrackerWeb.Services;

namespace TaskTrackerWeb.Components.Layout
{
    
    public partial class Auth()
    {
        [Parameter]
        public EventCallback OnClose { get; set; }
        [Parameter]
        public AuthModel AuthOption { get; set; } = AuthModel.Login;
        [Parameter]
        public string RegisterSuccess { get; set; } = string.Empty;
        [Inject]
        public AuthService AuthService { get; set; } = default!;
        [Inject]
        public NavigationManager Navigation { get; set; } = default!;
        private RegisterModel _register = new();
        private EditContext? _editContext;
        private ValidationMessageStore? _messageStore;

        protected override void OnInitialized()
        {
            _editContext = new(_register);
            _messageStore = new(_editContext);

            _editContext.OnFieldChanged += (s, e) =>
            {
                if (e.FieldIdentifier.FieldName == nameof(_register.Email))
                {
                    _messageStore?.Clear(e.FieldIdentifier);
                }
            };
            base.OnInitialized();
        }
    
        private async Task CloseForm()
        {
            await OnClose.InvokeAsync();
        }
        private void HandleFormModel(AuthModel option) => AuthOption = option;
        private async Task SubmitRegister()
        {
            (bool accountExist, string error) = await AuthService.AccountExist(_register.Email);
            if(accountExist && string.IsNullOrEmpty(error))
            {
                _messageStore?.Add(_editContext!.Field(nameof(_register.Email)), "Account already exist!");
                _editContext?.NotifyValidationStateChanged();
                return;
            } else if(!accountExist && !string.IsNullOrEmpty(error))
            {
                _messageStore?.Add(_editContext!.Field(nameof(_register.Email)), error);
                _editContext?.NotifyValidationStateChanged();
                return;
            }
             bool success = await AuthService.AddUser(_register);
            if(success) Navigation.NavigateTo("/?success=true", true);
            else
            {
                _messageStore?.Add(_editContext!.Field(nameof(_register.Name)), "Something goes wrong, try later!");
                _editContext?.NotifyValidationStateChanged();
            }
        }
    }
}
