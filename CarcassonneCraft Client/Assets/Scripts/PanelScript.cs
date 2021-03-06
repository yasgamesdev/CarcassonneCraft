﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CarcassonneCraft
{
    class PanelScript : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;
        [SerializeField]
        GameObject content;
        [SerializeField]
        GameObject forkWindow;
        [SerializeField]
        InputField forkInput;
        [SerializeField]
        GameObject addWindow;
        [SerializeField]
        InputField addInput;
        [SerializeField]
        GameObject removeWindow;
        [SerializeField]
        InputField removeInput;
        [SerializeField]
        GameObject editorWindow;
        [SerializeField]
        Text editorText;

        Player player = null;
        List<GameObject> nodes = new List<GameObject>();

        AreaInfo currentAreaInfo;

        void Update()
        {
            if(player == null)
            {
                player = Players.GetPlayer();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Tab) && !(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
                {
                    panel.SetActive(!panel.activeSelf);
                    player.SetControllerActive(!IsPanelOpen());
                }
            }
        }

        public bool IsPanelOpen()
        {
            return panel.activeSelf;
        }

        public void CreateList()
        {
            CloseAllWindows();

            foreach (GameObject node in nodes)
            {
                Destroy(node);
            }
            nodes.Clear();

            XZNum playerPos = Players.GetPlayerPos();
            XZNum areasNum = Env.GetAreasNum(playerPos);

            List<AreaInfo> infos = World.GetAllAreaInfo(areasNum);
            /*for (int i = 0; i < 20; i++)
            {
                infos.Add(infos[0]);
            }*/

            for (int i = 0; i < infos.Count; i++)
            {
                GameObject node = GameObject.Instantiate(Resources.Load("Node"), content.transform) as GameObject;
                node.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * i);
                node.GetComponent<NodeScript>().Init(infos[i], this);
                nodes.Add(node);
            }

            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 30 * infos.Count);
        }

        public void CloseAllWindows()
        {
            forkWindow.SetActive(false);
            addWindow.SetActive(false);
            removeWindow.SetActive(false);
            editorWindow.SetActive(false);
        }

        public void OpenForkWindow(AreaInfo info)
        {
            currentAreaInfo = info;
            CloseAllWindows();
            forkWindow.SetActive(true);
        }

        public void OpenAddWindow(AreaInfo info)
        {
            currentAreaInfo = info;
            CloseAllWindows();
            addWindow.SetActive(true);
        }

        public void OpenRemoveWindow(AreaInfo info)
        {
            currentAreaInfo = info;
            CloseAllWindows();
            removeWindow.SetActive(true);
        }

        public void OpenEditorsWindow(AreaInfo info)
        {
            currentAreaInfo = info;
            CloseAllWindows();

            editorText.text = "";
            foreach (UserInfo user in info.editusers)
            {
                editorText.text += user.username + "\n";
            }

            editorWindow.SetActive(true);
        }

        public void RequestAllAreaInfo()
        {
            XZNum playerPos = Players.GetPlayerPos();
            XZNum areasNum = Env.GetAreasNum(playerPos);

            RequestAllAreaInfo info = new RequestAllAreaInfo();
            info.xareasnum = areasNum.xnum;
            info.zareasnum = areasNum.znum;
            GCli.Send(MessageType.RequestAllAreaInfo, GCli.Serialize<RequestAllAreaInfo>(info), NetDeliveryMethod.ReliableOrdered);
        }

        public void SendFork()
        {
            CloseAllWindows();

            AreaInfo info = currentAreaInfo;
            string areaname = forkInput.text;

            RequestForkInfo fork = new RequestForkInfo();
            fork.areaid = currentAreaInfo.areaid;
            fork.forkname = forkInput.text;
            fork.xareasnum = currentAreaInfo.xareasnum;
            fork.zareasnum = currentAreaInfo.zareasnum;
            GCli.Send(MessageType.RequestFork, GCli.Serialize<RequestForkInfo>(fork), NetDeliveryMethod.ReliableOrdered);
        }

        public void SendAddEditor()
        {
            CloseAllWindows();

            AreaInfo info = currentAreaInfo;
            string username = addInput.text;

            EditorInfo editor = new EditorInfo();
            editor.areaid = info.areaid;
            editor.username = username;
            GCli.Send(MessageType.RequestAddEditor, GCli.Serialize<EditorInfo>(editor), NetDeliveryMethod.ReliableOrdered);
        }

        public void SendRemoveEditor()
        {
            CloseAllWindows();

            AreaInfo info = currentAreaInfo;
            string username = removeInput.text;

            EditorInfo editor = new EditorInfo();
            editor.areaid = info.areaid;
            editor.username = username;
            GCli.Send(MessageType.RequestRemoveEditor, GCli.Serialize<EditorInfo>(editor), NetDeliveryMethod.ReliableOrdered);
        }
    }
}
