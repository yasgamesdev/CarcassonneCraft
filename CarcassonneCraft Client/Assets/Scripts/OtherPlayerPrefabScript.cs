using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CarcassonneCraft
{
    class OtherPlayerPrefabScript : MonoBehaviour
    {
        [SerializeField]
        GameObject model;
        [SerializeField]
        Transform neck;
        float xrot;

        Animator anime;
        int animeState;

        public void Init(OtherPlayerInitData init)
        {
            anime = GetComponentInChildren<Animator>();

            transform.position = new Vector3(init.sync.xpos, init.sync.ypos, init.sync.zpos);
            this.xrot = init.sync.xrot;
            model.transform.localEulerAngles = new Vector3(0, init.sync.yrot, 0);
            this.animeState = init.sync.animestate;

            SetAnime();
        }

        void SetAnime()
        {
            if (animeState == 0)
            {
                anime.SetTrigger("Idle");
            }
            else if (animeState == 1)
            {
                anime.SetTrigger("Walk");
            }
        }

        public void Interpolation(PlayerSyncData latest, float delta)
        {
            Vector3 latestPos = new Vector3(latest.xpos, latest.ypos, latest.zpos);
            transform.position = Vector3.Lerp(transform.position, latestPos, delta * 10.0f);

            model.transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(model.transform.localEulerAngles.y, latest.yrot, delta * 10.0f), 0);

            if (this.animeState != latest.animestate)
            {
                this.animeState = latest.animestate;
                SetAnime();
            }
        }

        void LateUpdate()
        {
            Quaternion memory = transform.localRotation;
            transform.localRotation = Quaternion.identity;
            neck.Rotate(new Vector3(xrot, 0, 0), Space.World);
            transform.localRotation = memory;
        }
    }
}
