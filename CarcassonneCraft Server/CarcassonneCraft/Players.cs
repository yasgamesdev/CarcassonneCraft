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

        public static int GetPlayerCount()
        {
            return players.Count;
        }

        public static void Push(NetConnection connection, PushData push)
        {
            players[connection].Push(push);
        }

        public static Dictionary<int, List<Player>> GetSnapshotPlayer()
        {
            Dictionary<int, List<Player>> ret = new Dictionary<int, List<Player>>();

            foreach(Player player in players.Values)
            {
                XZNum playerPos = new XZNum(player.init.sync.xpos, player.init.sync.zpos);
                XZNum areasNum = Env.GetAreasNum(playerPos);
                int index = areasNum.xnum + Env.XAreasN * areasNum.znum;
                int areaid = player.init.selects[index];

                if(!ret.ContainsKey(areaid))
                {
                    List<Player> list = new List<Player>();
                    list.Add(player);
                    ret.Add(areaid, list);
                }
                else
                {
                    ret[areaid].Add(player);
                }
            }

            return ret;
        }

        public static OtherPlayerInitData GetPlayerInitData(int userid)
        {
            foreach (Player player in players.Values)
            {
                if(player.init.sync.userid == userid)
                {
                    OtherPlayerInitData other = new OtherPlayerInitData();
                    other.username = player.init.username;
                    PlayerSyncData sync = new PlayerSyncData();
                    sync.userid = player.init.sync.userid;
                    sync.xpos = player.init.sync.xpos;
                    sync.ypos = player.init.sync.ypos;
                    sync.zpos = player.init.sync.zpos;
                    sync.xrot = player.init.sync.xrot;
                    sync.yrot = player.init.sync.yrot;
                    sync.animestate = player.init.sync.animestate;
                    other.sync = sync;
                    return other;
                }
            }

            return null;
        }
    }
}
