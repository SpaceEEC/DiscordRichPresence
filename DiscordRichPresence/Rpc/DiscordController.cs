using System;
using System.Diagnostics;
using static DiscordRichPresence.Rpc.DiscordRpc;

namespace DiscordRichPresence.Rpc
{
	internal class DiscordController
	{
		internal void ReadyCallback(ref DiscordUser connectedUser) => Debug.WriteLine($"Discord: ready {connectedUser.userId}#{connectedUser.discriminator}");

		internal void DisconnectedCallback(Int32 errorCode, String message)
			=> Debug.WriteLine($"Discord: disconnect {errorCode}: {message}");

		internal void ErrorCallback(Int32 errorCode, String message)
			=> Debug.WriteLine($"Discord: error {errorCode}: {message}");

		internal void OnEnable(String applicationId)
		{
			Debug.WriteLine("Discord: init");

			EventHandlers handlers = new EventHandlers();
			handlers.readyCallback += this.ReadyCallback;
			handlers.disconnectedCallback += this.DisconnectedCallback;
			handlers.errorCallback += this.ErrorCallback;
			DiscordRpc.Initialize(applicationId, ref handlers, true, null);
		}

		internal void OnDisable()
		{
			Debug.WriteLine("Discord: shutdown");
			DiscordRpc.Shutdown();
		}
	}
}
