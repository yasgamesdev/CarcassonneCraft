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

        public bool updated = true;

        Transform player;

        public OtherPlayer(OtherPlayerInitData init, Player player)
        {
            this.init = init;
            previous = init.sync;

            prefab = new PrefabManager();
            prefab.LoadPrefab("OtherPlayer");
            prefab.GetInstance().GetComponent<OtherPlayerPrefabScript>().Init(init);

            name = new PrefabManager();
            name.LoadPrefab("NamePlate", GameObject.Find("Canvas").transform);
            name.GetInstance().GetComponent<Follow>().Init(prefab.GetInstance().transform, init.username);

            this.player = player.GetPrefabTransform();
        }

        public void Update(float delta)
        {
            prefab.GetInstance().GetComponent<OtherPlayerPrefabScript>().Interpolation(init.sync, delta);

            Vector3 dir = prefab.GetInstance().transform.position - player.position;

            if(Vector3.Dot(player.forward, dir) >= 0)
            {
                name.GetInstance().SetActive(true);
            }
            else
            {
                name.GetInstance().SetActive(false);
            }
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
