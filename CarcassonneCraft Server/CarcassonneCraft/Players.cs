using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    public static class Players
    {
        static Dictionary<NetConnection, Player> players = new Dictionary<NetConnection, Player>();

        public static void Init()
        {
        }

        public static void AddPlayer(NetConnection connection, PlayerInitData init)
        {
            players.Add(connection, new Player(connection, init));
        }

        public static void DeletePlayer(NetConnection connection)
        {
            players.Remove(connection);
        }

        public static bool IsAuthDone(NetConnection connection)
        {
            return players.ContainsKey(connection);
        }

        public static int GetUserID(NetConnection connection)
        {
            return players[connection].init.sync.userid;
        }

        public static void UpdateSelect(NetConnection connection, SelectInfo select)
        {
            players[connection].init.selects[select.selectindex] = select.areaid;
        }
    }
}
