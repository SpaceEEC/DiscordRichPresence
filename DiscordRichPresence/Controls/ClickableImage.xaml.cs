using DiscordRichPresence.Api;
using System.Windows.Controls;

namespace DiscordRichPresence.Controls
{
	/// <summary>
	/// Interaction logic for ClickableImage.xaml
	/// </summary>
	public partial class ClickableImage : UserControl
	{
		internal readonly DiscordAsset Asset;

		internal ClickableImage(DiscordAsset asset)
		{
			this.InitializeComponent();

			this.Asset = asset;
			this.Image.Source = asset.Image;
		}
	}
}
