using DiscordRichPresence.Api;
using DiscordRichPresence.Controls;
using System;
using System.Windows;

namespace DiscordRichPresence.Windows
{
	/// <summary>
	/// Interaction logic for ImagePicker.xaml
	/// </summary>
	public partial class ImagePicker : Window
	{
		internal DiscordAsset SelectedAsset { get; private set; }

		internal ImagePicker(DiscordAsset[] assets)
		{
			this.InitializeComponent();

			for (Int32 i = 0; i < assets.Length; ++i)
			{
				ClickableImage image = new ClickableImage(assets[i]);
				image.MouseDoubleClick += this.OnImageDoubleClick;
				this.WrapPanel.Children.Add(image);
			}
		}

		private void OnImageDoubleClick(Object sender, EventArgs e)
		{
			this.DialogResult = true;
			this.SelectedAsset = ((ClickableImage)sender).Asset;
			this.Close();
		}
	}
}
