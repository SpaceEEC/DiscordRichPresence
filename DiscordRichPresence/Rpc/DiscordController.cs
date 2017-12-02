using System;

namespace DiscordRichPresence.Rpc
{
    internal class DiscordController
    {
        internal void ReadyCallback()
        {
            Console.WriteLine("Discord: ready");
        }

        internal void DisconnectedCallback(int errorCode, string message)
        {
            Console.WriteLine($"Discord: disconnect {errorCode}: {message}");
        }

        internal void ErrorCallback(int errorCode, string message)
        {
            Console.WriteLine($"Discord: error {errorCode}: {message}");
        }

        internal void OnEnable(string applicationId)
        {
            Console.WriteLine("Discord: init");

            DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();
            handlers.readyCallback += ReadyCallback;
            handlers.disconnectedCallback += DisconnectedCallback;
            handlers.errorCallback += ErrorCallback;
            DiscordRpc.Initialize(applicationId, ref handlers, true, null);
        }

        internal void OnDisable()
        {
            Console.WriteLine("Discord: shutdown");
            DiscordRpc.Shutdown();
        }
    }
}
