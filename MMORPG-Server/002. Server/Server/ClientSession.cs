using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;

namespace Server
{
	class ClientSession : PacketSession
	{
		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			//Packet packet = new Packet() { size = 100, packetId = 10 };

			//ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
			//byte[] buffer = BitConverter.GetBytes(packet.size);
			//byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
			//Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
			//Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
			//ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

			// 100명
			// 1 -> 이동패킷이 100명
			// 100 -> 이동패킷이 100 * 100 = 1만}
			//Send(sendBuff);			
			Thread.Sleep(5000);
			Disconnect();
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
