using DiscordRichPresence.Api;
using DiscordRichPresence.Rpc;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DiscordRichPresence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal DiscordAsset[] Assets { get; private set; }

        private DiscordController _controller = null;
        private string _appId = null;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async Task<bool> UpdateAssets()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    Uri uri = new Uri($"https://discordapp.com/api/v7/oauth2/applications/{this._appId}/assets");
                    string data = await client.DownloadStringTaskAsync(uri);

                    DiscordAsset[] assets = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscordAsset[]>(data);

                    for (int i = 0; i < assets.Length; ++i)
                        assets[i].Init(this._appId);

                    this.Assets = assets;

                    this.PresenceControl.SetAssets(assets);

                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show($"An error occured:\n{ex.ToString()}", "Error", MessageBoxButton.OK);

                return false;
            }

            return true;
        }

#region Event Handlers

        private void OnClosing(object sender, EventArgs e)
        {
            this._controller?.OnDisable();
        }

        private async void OnEnableApplicationClick(object sender, EventArgs e)
        {
            this.EnableApplicationButton.IsEnabled = false;
            this.ApplicationIdTextBox.IsEnabled = false;

            this._appId = this.ApplicationIdTextBox.Text;

            if (!await this.UpdateAssets())
            {
                this.EnableApplicationButton.IsEnabled = true;
                this.ApplicationIdTextBox.IsEnabled = true;

                return;
            }


            this._controller = new DiscordController();

            this.PresenceControl.IsEnabled = true;
            this.UpdateAssetsButton.IsEnabled = true;
            this.UpdatePresenceButton.IsEnabled = true;
            this.RemovePresenceButton.IsEnabled = true;

            this._controller.OnEnable(this._appId);
        }

        private void OnUpdateAssetsClick(object sender, EventArgs e)
        {
            // Assigning to variable to get rid of warning, exception is handled in the method itself
            Task<bool> _ = this.UpdateAssets();
        }

        private void OnUpdatePresenceClick(object sender, EventArgs e)
        {
            DiscordRpc.RichPresence richPresence = this.PresenceControl.GetRichPresence();
            DiscordRpc.UpdatePresence(ref richPresence);
        }


        private void OnRemovePresenceClick(object sender, EventArgs e)
        {
            DiscordRpc.RichPresence richPresence = new DiscordRpc.RichPresence();
            DiscordRpc.UpdatePresence(ref richPresence);
        }

#endregion Event Handlers
    }
}
