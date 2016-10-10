using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public static class Players
    {
        static Player player;
        static Dictionary<int, OtherPlayer> otherPlayers = new Dictionary<int, OtherPlayer>();

        public static void AddPlayerInitDatas(PlayerInitDatas players)
        {
            player = new Player(players.player);

            foreach (OtherPlayerInitData init in players.otherplayers)
            {
                otherPlayers.Add(init.sync.userid, new OtherPlayer(init, player));
            }
        }

        public static XZNum GetPlayerPos()
        {
            return player.GetPlayerPos();
        }

        public static int GetSelectArea(XZNum areasNum)
        {
            return player.GetSelectArea(areasNum);
        }

        public static Player GetPlayer()
        {
            return player;
        }

        public static int GetPlayerUserID()
        {
            return player.GetUserID();
        }

        public static void UpdateSelect(SelectInfo select)
        {
            player.UpdateSelect(select);
        }

        public static PushData GetPushData()
        {
            return player.GetPushData();
        }

        public static void UpdatePlayerSyncData(List<PlayerSyncData> syncs)
        {
            foreach (OtherPlayer other in otherPlayers.Values)
            {
                other.ResetUpdateFlag();
            }

            foreach (PlayerSyncData sync in syncs)
            {
                if (sync.userid == player.GetUserID())
                {
                    player.ReceiveLatestData(sync);
                }
                else if (otherPlayers.ContainsKey(sync.userid))
                {
                    otherPlayers[sync.userid].ReceiveLatestData(sync);
                }
                else
                {
                    GCli.Send(MessageType.RequestInitData, sync.userid, NetDeliveryMethod.ReliableOrdered);
                }
            }

            List<OtherPlayer> removes = new List<OtherPlayer>();
            foreach(OtherPlayer other in otherPlayers.Values)
            {
                if(other.updated == false)
                {
                    removes.Add(other);
                }
            }
            foreach(OtherPlayer other in removes)
            {
                other.Destroy();
                otherPlayers.Remove(other.GetUserID());
            }
            /*var removes = otherPlayers.Where(f => f.Value.updated == false).ToArray();
            foreach (var remove in removes)
            {
                remove.Value.Destroy();
                otherPlayers.Remove(remove.Key);
            }*/
        }

        public static void AddOtherPlayer(OtherPlayerInitData other)
        {
            if (!otherPlayers.ContainsKey(other.sync.userid))
            {
                otherPlayers.Add(other.sync.userid, new OtherPlayer(other, player));
            }
        }

        public static void FixedUpdate(float delta)
        {
            foreach (OtherPlayer other in otherPlayers.Values)
            {
                other.Update(delta);
            }
        }

        /*static Player player;
        static Dictionary<int, ClientObject>[] objs;

        public static int GetObjectTypeCount()
        {
            return Enum.GetNames(typeof(ObjectType)).Length;
        }

        public static void Init()
        {
            objs = new Dictionary<int, ClientObject>[GetObjectTypeCount()];
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = new Dictionary<int, ClientObject>();
            }
        }        

        public static void AddPlayer(PlayerInitData init)
        {
            player = new Player(init);
        }

        public static void PushData()
        {
            player.PushData();
        }

        public static int GetPlayerObjID()
        {
            return player.GetObjID();
        }

        public static void UpdatePlayer(PlayerSyncData sync)
        {
            player.ReceiveLatestData(sync);
        }

        public static ClientObject GetObject(int objID, ObjectType type)
        {
            Dictionary<int, ClientObject> targets = objs[(int)type];

            if (targets.ContainsKey(objID))
            {
                return targets[objID];
            }
            else
            {
                return null;
            }
        }

        public static void AddInitDatas(InitDatas inits)
        {
            foreach(PlayerInitData init in inits.inits)
            {
                objs[(int)ObjectType.Player].Add(init.sync.userid, new OtherPlayer(init));
            }
        }

        public static void AddObject(ClientObject obj, ObjectType type)
        {
            Dictionary<int, ClientObject> targets = objs[(int)type];
            targets.Add(obj.GetObjID(), obj);
        }        

        public static void CreateBalloon(MessageData data)
        {
            Dictionary<int, ClientObject> targets = objs[(int)ObjectType.Player];

            if (data.userid == player.GetObjID())
            {
                player.CreateBalloon(data.message);
            }
            else if(targets.ContainsKey(data.userid))
            {
                ((OtherPlayer)targets[data.userid]).CreateBalloon(data.message);
            }
        }*/
    }
}
