using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace CarcassonneCraft
{
    class PlayerPrefabScript : MonoBehaviour
    {
        [SerializeField]
        FirstPersonController fpsCon;
        [SerializeField]
        Transform fpsCamera;

        public void Init(PlayerInitData init)
        {
            transform.position = new Vector3(init.sync.xpos, init.sync.ypos, init.sync.zpos);
            transform.localEulerAngles = new Vector3(0, init.sync.yrot, 0);
        }

        public PushData GetPushData()
        {
            float xpos = transform.position.x;
            float ypos = transform.position.y;
            float zpos = transform.position.z;
            float xrot = fpsCamera.localEulerAngles.x;
            float yrot = transform.localEulerAngles.y;
            int animestate = fpsCon.isMoving ? 1 : 0;

            PushData data = new PushData();
            data.xpos = xpos;
            data.ypos = ypos;
            data.zpos = zpos;
            data.xrot = xrot;
            data.yrot = yrot;
            data.animestate = animestate;
            return data;
        }
    }
}
