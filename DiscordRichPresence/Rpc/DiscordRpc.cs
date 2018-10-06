using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DiscordRichPresence.Rpc
{
	/* https://github.com/discordapp/discord-rpc/blob/master/examples/button-clicker/Assets/DiscordRpc.cs */
	internal class DiscordRpc
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ReadyCallback(ref DiscordUser connectedUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void DisconnectedCallback(Int32 errorCode, String message);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ErrorCallback(Int32 errorCode, String message);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void JoinCallback(String secret);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SpectateCallback(String secret);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void RequestCallback(ref DiscordUser connectedUser);

		public struct EventHandlers
		{
			public ReadyCallback readyCallback;
			public DisconnectedCallback disconnectedCallback;
			public ErrorCallback errorCallback;
			public JoinCallback joinCallback;
			public SpectateCallback spectateCallback;
			public RequestCallback requestCallback;
		}

		[Serializable, StructLayout(LayoutKind.Sequential)]
		public struct RichPresenceStruct
		{
			public IntPtr state; /* max 128 bytes */
			public IntPtr details; /* max 128 bytes */
			public Int64 startTimestamp;
			public Int64 endTimestamp;
			public IntPtr largeImageKey; /* max 32 bytes */
			public IntPtr largeImageText; /* max 128 bytes */
			public IntPtr smallImageKey; /* max 32 bytes */
			public IntPtr smallImageText; /* max 128 bytes */
			public IntPtr partyId; /* max 128 bytes */
			public Int32 partySize;
			public Int32 partyMax;
			public IntPtr matchSecret; /* max 128 bytes */
			public IntPtr joinSecret; /* max 128 bytes */
			public IntPtr spectateSecret; /* max 128 bytes */
			public Boolean instance;
		}

#pragma warning disable CS0649

		[Serializable]
		public struct DiscordUser
		{
			public String userId;
			public String username;
			public String discriminator;
			public String avatar;
		}
#pragma warning restore CS0649

		public enum Reply
		{
			No = 0,
			Yes = 1,
			Ignore = 2
		}

		[DllImport("discord-rpc", EntryPoint = "Discord_Initialize", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Initialize(String applicationId, ref EventHandlers handlers, Boolean autoRegister, String optionalSteamId);

		[DllImport("discord-rpc", EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Shutdown();

		[DllImport("discord-rpc", EntryPoint = "Discord_RunCallbacks", CallingConvention = CallingConvention.Cdecl)]
		public static extern void RunCallbacks();

		[DllImport("discord-rpc", EntryPoint = "Discord_UpdatePresence", CallingConvention = CallingConvention.Cdecl)]
		private static extern void UpdatePresenceNative(ref RichPresenceStruct presence);

		[DllImport("discord-rpc", EntryPoint = "Discord_ClearPresence", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ClearPresence();

		[DllImport("discord-rpc", EntryPoint = "Discord_Respond", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Respond(String userId, Reply reply);

		[DllImport("discord-rpc", EntryPoint = "Discord_UpdateHandlers", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UpdateHandlers(ref EventHandlers handlers);

		public static void UpdatePresence(RichPresence presence)
		{
			var presencestruct = presence.GetStruct();
			UpdatePresenceNative(ref presencestruct);
			presence.FreeMem();
		}

		public class RichPresence
		{
			private RichPresenceStruct _presence;
			private readonly List<IntPtr> _buffers = new List<IntPtr>(10);

			public String state; /* max 128 bytes */
			public String details; /* max 128 bytes */
			public Int64 startTimestamp;
			public Int64 endTimestamp;
			public String largeImageKey; /* max 32 bytes */
			public String largeImageText; /* max 128 bytes */
			public String smallImageKey; /* max 32 bytes */
			public String smallImageText; /* max 128 bytes */
			public String partyId; /* max 128 bytes */
			public Int32 partySize;
			public Int32 partyMax;
			public String matchSecret; /* max 128 bytes */
			public String joinSecret; /* max 128 bytes */
			public String spectateSecret; /* max 128 bytes */
			public Boolean instance;

			/// <summary>
			/// Get the <see cref="RichPresenceStruct"/> reprensentation of this instance
			/// </summary>
			/// <returns><see cref="RichPresenceStruct"/> reprensentation of this instance</returns>
			internal RichPresenceStruct GetStruct()
			{
				if (_buffers.Count > 0)
				{
					FreeMem();
				}

				_presence.state = StrToPtr(state);
				_presence.details = StrToPtr(details);
				_presence.startTimestamp = startTimestamp;
				_presence.endTimestamp = endTimestamp;
				_presence.largeImageKey = StrToPtr(largeImageKey);
				_presence.largeImageText = StrToPtr(largeImageText);
				_presence.smallImageKey = StrToPtr(smallImageKey);
				_presence.smallImageText = StrToPtr(smallImageText);
				_presence.partyId = StrToPtr(partyId);
				_presence.partySize = partySize;
				_presence.partyMax = partyMax;
				_presence.matchSecret = StrToPtr(matchSecret);
				_presence.joinSecret = StrToPtr(joinSecret);
				_presence.spectateSecret = StrToPtr(spectateSecret);
				_presence.instance = instance;

				return _presence;
			}

			/// <summary>
			/// Returns a pointer to a representation of the given String with a size of maxbytes
			/// </summary>
			/// <param name="input">String to convert</param>
			/// <returns>Pointer to the UTF-8 representation of <see cref="input"/></returns>
			private IntPtr StrToPtr(String input)
			{
				if (String.IsNullOrEmpty(input)) return IntPtr.Zero;
				var convbytecnt = Encoding.UTF8.GetByteCount(input);
				var buffer = Marshal.AllocHGlobal(convbytecnt + 1);
				for (Int32 i = 0; i < convbytecnt + 1; i++)
				{
					Marshal.WriteByte(buffer, i, 0);
				}
				_buffers.Add(buffer);
				Marshal.Copy(Encoding.UTF8.GetBytes(input), 0, buffer, convbytecnt);
				return buffer;
			}

			/// <summary>
			/// Convert String to UTF-8 and add null termination
			/// </summary>
			/// <param name="toconv">String to convert</param>
			/// <returns>UTF-8 representation of <see cref="toconv"/> with added null termination</returns>
			private static String StrToUtf8NullTerm(String toconv)
			{
				var str = toconv.Trim();
				var bytes = Encoding.Default.GetBytes(str);
				if (bytes.Length > 0 && bytes[bytes.Length - 1] != 0)
				{
					str += "\0\0";
				}
				return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(str));
			}

			/// <summary>
			/// Free the allocated memory for conversion to <see cref="RichPresenceStruct"/>
			/// </summary>
			internal void FreeMem()
			{
				for (var i = _buffers.Count - 1; i >= 0; i--)
				{
					Marshal.FreeHGlobal(_buffers[i]);
					_buffers.RemoveAt(i);
				}
			}
		}
	}
}