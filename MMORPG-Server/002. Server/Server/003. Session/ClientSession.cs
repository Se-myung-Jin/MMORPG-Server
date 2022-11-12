using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;

namespace Server
{
	class ClientSession : PacketSession
	{
		public int SessionId { get; set; }

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			// PROTO Test
			S_Chat chat = new S_Chat()
			{
				Context = "안녕하세요"
			};

			ushort size = (ushort)chat.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
			ushort protocolId = (ushort)MsgId.SChat;
			Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(chat.ToByteArray(), 0, sendBuffer, 4, size);

			Send(new ArraySegment<byte>(sendBuffer));

			//S_Chat chat2 = new S_Chat();
			//chat2.MergeFrom(sendBuffer, 4, sendBuffer.Length - 4);
			//////////////////////////
			//////////////////////////
			//Program.Room.Push(() => Program.Room.Enter(this));
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
