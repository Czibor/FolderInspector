using System.Windows.Data;

namespace FolderInspectorView
{
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path) : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = FolderInspector.Properties.Settings.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }
}