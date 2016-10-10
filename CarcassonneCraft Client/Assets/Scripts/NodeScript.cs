using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CarcassonneCraft
{
    class NodeScript : MonoBehaviour
    {
        [SerializeField]
        Text areaName;
        [SerializeField]
        Text userName;
        [SerializeField]
        Text rating;
        //[SerializeField]
        //Toggle good;
        [SerializeField]
        GameObject hearticon;
        [SerializeField]
        GameObject selectButton;
        [SerializeField]
        GameObject addEditorButton;
        [SerializeField]
        GameObject removeEditorButton;
        [SerializeField]
        GameObject deleteButton;
        [SerializeField]
        GameObject constractable;

        AreaInfo info;
        PanelScript parent;

        //bool firstTime = true;

        public void Init(AreaInfo info, PanelScript parent)
        {
            this.info = info;
            this.parent = parent;

            areaName.text = info.areaname;
            userName.text = info.username;
            rating.text = info.rating.ToString();
            hearticon.SetActive(info.rated);
            //good.isOn = info.rated;

            int areaid = Players.GetSelectArea(new XZNum(info.xareasnum, info.zareasnum));
            if(areaid == info.areaid)
            {
                selectButton.SetActive(false);
            }

            int userid = Players.GetPlayerUserID();
            if(userid != info.userid)
            {
                addEditorButton.SetActive(false);
                removeEditorButton.SetActive(false);
                deleteButton.SetActive(false);
            }

            foreach(UserInfo user in info.editusers)
            {
                if(userid == user.userid)
                {
                    constractable.SetActive(true);
                    break;
                }
            }
        }

        public void GoodClicked()
        {
            /*if (firstTime)
            {
                firstTime = false;
            }
            else
            {
                //bool value = good.isOn;

                PressGoodInfo packet = new PressGoodInfo();
                packet.areaid = info.areaid;
                packet.good = !info.rated;

                GCli.Send(MessageType.PressGoodButton, GCli.Serialize<PressGoodInfo>(packet), NetDeliveryMethod.ReliableOrdered);
            }*/
            PressGoodInfo packet = new PressGoodInfo();
            packet.areaid = info.areaid;
            packet.good = !info.rated;

            GCli.Send(MessageType.PressGoodButton, GCli.Serialize<PressGoodInfo>(packet), NetDeliveryMethod.ReliableOrdered);
        }

        public void ForkPressed()
        {
            parent.OpenForkWindow(info);
        }

        public void SelectPressed()
        {
            SelectInfo select = new SelectInfo();
            select.areaid = info.areaid;
            select.selectindex = info.xareasnum + info.zareasnum * Env.XAreasN;

            GCli.Send(MessageType.RequestSelect, GCli.Serialize<SelectInfo>(select), NetDeliveryMethod.ReliableOrdered);
        }

        public void ShowEditorsPressed()
        {
            parent.OpenEditorsWindow(info);
        }

        public void AddEditorPressed()
        {
            parent.OpenAddWindow(info);
        }

        public void RemoveEditorPressed()
        {
            parent.OpenRemoveWindow(info);
        }
    }
}
