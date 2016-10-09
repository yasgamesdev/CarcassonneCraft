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
        [SerializeField]
        Toggle good;
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

        bool firstTime = true;

        public void Init(AreaInfo info, PanelScript parent)
        {
            this.info = info;
            this.parent = parent;

            areaName.text = info.areaname;
            userName.text = info.username;
            rating.text = info.rating.ToString();
            good.isOn = info.rated;

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
                if(userid == info.userid)
                {
                    constractable.SetActive(true);
                    break;
                }
            }
        }

        public void GoodClicked()
        {
            if (firstTime)
            {
                firstTime = false;
            }
            else
            {
                bool value = good.isOn;
                Debug.Log("good:" + value);
            }
        }

        public void ForkPressed()
        {
            parent.OpenForkWindow(info);
        }

        public void SelectPressed()
        {
            Debug.Log("select:" + info.areaid);
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
