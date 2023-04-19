using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
	public class SessionManager
	{
		public static SessionManager Instance { get; } = new SessionManager();

		HashSet<ServerSession> _sessions = new HashSet<ServerSession>();
		object _lock = new object();
		int _dummyId = 1;

        public ServerSession Generate()
		{
			lock (_lock)
			{
				ServerSession session = new ServerSession();
				session.DummyId = _dummyId;
				_dummyId++;

				_sessions.Add(session);
				Console.WriteLine($"Connected ({_sessions.Count}) Players");
				return session;
			}
		}

		public void Remove(ServerSession session)
		{
			lock (_lock)
			{
				_sessions.Remove(session);
				Console.WriteLine($"Connected ({_sessions.Count}) Players");
			}
		}
	}
}
