﻿using DiscordRichPresence.Api;
using DiscordRichPresence.Rpc;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace DiscordRichPresence
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal DiscordAsset[] Assets { get; private set; }

		private DiscordController _controller = null;
		private String _appId = null;

		public MainWindow()
		{
			this.InitializeComponent();
#if DEBUG
			this.ApplicationIdTextBox.Text = "257884228451041280";
#endif
		}

		private async Task<Boolean> UpdateAssets()
		{
			try
			{
				using (WebClient client = new WebClient())
				{
					Uri uri = new Uri($"https://discordapp.com/api/v7/oauth2/applications/{this._appId}/assets");
					String data = await client.DownloadStringTaskAsync(uri);

					DiscordAsset[] assets = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscordAsset[]>(data);

					for (Int32 i = 0; i < assets.Length; ++i)
						assets[i].Init(this._appId);

					this.Assets = assets;

					this.PresenceControl.SetAssets(assets);

				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				MessageBox.Show($"An error occured:\n{ex.ToString()}", "Error", MessageBoxButton.OK);

				return false;
			}

			return true;
		}

#region Event Handlers

		private void OnClosing(Object sender, EventArgs e) => this._controller?.OnDisable();

		private async void OnEnableApplicationClick(Object sender, EventArgs e)
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

		private void OnUpdateAssetsClick(Object sender, EventArgs e)
		{
#pragma warning disable CS4014
			this.UpdateAssets();
#pragma warning restore CS4014
		}

		private void OnUpdatePresenceClick(Object sender, EventArgs e)
		{
			this.LockRichPresenceButtons();

			DiscordRpc.UpdatePresence(this.PresenceControl.GetRichPresence());
		}


		private void OnRemovePresenceClick(Object sender, EventArgs e)
		{
			this.LockRichPresenceButtons();

			DiscordRpc.ClearPresence();
		}

		private void RichPresenceButtonLockTimerElapsed(Object sender, EventArgs e)
		{
			this.Dispatcher.Invoke(() => this.SetRichPresenceButtonsEnabled(true));
			Timer timer = (Timer)sender;
			timer.Elapsed -= this.RichPresenceButtonLockTimerElapsed;
			timer.Dispose();
		}

#endregion Event Handlers

		private void LockRichPresenceButtons()
		{
			this.SetRichPresenceButtonsEnabled(false);

			new Timer(15_000)
			{
				AutoReset = false,
				Enabled = true
			}
			.Elapsed += this.RichPresenceButtonLockTimerElapsed;
		}

		private void SetRichPresenceButtonsEnabled(Boolean state)
		{
			this.UpdatePresenceButton.IsEnabled = state;
			this.RemovePresenceButton.IsEnabled = state;
		}
	}
}
