using System.Diagnostics;
using static DiscordRichPresence.Rpc.DiscordRpc;

namespace DiscordRichPresence.Rpc
{
	internal class DiscordController
	{
		internal void ReadyCallback(ref DiscordUser connectedUser) => Debug.WriteLine($"Discord: ready {connectedUser.userId}#{connectedUser.discriminator}");

		internal void DisconnectedCallback(int errorCode, string message)
			=> Debug.WriteLine($"Discord: disconnect {errorCode}: {message}");

		internal void ErrorCallback(int errorCode, string message)
			=> Debug.WriteLine($"Discord: error {errorCode}: {message}");

		internal void OnEnable(string applicationId)
		{
			Debug.WriteLine("Discord: init");

			EventHandlers handlers = new EventHandlers();
			handlers.readyCallback += ReadyCallback;
			handlers.disconnectedCallback += DisconnectedCallback;
			handlers.errorCallback += ErrorCallback;
			DiscordRpc.Initialize(applicationId, ref handlers, true, null);
		}

		internal void OnDisable()
		{
			Debug.WriteLine("Discord: shutdown");
			DiscordRpc.Shutdown();
		}
	}
}
