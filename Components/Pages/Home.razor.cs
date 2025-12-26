namespace TaskTrackerWeb.Components.Pages
{
    
    public partial class Home
    {
        private Dictionary<string, bool> _showDesc = new() { { "Plan", false }, { "Track", false }, { "Achieve", false } };
        private bool _showForm = false;
        private AuthModel _authOption = AuthModel.Login;
        private void HandleDesc(string name)
        {
            foreach(var item in _showDesc)
            {
                if (name != item.Key) _showDesc[item.Key] = false;
                else _showDesc[item.Key] = _showDesc[item.Key] == false; 
            }
        }
        private void CloseForm() => _showForm = false;
        private void OpenForm(AuthModel authOption)
        {
            _showForm = true;
            _authOption = authOption;
        }
    }
}
