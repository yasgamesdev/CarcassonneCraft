using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CarcassonneCraft
{
    public class OtherPlayer : PlayerBase
    {
        OtherPlayerInitData init;
        PlayerSyncData previous;

        PrefabManager prefab, name;

        bool updated = true;

        public OtherPlayer(OtherPlayerInitData init)
        {
            this.init = init;
            previous = init.sync;

            prefab = new PrefabManager();
            prefab.LoadPrefab("OtherPlayer");
            prefab.GetInstance().GetComponent<OtherPlayerPrefabScript>().Init(init);

            name = new PrefabManager();
            name.LoadPrefab("NamePlate", GameObject.Find("Canvas").transform);
            name.GetInstance().GetComponent<Follow>().Init(prefab.GetInstance().transform, init.username);
        }

        public void Update(float delta)
        {
            prefab.GetInstance().GetComponent<OtherPlayerPrefabScript>().Interpolation(init.sync, delta);
        }

        public void Destroy()
        {
            prefab.Destroy();
            name.Destroy();
        }

        public int GetUserID()
        {
            return init.sync.userid;
        }

        public override void ReceiveLatestData(PlayerSyncData sync)
        {
            previous = init.sync;
            init.sync = sync;

            updated = true;
        }

        public void ResetUpdateFlag()
        {
            updated = false;
        }
    }
}
