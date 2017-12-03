using System.IO;
using System.Windows.Input;

namespace FolderInspectorView
{
    public partial class InspectorView
    {
        public InspectorView()
        {
            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (goToButton.CommandParameter != null)
            {
                string requestedPath = goToButton.CommandParameter.ToString();

                if (Directory.Exists(requestedPath))
                {
                    goToButton.Command.Execute(requestedPath);
                }
            }
        }
    }
}
