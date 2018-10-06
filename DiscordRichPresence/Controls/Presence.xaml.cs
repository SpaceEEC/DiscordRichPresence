using DiscordRichPresence.Api;
using DiscordRichPresence.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using static DiscordRichPresence.Rpc.DiscordRpc;

namespace DiscordRichPresence.Controls
{
	/// <summary>
	/// Interaction logic for Presence.xaml
	/// </summary>
	public partial class Presence : UserControl
	{

		private DiscordAsset[] _assets;

		private DiscordAsset _largeAsset;
		private DiscordAsset _smallAsset;

		public Presence()
		{
			this.InitializeComponent();

			this.PartySizeTextBox.SetValidator(this.ValidateNumericTextBox);
			this.PartyMaxTextBox.SetValidator(this.ValidateNumericTextBox);

			this.DetailsTextBox.SetValidator(this.ValidateTextTextBox);
			this.StateTextBox.SetValidator(this.ValidateTextTextBox);

			this.LargeImageTextTextBox.SetValidator(this.ValidateTextTextBox);
			this.SmallImageTextTextBox.SetValidator(this.ValidateTextTextBox);

			this.MatchSecretTextBox.SetValidator(this.ValidateTextTextBox);
			this.JoinSecretTextBox.SetValidator(this.ValidateTextTextBox);
			this.SpectateSecretTextBox.SetValidator(this.ValidateTextTextBox);

			this.TimestampTextBox.SetValidator(this.ValidateDateTimeTextBox);
		}

		internal void SetAssets(DiscordAsset[] assets) => this._assets = assets;

		internal RichPresence GetRichPresence()
		{
			RichPresence richPresence = new RichPresence()
			{
				state = (String)this.StateTextBox.Value,
				details = (String)this.DetailsTextBox.Value,
				instance = this.InstanceCheckBox.IsChecked ?? false,
				matchSecret = (String)this.MatchSecretTextBox.Value,
				joinSecret = (String)this.JoinSecretTextBox.Value,
				spectateSecret = (String)this.SpectateSecretTextBox.Value
			};

			if (this._largeAsset != null)
			{
				richPresence.largeImageKey = this._largeAsset.Name;
				richPresence.largeImageText = (String)this.LargeImageTextTextBox.Value;
			}

			if (this._smallAsset != null)
			{
				richPresence.smallImageKey = this._smallAsset.Name;
				richPresence.smallImageText = (String)this.SmallImageTextTextBox.Value;
			}

			if (this.PartyMaxTextBox != null && this.PartySizeTextBox.Value != null)
			{
				richPresence.partyId = "partyId";
				richPresence.partySize = (Int32)this.PartySizeTextBox.Value;
				richPresence.partyMax = (Int32)this.PartyMaxTextBox.Value;
			}

			if (this.TimestampTextBox.Value != null)
			{
				DateTime dateTime = (DateTime)this.TimestampTextBox.Value;

				// Yesterday
				if (this.TimestampComboBox.SelectedIndex == 0)
					dateTime = dateTime.AddDays(-1);
				// Tomorrow
				else if (this.TimestampComboBox.SelectedIndex == 2)
					dateTime = dateTime.AddDays(1);

				// Now or in the past
				if (dateTime <= DateTime.Now)
					richPresence.startTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
				// In the future
				else
					richPresence.endTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
			}

			return richPresence;
		}

		private Object ValidateDateTimeTextBox(ValidatableTextBox textBox, String text)
		{
			if (String.IsNullOrEmpty(text)) return null;

			if (DateTime.TryParse(text, out DateTime parsedTime))
				return parsedTime;

			return textBox;
		}

		private Object ValidateNumericTextBox(ValidatableTextBox textBox, String text)
		{
			if (String.IsNullOrEmpty(text)) return null;

			if (Int32.TryParse(text, out Int32 value))
				return value;

			return textBox;
		}

		private Object ValidateTextTextBox(ValidatableTextBox textBox, String text)
		{
			if (text.Length == 0 || text.Length > 1)
				return text;

			return textBox;
		}

		#region Wpf Event Callbacks

		private void CanDoubleClickImage(Object sender, CanExecuteRoutedEventArgs e)
			=> e.CanExecute = true;

		private void OnDoubleClickImage(Object sender, ExecutedRoutedEventArgs e)
		{
			String parameter = (String)e.Parameter;

			ImagePicker picker = new ImagePicker(this._assets);
			if (!(picker.ShowDialog() ?? false)) return;

			if (parameter == "large")
			{
				this._largeAsset = picker.SelectedAsset;
				this.LargeImage.Source = picker.SelectedAsset.Image;
			}
			else if (parameter == "small")
			{
				this._smallAsset = picker.SelectedAsset;
				this.SmallImage.Source = picker.SelectedAsset.Image;
			}
			else
			{
				throw new ArgumentOutOfRangeException(
					$"Expected parameter to be one of \"small\" and \"large\". Received: \"{parameter}\" instead."
				);
			}
		}

		private void OnRemoveSmallImageMenuItemClick(Object sender, RoutedEventArgs e)
		{
			this._smallAsset = null;
			this.SmallImage.Source = null;
		}

		private void OnRemoveLargeImageMenuItemClick(Object sender, RoutedEventArgs e)
		{
			this._largeAsset = null;
			this.LargeImage.Source = null;
		}

#endregion Wpf Event Callbacks
	}
}
