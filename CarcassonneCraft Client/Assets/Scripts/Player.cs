using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using UnityEngine;

namespace CarcassonneCraft
{
    public class Player : PlayerBase
    {
        PlayerInitData init;
        PlayerSyncData previous;

        PrefabManager prefab;

        public Player(PlayerInitData init)
        {
            this.init = init;
            previous = init.sync;

            prefab = new PrefabManager();
            prefab.LoadPrefab("Player");
            prefab.GetInstance().GetComponent<PlayerPrefabScript>().Init(init);
        }

        public void PushData()
        {
            PushData data = prefab.GetInstance().GetComponent<PlayerPrefabScript>().GetPushData();

            GCli.Send(MessageType.Push, GCli.Serialize<PushData>(data), NetDeliveryMethod.UnreliableSequenced);
        }

        public int GetUserID()
        {
            return init.sync.userid;
        }

        public override void ReceiveLatestData(PlayerSyncData sync)
        {
            previous = init.sync;
            init.sync = sync;
        }

        public XZNum GetPlayerPos()
        {
            return new XZNum(prefab.GetInstance().transform.position.x, prefab.GetInstance().transform.position.z);
        }

        public int GetSelectArea(XZNum areasNum)
        {
            return init.selects[areasNum.xnum + Env.XAreasN * areasNum.znum];
        }
    }
}
