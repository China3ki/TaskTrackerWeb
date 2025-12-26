namespace TaskTrackerWeb.Components.Pages
{
    public partial class Home
    {
        private Dictionary<string, bool> _showDesc = new() { { "Plan", false }, { "Track", false }, { "Achieve", false } };
        //private bool _showDesc = false;
        private void HandleDesc(string name)
        {
            foreach(var item in _showDesc)
            {
                if (name != item.Key) _showDesc[item.Key] = false;
                else _showDesc[item.Key] = _showDesc[item.Key] == false; 
            }
        }
    }
}
