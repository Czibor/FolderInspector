namespace FolderInspector
{
    public partial class App
    {
        private void Application_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            FolderInspector.Properties.Settings.Default.Save();
        }
    }
}
