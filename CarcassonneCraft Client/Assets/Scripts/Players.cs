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
                otherPlayers.Add(init.sync.userid, new OtherPlayer(init));
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

        public static void FixedUpdate(ObjectType type, float delta)
        {
            Dictionary<int, ClientObject> targets = objs[(int)type];

            foreach (KeyValuePair<int, ClientObject> tar in targets)
            {
                tar.Value.Update(delta);
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

        public static void UpdatePlayerSyncData(List<PlayerSyncData> syncs)
        {
            Dictionary<int, ClientObject> targets = objs[(int)ObjectType.Player];

            foreach (KeyValuePair<int, ClientObject> tar in targets)
            {
                tar.Value.ResetUpdateFlag();
            }

            foreach (PlayerSyncData sync in syncs)
            {
                if(sync.userid == player.GetObjID())
                {
                    player.ReceiveLatestData(sync);
                }
                else if (targets.ContainsKey(sync.userid))
                {
                    ((OtherPlayer)targets[sync.userid]).ReceiveLatestData(sync);
                }
                else
                {
                    //warning
                }
            }

            var removes = targets.Where(f => f.Value.updated == false).ToArray();
            foreach (var remove in removes)
            {
                remove.Value.Destroy();
                targets.Remove(remove.Key);
            }
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
