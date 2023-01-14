using Flurl.Http;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GTAChaos.Utils
{
    public class UpdateChecker
    {
        private static readonly string apiLatest = "https://github.com/gta-chaos-mod/Trilogy-ASI-Script/releases/latest";

        public static async void CheckForUpdate(bool automatic = true)
        {
            try
            {
                string text = await apiLatest.GetStringAsync();
                string pattern = @"/gta-chaos-mod/Trilogy-ASI-Script/tree/v(\d\.\d\.\d)";

                Match m = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (!m.Success || m.Groups.Count < 2 || m.Groups[1].Captures.Count == 0)
                {
                    return;
                }

                Version remoteVersion = new(m.Groups[1].Captures[0].Value);

                if (remoteVersion > Shared.Version)
                {
                    ShowUpdateWindow(remoteVersion);
                }
                else if (!automatic)
                {
                    ShowLatestVersionWindow();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void ShowUpdateWindow(Version version)
        {
            DialogResult result = MessageBox.Show(null, $"A new version is available - v{version}\nWould you like to go to the GitHub repository to download the new version?", $"Update Available (v{version})", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(apiLatest);
            }
        }

        private static void ShowLatestVersionWindow() => MessageBox.Show(null, $"You are already on the latest version (v{Shared.Version})", $"No Updates Available (v{Shared.Version})");
    }
}
