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
            InitializeComponent();

            this.PartySizeTextBox.SetValidator(this.ValidateTextBox);
            this.PartyMaxTextBox.SetValidator(this.ValidateTextBox);
            this.TimestampTextBox.SetValidator((textBox, text) =>
            {
                if (String.IsNullOrEmpty(text)) return null;

                if (DateTime.TryParse(text, out DateTime parsedTime))
                {
                    return parsedTime;
                }

                return textBox;
            });
        }

        internal void SetAssets(DiscordAsset[] assets)
        {
            this._assets = assets;
        }

        internal RichPresence GetRichPresence()
        {
            RichPresence richPresence = new RichPresence()
            {
                state = this.StateTextBox.Text,
                details = this.DetailsTextBox.Text,
                instance = this.InstanceCheckBox.IsChecked ?? false,
                matchSecret = this.MatchSecretTextBox.Text,
                joinSecret = this.JoinSecretTextBox.Text,
                spectateSecret = this.SpectateSecretTextBox.Text
            };

            if (this._largeAsset != null)
            {
                richPresence.largeImageKey = this._largeAsset.Name;
                richPresence.largeImageText = this.LargeImageTextTextBox.Text;
            }

            if (this._smallAsset != null)
            {
                richPresence.smallImageKey = this._smallAsset.Name;
                richPresence.smallImageText = this.SmallImageTextTextBox.Text;
            }

            if (this.PartyMaxTextBox != null && this.PartySizeTextBox.Value != null)
            {
                richPresence.partyId = "partyId";
                richPresence.partySize = (int)this.PartySizeTextBox.Value;
                richPresence.partyMax = (int)this.PartyMaxTextBox.Value;
            }

            if (this.TimestampTextBox.Value != null)
            {
                DateTime dateTime = (DateTime)this.TimestampTextBox.Value;

                // Yesterday
                if (this.TimestampComboBox.SelectedIndex == 0)
                {
                    dateTime = dateTime.AddDays(-1);
                }
                // Tomorrow
                else if (this.TimestampComboBox.SelectedIndex == 2)
                {
                    dateTime = dateTime.AddDays(1);
                }

                // Now or in the past
                if (dateTime <= DateTime.Now)
                {
                    richPresence.startTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
                }
                // In the future
                else
                {
                    richPresence.endTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
                }

            }

            return richPresence;
        }

        private object ValidateTextBox(ValidatableTextBox textBox, string text)
        {
            if (String.IsNullOrEmpty(text)) return null;

            if (int.TryParse(text, out int value))
                return value;

            return textBox;
        }

#region Wpf Event Callbacks

        private void CanDoubleClickImage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnDoubleClickImage(object sender, ExecutedRoutedEventArgs e)
        {
            string parameter = (string)e.Parameter;

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

        private void OnRemoveSmallImageMenuItemClick(object sender, RoutedEventArgs e)
        {
            this._smallAsset = null;
            this.SmallImage.Source = null;
        }

        private void OnRemoveLargeImageMenuItemClick(object sender, RoutedEventArgs e)
        {
            this._largeAsset = null;
            this.LargeImage.Source = null;
        }

#endregion Wpf Event Callbacks
    }
}
