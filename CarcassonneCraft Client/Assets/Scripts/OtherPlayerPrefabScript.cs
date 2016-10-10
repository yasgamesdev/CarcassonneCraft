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
        //float xrot;
        float previous_xrot;
        float latest_xrot;

        Animator anime;
        int animeState;

        public void Init(OtherPlayerInitData init)
        {
            anime = GetComponentInChildren<Animator>();

            transform.position = new Vector3(init.sync.xpos, init.sync.ypos, init.sync.zpos);
            //this.xrot = init.sync.xrot;
            previous_xrot = init.sync.xrot;
            latest_xrot = init.sync.xrot;
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
            transform.position = Vector3.Lerp(transform.position, latestPos, delta * 5.0f);

            model.transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(model.transform.localEulerAngles.y, latest.yrot, delta * 5.0f), 0);
            //xrot = latest.xrot;
            latest_xrot = latest.xrot;

            if (this.animeState != latest.animestate)
            {
                this.animeState = latest.animestate;
                SetAnime();
            }
        }

        void LateUpdate()
        {
            previous_xrot = Mathf.LerpAngle(previous_xrot, latest_xrot, Time.deltaTime * 5.0f);

            Quaternion memory = model.transform.localRotation;
            model.transform.rotation = Quaternion.identity;
            neck.Rotate(new Vector3(previous_xrot, 0, 0), Space.World);
            model.transform.localRotation = memory;
        }
    }
}
