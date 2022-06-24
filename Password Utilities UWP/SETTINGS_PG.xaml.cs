using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Security_Utilities_UWP
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SETTINGS_PG : Page
    {
        public SETTINGS_PG()
        {
            this.InitializeComponent();
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            VERSION_TXT.Text = string.Format("Build: v{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        private async void GitHub_BTN_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var success = await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/EinsKatze/Security-Utilities"));
        }
    }
}
