using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CarcassonneCraft
{
    public class NetworkScript : MonoBehaviour
    {
        //start
        [SerializeField]
        Text debugText;
        [SerializeField]
        GameObject login, register;
        [SerializeField]
        InputField loginName, loginPasswd, registerName, registerPasswd;
        [SerializeField]
        GameObject startScreen;

        string host;
        int port;

        //main
        [SerializeField]
        PanelScript panel;
        [SerializeField]
        GameObject bottom, front, back, left, right;

        float frameSpan = 0;

        void Start()
        {
            Env.Init();
            BlockTypes.Init();
            World.Init();

            if (!ReadFile(Application.dataPath + "/../Setting/setting.txt"))
            {
                return;
            }

            GCli.Init();
            GCli.SetConnectPacketHandler(ConnectHandler);
            GCli.SetDebugPacketHandler(DebugHandler);

            GCli.Connect("CarcassonneCraft0.1", host, port);

            /*GCli.Init();
            GCli.SetConnectPacketHandler(ConnectHandler);
            GCli.Connect("CarcassonneCraft0.1", "localhost", 15127);*/

            //PlayerInitDatas players = CreateDummyPlayerInitDatas();
            //Players.AddPlayerInitDatas(players);

            /*GCli.SetPacketHandler(MessageType.Snapshot, DataType.Bytes, SnapshotHandler);
            GCli.SetPacketHandler(MessageType.Join, DataType.Bytes, JoinHandler);
            GCli.SetPacketHandler(MessageType.BroadcastMessage, DataType.Bytes, BroadcastMessageHandler);*/

            //StartCoroutine("AutoLoadChunk");

            SetupBounds();
        }

        bool ReadFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            try
            {
                using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
                {
                    while (true)
                    {
                        string line = sr.ReadLine();

                        if (line == null)
                        {
                            break;
                        }

                        if (line.StartsWith("Host="))
                        {
                            string sub = line.Substring("Host=".Length);
                            host = sub;
                        }
                        else if (line.StartsWith("Port="))
                        {
                            string sub = line.Substring("Port=".Length);
                            if (!int.TryParse(sub, out port))
                            {
                                throw new Exception();
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                debugText.text = e.Message;
                return false;
            }

            return true;
        }

        void SetupBounds()
        {
            int x = Env.XBlockN * Env.XChunkN * Env.XAreasN;
            int y = Env.YBlockN;
            int z = Env.ZBlockN * Env.ZChunkN * Env.ZAreasN;

            bottom.transform.localScale = new Vector3((float)x / 10, 1, (float)z / 10);
            bottom.transform.position = new Vector3(x / 2, 0, z / 2);

            front.transform.localScale = new Vector3((float)x / 10, 1, (float)(y * 2) / 10);
            front.transform.position = new Vector3(x / 2, y, z);

            back.transform.localScale = new Vector3((float)x / 10, 1, (float)(y * 2) / 10);
            back.transform.position = new Vector3(x / 2, y, 0);

            left.transform.localScale = new Vector3((float)(y * 2) / 10, 1, (float)z / 10);
            left.transform.position = new Vector3(0, y, z / 2);

            right.transform.localScale = new Vector3((float)(y * 2) / 10, 1, (float)z / 10);
            right.transform.position = new Vector3(x, y, z / 2);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            GCli.Receive();

            frameSpan += Time.deltaTime;
            if (frameSpan >= 0.2f)
            {
                frameSpan = 0;

                if(Players.GetPlayer() != null)
                {
                    PushData push = Players.GetPushData();
                    GCli.Send(MessageType.Push, GCli.Serialize<PushData>(push), NetDeliveryMethod.UnreliableSequenced);
                }

                //ObjectManager.PushData();
            }

            //AutoUnloadPrefab();
        }

        void FixedUpdate()
        {
            Players.FixedUpdate(Time.deltaTime);
        }

        void OnDestroy()
        {
            GCli.Shutdown();
        }

        public void Login()
        {
            LoginInfo info = new LoginInfo();
            info.username = loginName.text;
            info.passwd = GenerateHash(loginPasswd.text, loginName.text);

            GCli.Send(MessageType.Login, GCli.Serialize<LoginInfo>(info), NetDeliveryMethod.ReliableOrdered);
        }

        public void SelectRegister()
        {
            login.SetActive(false);
            register.SetActive(true);
        }

        public void Register()
        {
            string name = registerName.text;
            string passwd = registerPasswd.text;

            Regex nameRegex = new Regex(@"\A[a-zA-Z0-9]{1,20}\z");
            if (!nameRegex.IsMatch(name))
            {
                debugText.text = "Name must be (1~20) characters and numbers";
                return;
            }

            Regex passwdRegex = new Regex(@"\A[a-zA-Z0-9]{6,}\z");
            if (!passwdRegex.IsMatch(passwd))
            {
                debugText.text = "Passwd must be (6~) characters and numbers";
                return;
            }

            LoginInfo info = new LoginInfo();
            info.username = name;
            info.passwd = GenerateHash(passwd, name);

            GCli.Send(MessageType.Register, GCli.Serialize<LoginInfo>(info), NetDeliveryMethod.ReliableOrdered);
        }

        public void Back()
        {
            register.SetActive(false);
            login.SetActive(true);
        }

        byte[] GenerateHash(string text, string salt)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] byteValue = Encoding.UTF8.GetBytes(text + salt);
            byte[] hash = mySHA256.ComputeHash(byteValue);
            return hash;
        }

        public void ConnectHandler(NetConnection connection, object data)
        {
            GCli.UnsetPacketHandler(MessageType.Connect);
            login.SetActive(true);
            GCli.SetPacketHandler(MessageType.LoginSuccess, DataType.Bytes, LoginSuccessHandler);
            GCli.SetPacketHandler(MessageType.LoginFailed, DataType.String, LoginFailedHandler);
            GCli.SetPacketHandler(MessageType.RegisterSuccess, DataType.Bytes, LoginSuccessHandler);
            GCli.SetPacketHandler(MessageType.RegisterFailed, DataType.String, RegisterFailedHandler);

            //GCli.UnsetPacketHandler(MessageType.Connect);
            //GCli.SetPacketHandler(MessageType.LoginSuccess, DataType.Bytes, LoginSuccessHandler);
            /*GCli.SetPacketHandler(MessageType.LoginFailed, DataType.String, LoginFailedHandler);
            GCli.SetPacketHandler(MessageType.RegisterSuccess, DataType.Bytes, RegisterSuccessHandler);
            GCli.SetPacketHandler(MessageType.RegisterFailed, DataType.String, RegisterFailedHandler);*/
        }

        public void DebugHandler(NetConnection connection, object data)
        {
            string message = (string)data;
            debugText.text = message;
        }

        public void LoginSuccessHandler(NetConnection connection, object data)
        {
            GCli.UnsetPacketHandler(MessageType.Debug);
            GCli.UnsetPacketHandler(MessageType.LoginSuccess);
            GCli.UnsetPacketHandler(MessageType.LoginFailed);
            GCli.UnsetPacketHandler(MessageType.RegisterSuccess);
            GCli.UnsetPacketHandler(MessageType.RegisterFailed);
            startScreen.SetActive(false);

            PlayerInitData init = GCli.Deserialize<PlayerInitData>((byte[])data);
            PlayerInitDatas inits = new PlayerInitDatas();
            inits.player = init;

            Players.AddPlayerInitDatas(inits);

            GCli.SetPacketHandler(MessageType.ReplyAreaInfo, DataType.Bytes, ReplyAreaInfoHandler);
            GCli.SetPacketHandler(MessageType.ReplyFork, DataType.Bytes, ReplyForkHandler);
            GCli.SetPacketHandler(MessageType.ReplyAllAreaInfo, DataType.Bytes, ReplyAllAreaInfoHandler);
            GCli.SetPacketHandler(MessageType.ReplySelect, DataType.Bytes, ReplySelectHandler);
            GCli.SetPacketHandler(MessageType.ReplyChunkDiffs, DataType.Bytes, ReplyChunkDiffsHandler);
            GCli.SetPacketHandler(MessageType.ReplyLatestRating, DataType.Bytes, ReplyLatestRatingHandler);
            GCli.SetPacketHandler(MessageType.ReplyAddEditor, DataType.Bytes, ReplyAddEditorHandler);
            GCli.SetPacketHandler(MessageType.ReplyRemoveEditor, DataType.Bytes, ReplyRemoveEditorHandler);
            GCli.SetPacketHandler(MessageType.BroadcastSetBlock, DataType.Bytes, BroadcastSetBlockHandler);
            GCli.SetPacketHandler(MessageType.BroadcastResetBlock, DataType.Bytes, BroadcastResetBlockHandler);
            GCli.SetPacketHandler(MessageType.Snapshot, DataType.Bytes, SnapshotHandler);
            GCli.SetPacketHandler(MessageType.ReplyInitData, DataType.Bytes, ReplyInitDataHandler);

            StartCoroutine("AutoLoadChunk");
        }

        public void LoginFailedHandler(NetConnection connection, object data)
        {
            debugText.text = (string)data;
        }

        /*public void RegisterSuccessHandler(NetConnection connection, object data)
        {
            //InitDatas inits = GCli.Deserialize<InitDatas>((byte[])data);
            //int last = inits.inits.Count - 1;
            //Debug.Log(inits.inits[last].name);
            //Debug.Log(inits.inits[last].sync.userid);
            //LoadScene(inits);
        }*/

        public void RegisterFailedHandler(NetConnection connection, object data)
        {
            debugText.text = (string)data;
        }

        public void ReplyAreaInfoHandler(NetConnection connection, object data)
        {
            AreaInfo info = GCli.Deserialize<AreaInfo>((byte[])data);

            World.AddAreaInfo(info);
        }

        public void ReplyForkHandler(NetConnection connection, object data)
        {
            AreaInfo info = GCli.Deserialize<AreaInfo>((byte[])data);

            World.AddAreaInfo(info);
            if(panel.IsPanelOpen())
            {
                panel.CreateList();
            }
        }

        public void ReplyAllAreaInfoHandler(NetConnection connection, object data)
        {
            AreaInfos infos = GCli.Deserialize<AreaInfos>((byte[])data);

            World.AddAreaInfos(infos);
            if (panel.IsPanelOpen())
            {
                panel.CreateList();
            }
        }

        public void ReplySelectHandler(NetConnection connection, object data)
        {
            SelectInfo select = GCli.Deserialize<SelectInfo>((byte[])data);

            int xareasnum = select.selectindex % Env.XAreasN;
            int zareasnum = select.selectindex / Env.XAreasN;
            int oldAreaID = Players.GetSelectArea(new XZNum(xareasnum, zareasnum));

            World.UnLoadAreaPrefab(oldAreaID, new XZNum(xareasnum, zareasnum));

            Players.UpdateSelect(select);
            if (panel.IsPanelOpen())
            {
                panel.CreateList();
            }
        }

        public void ReplyChunkDiffsHandler(NetConnection connection, object data)
        {
            Chunk chunk = GCli.Deserialize<Chunk>((byte[])data);

            World.LoadChunk(chunk);
        }

        public void ReplyLatestRatingHandler(NetConnection connection, object data)
        {
            AreaInfo info = GCli.Deserialize<AreaInfo>((byte[])data);

            World.AddAreaInfo(info);
            if (panel.IsPanelOpen())
            {
                panel.CreateList();
            }
        }

        public void ReplyAddEditorHandler(NetConnection connection, object data)
        {
            AreaInfo info = GCli.Deserialize<AreaInfo>((byte[])data);

            World.AddAreaInfo(info);
            if (panel.IsPanelOpen())
            {
                panel.CreateList();
            }
        }

        public void ReplyRemoveEditorHandler(NetConnection connection, object data)
        {
            AreaInfo info = GCli.Deserialize<AreaInfo>((byte[])data);

            World.AddAreaInfo(info);
            if (panel.IsPanelOpen())
            {
                panel.CreateList();
            }
        }

        public void BroadcastSetBlockHandler(NetConnection connection, object data)
        {
            SetBlockInfo block = GCli.Deserialize<SetBlockInfo>((byte[])data);

            World.SetBlock(block);
        }

        public void BroadcastResetBlockHandler(NetConnection connection, object data)
        {
            SetBlockInfo block = GCli.Deserialize<SetBlockInfo>((byte[])data);

            World.ResetBlock(block);
        }

        /*public void JoinHandler(NetConnection connection, object data)
        {
            PlayerInitData init = GCli.Deserialize<PlayerInitData>((byte[])data);
            ObjectManager.AddObject(new OtherPlayer(init), ObjectType.Player);
        }*/

        public void SnapshotHandler(NetConnection connection, object data)
        {
            PlayerSyncDatas syncs = GCli.Deserialize<PlayerSyncDatas>((byte[])data);

            Players.UpdatePlayerSyncData(syncs.syncs);
        }

        public void ReplyInitDataHandler(NetConnection connection, object data)
        {
            OtherPlayerInitData other = GCli.Deserialize<OtherPlayerInitData>((byte[])data);

            Players.AddOtherPlayer(other);
        }

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
                        //DummyInquiryArea(areaid);
                        GCli.Send(MessageType.RequestAreaInfo, areaid, NetDeliveryMethod.ReliableOrdered);
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
                    XZNum areasNum = Env.GetAreasNum(loadChunkPos);
                    XZNum loadChunkNum = Env.GetChunkNum(loadChunkPos);

                    RequestChunkInfo chunk = new RequestChunkInfo();
                    chunk.areaid = areaid;
                    chunk.xareasnum = areasNum.xnum;
                    chunk.zareasnum = areasNum.znum;
                    chunk.xchunknum = loadChunkNum.xnum;
                    chunk.zchunknum = loadChunkNum.znum;

                    GCli.Send(MessageType.RequestChunkDiffs, GCli.Serialize<RequestChunkInfo>(chunk), NetDeliveryMethod.ReliableOrdered);
                    //DummyInquiryChunk(areaid, loadChunkNum);
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
