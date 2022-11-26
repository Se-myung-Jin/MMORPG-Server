using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();
        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        int _playerId = 1;

        public Player Add()
        {
            Player player = new Player();

            lock (_lock)
            {
                player.Info.PlayerId = _playerId;
                _players.Add(_playerId, player);
                _playerId++;
            }

            return player;
        }

        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return _players.Remove(playerId);
            }
        }

        public Player Find(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.TryGetValue(playerId, out player))
                    return player;

                return null;
            }
        }
    }
}
