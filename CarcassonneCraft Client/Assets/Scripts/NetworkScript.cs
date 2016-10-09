using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CarcassonneCraft
{
    public class NetworkScript : MonoBehaviour
    {
        float frameSpan = 0;

        void Start()
        {
            Env.Init();
            BlockTypes.Init();
            World.Init();

            PlayerInitDatas players = CreateDummyPlayerInitDatas();
            Players.AddPlayerInitDatas(players);

            /*GCli.SetPacketHandler(MessageType.Snapshot, DataType.Bytes, SnapshotHandler);
            GCli.SetPacketHandler(MessageType.Join, DataType.Bytes, JoinHandler);
            GCli.SetPacketHandler(MessageType.BroadcastMessage, DataType.Bytes, BroadcastMessageHandler);*/

            StartCoroutine("AutoLoadChunk");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            //GCli.Receive();

            frameSpan += Time.deltaTime;
            if (frameSpan >= 0.1f)
            {
                frameSpan = 0;

                //ObjectManager.PushData();
            }

            //AutoUnloadPrefab();
        }

        void FixedUpdate()
        {
            //ObjectManager.FixedUpdate(ObjectType.Player, Time.deltaTime);
        }

        void OnDestroy()
        {
            GCli.Shutdown();
        }

        /*public void JoinHandler(NetConnection connection, object data)
        {
            PlayerInitData init = GCli.Deserialize<PlayerInitData>((byte[])data);
            ObjectManager.AddObject(new OtherPlayer(init), ObjectType.Player);
        }

        public void SnapshotHandler(NetConnection connection, object data)
        {
            SyncDatas syncs = GCli.Deserialize<SyncDatas>((byte[])data);
            ObjectManager.UpdatePlayerSyncData(syncs.syncs);
        }*/

        public PlayerInitDatas CreateDummyPlayerInitDatas()
        {
            PlayerSyncData sync = new PlayerSyncData();
            sync.userid = 1;
            sync.ypos = 1.0f;
            PlayerInitData init = new PlayerInitData();
            init.sync = sync;
            init.username = "Yas";
            Env.CreateDefaultSelects(init.selects);
            PlayerInitDatas datas = new PlayerInitDatas();
            datas.player = init;

            PlayerSyncData sync2 = new PlayerSyncData();
            sync2.userid = 3;
            sync2.ypos = 1.0f;
            sync2.zpos = 5.0f;
            OtherPlayerInitData init2 = new OtherPlayerInitData();
            init2.sync = sync2;
            init2.username = "Unitychan";
            datas.otherplayers.Add(init2);

            return datas;
        }

        /*public void SendInputField()
        {
            GameObject field = GameObject.Find("TextField");
            string text = field.GetComponent<InputField>().text;
            field.GetComponent<InputField>().text = "";
            if (text == "")
            {
                return;
            }
            GCli.Send(MessageType.SendMessage, text, NetDeliveryMethod.ReliableOrdered);            
        }

        public void BroadcastMessageHandler(NetConnection connection, object data)
        {
            MessageData message = GCli.Deserialize<MessageData>((byte[])data);

            ObjectManager.CreateBalloon(message);
        }*/

        IEnumerator AutoLoadChunk()
        {
            int loadChunkCount = (int)Mathf.Pow(Env.AutoLoadDistance * 2 + 1, 2);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            int spanMilliseconds = (int)(Env.LoadMilliseconds / loadChunkCount);

            int count = 0;
            while (true)
            {
                sw.Reset();
                sw.Start();

                int xOffset = count / (Env.AutoLoadDistance * 2 + 1) - Env.AutoLoadDistance;
                int zOffset = count % (Env.AutoLoadDistance * 2 + 1) - Env.AutoLoadDistance;

                XZNum playerPos = Players.GetPlayerPos();
                XZNum loadChunkPos = new XZNum(playerPos.xnum + xOffset * Env.XBlockN, playerPos.znum + zOffset * Env.ZBlockN);

                if (Env.IsInsideWorld(loadChunkPos))
                {
                    XZNum areasNum = Env.GetAreasNum(loadChunkPos);
                    int areaid = Players.GetSelectArea(areasNum);

                    if (!World.IsAreaLoaded(areaid, areasNum))
                    {
                        DummyInquiryArea(areaid);
                    }
                    else
                    {
                        LoadChunk(areaid, loadChunkPos);
                        //Debug.Log(loadChunkPos.xnum + "," + loadChunkPos.znum);
                        //Debug.Log("Load Chunk");
                    }
                    //Debug.Log(areasNum.xnum + "," + areasNum.znum);
                    //Debug.Log(areaid);
                }

                count++;
                if (count >= loadChunkCount)
                {
                    count = 0;
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds < spanMilliseconds)
                {
                    yield return new WaitForSeconds((spanMilliseconds - sw.ElapsedMilliseconds) / 1000);
                }
            }
        }

        void DummyInquiryArea(int areaid)
        {
            AreaInfo info = new AreaInfo();
            info.areaid = areaid;
            info.areaname = "Default";
            info.userid = 1;
            info.username = "Master";
            info.rating = 0;
            info.rated = false;
            UserInfo user0 = new UserInfo();
            user0.userid = 1;
            user0.username = "Master";
            UserInfo user1 = new UserInfo();
            user1.userid = 3;
            user1.username = "Unitychan";
            info.editusers.Add(user0);
            info.editusers.Add(user1);
            info.xareasnum = (areaid - 1) % Env.XAreasN;
            info.zareasnum = (areaid - 1) / Env.XAreasN;

            World.AddAreaInfo(info);
        }

        void LoadChunk(int areaid, XZNum loadChunkPos)
        {
            if(World.IsChunkLoaded(areaid, loadChunkPos))
            {
                if(!World.IsPrefabLoaded(areaid, loadChunkPos))
                {
                    World.LoadPrefab(areaid, loadChunkPos);
                }
            }
            else
            {
                if (Env.IsDefaultArea(areaid, loadChunkPos))
                {
                    World.LoadDefaultChunk(loadChunkPos);
                    //Debug.Log(loadChunkPos.xnum + "," + loadChunkPos.znum);
                }
                else
                {
                    XZNum loadChunkNum = Env.GetChunkNum(loadChunkPos);
                    DummyInquiryChunk(areaid, loadChunkNum);
                }
            }
        }

        void DummyInquiryChunk(int areaid, XZNum loadChunkNum)
        {
            Debug.Log("Assert");
        }

        void AutoUnloadPrefab()
        {
            XZNum playerPos = Players.GetPlayerPos();

            for (int x = playerPos.xnum - Env.XBlockN * Env.AutoUnloadDistance; x <= playerPos.xnum + Env.XBlockN * Env.AutoUnloadDistance; x += Env.XBlockN)
            {
                for (int z = playerPos.znum - Env.ZBlockN * Env.AutoUnloadDistance; z <= playerPos.znum + Env.ZBlockN * Env.AutoUnloadDistance; z += Env.XBlockN)
                {
                    if (Env.IsInsideWorld(new XZNum(x, z)))
                    {
                        if (x == playerPos.xnum - Env.XBlockN * Env.AutoUnloadDistance || x == playerPos.xnum + Env.XBlockN * Env.AutoUnloadDistance ||
                            z == playerPos.znum - Env.ZBlockN * Env.AutoUnloadDistance || z == playerPos.znum + Env.ZBlockN * Env.AutoUnloadDistance)
                        {
                            XZNum areasNum = Env.GetAreasNum(new XZNum(x, z));
                            int areaid = Players.GetSelectArea(areasNum);
                            World.UnLoadPrefab(areaid, new XZNum(x, z));
                        }
                    }
                }
            }
        }
    }
}
