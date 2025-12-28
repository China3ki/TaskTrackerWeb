using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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
        public bool ShowSuccessMessage { get; set; } = false;
        [Inject]
        public AuthService AuthService { get; set; } = default!;
        [Inject]
        public NavigationManager Navigation { get; set; } = default!;
        private RegisterModel _register = new();
        private LoginModel _login = new();
        private EditContext? _registerContext;
        private EditContext? _loginContext;
        private ValidationMessageStore? _registerMessageStore;
        private ValidationMessageStore? _loginMessageStore;

        protected override void OnInitialized()
        {
            _registerContext = new(_register);
            _loginContext = new(_login);

            _registerMessageStore = new(_registerContext);
            _loginMessageStore = new(_loginContext);

            _registerContext.OnFieldChanged += (s, e) =>
            {
                if (e.FieldIdentifier.FieldName == nameof(_register.Email)) _registerMessageStore?.Clear(e.FieldIdentifier);
            };
            _loginContext.OnFieldChanged += (s, e) =>
            {
                if (e.FieldIdentifier.FieldName == nameof(_login.Email) || e.FieldIdentifier.FieldName == nameof(_login.Password)) _loginMessageStore?.Clear(e.FieldIdentifier);
            };
            base.OnInitialized();
        }
    
        private async Task CloseForm()
        {
            await OnClose.InvokeAsync();
        }
        private void CloseSuccessMessage() => ShowSuccessMessage = false;
        private void HandleFormModel(AuthModel option) => AuthOption = option;
        private async Task SubmitLogin()
        {
            (bool accountExist, string error) = await AuthService.ValidateLoginData(_login);
            if(!accountExist)
            {
                _loginMessageStore?.Add(_loginContext!.Field(nameof(_login.Password)), error);
                _loginContext?.NotifyValidationStateChanged();
                return;
            }
            Navigation.NavigateTo("/");
        }
        private async Task SubmitRegister()
        {
            (bool accountExist, string error) = await AuthService.AccountExist(_register.Email);
            if(accountExist && string.IsNullOrEmpty(error))
            {
                _registerMessageStore?.Add(_registerContext!.Field(nameof(_register.Email)), "Account already exist!");
                _registerContext?.NotifyValidationStateChanged();
                return;
            } else if(!accountExist && !string.IsNullOrEmpty(error))
            {
                _registerMessageStore?.Add(_registerContext!.Field(nameof(_register.Email)), error);
                _registerContext?.NotifyValidationStateChanged();
                return;
            }
             bool success = await AuthService.AddUser(_register);
            if (success)
            {
                ShowSuccessMessage = true;
                AuthOption = AuthModel.Login;
                _register = new RegisterModel();
                _registerContext = new EditContext(_register);
            }
            else
            {
                _registerMessageStore?.Add(_registerContext!.Field(nameof(_register.Name)), "Something goes wrong, try later!");
                _registerContext?.NotifyValidationStateChanged();
            }
        }
    }
}
