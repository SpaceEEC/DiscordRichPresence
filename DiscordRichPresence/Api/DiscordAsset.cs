using Newtonsoft.Json;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiscordRichPresence.Api
{
	internal class DiscordAsset
	{
		internal enum AssetType
		{
			SMALL = 1,
			LARGE = 2,
		}

#pragma warning disable CS0649
		[JsonProperty("type")]
		internal readonly AssetType Type;
		[JsonProperty("id")]
		internal readonly ulong Id;
		[JsonProperty("name")]
		internal readonly string Name;
#pragma warning restore CS0649

		internal bool IsInitialized => this._image != null;
		internal string ImageUrl => this._applicationId != null
			? $"https://cdn.discordapp.com/app-assets/{this._applicationId}/{this.Id}.png"
			: throw new InvalidOperationException("Asset has not been initialized yet!");
		internal ImageSource Image => this._image ?? throw new InvalidOperationException("Asset has not been initialized yet!");

		private string _applicationId = null;
		private ImageSource _image = null;

		internal void Init(string applicationId)
		{
			if (this._image != null) return;

			this._applicationId = applicationId;
			this._image = new BitmapImage(new Uri($"{this.ImageUrl}?size=128"));
		}

	}
}
