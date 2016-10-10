﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    class Program
    {
        static Task task;
        static bool exit = false;
        const float frameSpan = 1.0f / 30.0f;
        static int sendCount = 0;

        static int port;
        static string sqlpath;

        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "/Setting/setting.txt";
            if (!ReadFile(path))
            {
                return;
            }

            task = Task.Run(() => {
                Server();
            });

            while (true)
            {
                Console.WriteLine(@"type exit for close server");
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
            }
            exit = true;

            task.Wait();
        }

        static void Server()
        {
            Env.Init();
            Players.Init();
            World.Init();

            GSQLite.Open(sqlpath);

            //AreaInfo info = GSQLite.GetAreaInfo(1, 1);

            GSrv.Init();
            GSrv.SetConnectPacketHandler(ConnectHandler);
            GSrv.SetDisconnectPacketHandler(DisconnectHandler);
            GSrv.SetDebugPacketHandler(DebugHandler);
            /*GSrv.SetPacketHandler(MessageType.Login, DataType.Bytes, LoginHandler);
            GSrv.SetPacketHandler(MessageType.Register, DataType.Bytes, RegisterHandler);
            GSrv.SetPacketHandler(MessageType.Push, DataType.Bytes, PushHandler);
            GSrv.SetPacketHandler(MessageType.SendMessage, DataType.String, SendMessageHandler);*/
            GSrv.SetPacketHandler(MessageType.RequestAreaInfo, DataType.Int32, RequestAreaInfoHandler);
            GSrv.SetPacketHandler(MessageType.RequestChunkDiffs, DataType.Bytes, RequestChunkDiffsHandler);
            GSrv.SetPacketHandler(MessageType.RequestAllAreaInfo, DataType.Bytes, RequestAllAreaInfoHandler);
            GSrv.SetPacketHandler(MessageType.PressGoodButton, DataType.Bytes, PressGoodButtonHandler);
            GSrv.SetPacketHandler(MessageType.RequestFork, DataType.Bytes, RequestForkHandler);
            GSrv.SetPacketHandler(MessageType.RequestSelect, DataType.Bytes, RequestSelectHandler);
            GSrv.SetPacketHandler(MessageType.RequestAddEditor, DataType.Bytes, RequestAddEditorHandler);
            GSrv.SetPacketHandler(MessageType.RequestRemoveEditor, DataType.Bytes, RequestRemoveEditorHandler);
            GSrv.SetPacketHandler(MessageType.SetBlock, DataType.Bytes, SetBlockHandler);
            GSrv.SetPacketHandler(MessageType.ResetBlock, DataType.Bytes, ResetBlockHandler);
            GSrv.Listen("CarcassonneCraft0.1", port);

            while (!exit)
            {
                DateTime startTime = DateTime.Now;

                GSrv.Receive();

                sendCount++;
                if (sendCount == 3)
                {
                    sendCount = 0;

                    /*SyncDatas syncs = ObjectManager.GetSyncDatas();
                    GSrv.SendToAll(MessageType.Snapshot, GSrv.Serialize<SyncDatas>(syncs), NetDeliveryMethod.UnreliableSequenced);*/
                }

                TimeSpan span = DateTime.Now - startTime;
                if (span.TotalMilliseconds < frameSpan * 1000)
                {
                    Thread.Sleep((int)(frameSpan * 1000) - (int)span.TotalMilliseconds);
                }
            }

            GSrv.Shutdown();
            GSQLite.SaveAll();
            GSQLite.Close();
        }

        static bool ReadFile(string path)
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

                        if (line.StartsWith("Port="))
                        {
                            string sub = line.Substring("Port=".Length);
                            if (!int.TryParse(sub, out port))
                            {
                                throw new Exception();
                            }
                        }
                        else if (line.StartsWith("SQLite="))
                        {
                            string sub = line.Substring("SQLite=".Length);
                            sqlpath = Directory.GetCurrentDirectory() + "/SQLite/" + sub;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        static public void ConnectHandler(NetConnection connection, object data)
        {
            PlayerInitData player = GSQLite.LoginUser("hiro", new byte[32]);
            Players.AddPlayer(connection, player);
            GSrv.Send(MessageType.LoginSuccess, GSrv.Serialize<PlayerInitData>(player), connection, NetDeliveryMethod.ReliableOrdered);
        }

        static public void DisconnectHandler(NetConnection connection, object data)
        {
            Players.DeletePlayer(connection);
        }

        static public void DebugHandler(NetConnection connection, object data)
        {
            Console.WriteLine((string)data);
        }

        static public void RequestAreaInfoHandler(NetConnection connection, object data)
        {
            int areaid = (int)data;

            if(!Players.IsAuthDone(connection))
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            AreaInfo info = GSQLite.GetAreaInfo(areaid, userid);
            if(info != null)
            {
                GSrv.Send(MessageType.ReplyAreaInfo, GSrv.Serialize<AreaInfo>(info), connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void RequestChunkDiffsHandler(NetConnection connection, object data)
        {
            RequestChunkInfo request = GSrv.Deserialize<RequestChunkInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            Chunk chunk = GSQLite.GetChunkDiffs(request);
            if(chunk != null)
            {
                GSrv.Send(MessageType.ReplyChunkDiffs, GSrv.Serialize<Chunk>(chunk), connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void RequestAllAreaInfoHandler(NetConnection connection, object data)
        {
            RequestAllAreaInfo request = GSrv.Deserialize<RequestAllAreaInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            AreaInfos infos = GSQLite.GetAllAreaInfo(request, userid);

            GSrv.Send(MessageType.ReplyAllAreaInfo, GSrv.Serialize<AreaInfos>(infos), connection, NetDeliveryMethod.ReliableOrdered);
        }

        static public void PressGoodButtonHandler(NetConnection connection, object data)
        {
            PressGoodInfo push = GSrv.Deserialize<PressGoodInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            GSQLite.AcceptGoodRequest(push, userid);

            AreaInfo info = GSQLite.GetAreaInfo(push.areaid, userid);
            if (info != null)
            {
                GSrv.Send(MessageType.ReplyLatestRating, GSrv.Serialize<AreaInfo>(info), connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void RequestForkHandler(NetConnection connection, object data)
        {
            RequestForkInfo fork = GSrv.Deserialize<RequestForkInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            AreaInfo info = GSQLite.Fork(fork, userid);
            if (info != null)
            {
                GSrv.Send(MessageType.ReplyFork, GSrv.Serialize<AreaInfo>(info), connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void RequestSelectHandler(NetConnection connection, object data)
        {
            SelectInfo select = GSrv.Deserialize<SelectInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            if(select.selectindex < 0 || Env.XAreasN * Env.ZAreasN <= select.selectindex)
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            GSQLite.Select(select, userid);
            Players.UpdateSelect(connection, select);

            GSrv.Send(MessageType.ReplySelect, GSrv.Serialize<SelectInfo>(select), connection, NetDeliveryMethod.ReliableOrdered);
        }

        static public void RequestAddEditorHandler(NetConnection connection, object data)
        {
            EditorInfo editor = GSrv.Deserialize<EditorInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            AreaInfo info = GSQLite.AddEditor(editor, userid);
            if (info != null)
            {
                GSrv.Send(MessageType.ReplyAddEditor, GSrv.Serialize<AreaInfo>(info), connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void RequestRemoveEditorHandler(NetConnection connection, object data)
        {
            EditorInfo editor = GSrv.Deserialize<EditorInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            AreaInfo info = GSQLite.RemoveEditor(editor, userid);
            if (info != null)
            {
                GSrv.Send(MessageType.ReplyRemoveEditor, GSrv.Serialize<AreaInfo>(info), connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void SetBlockHandler(NetConnection connection, object data)
        {
            SetBlockInfo block = GSrv.Deserialize<SetBlockInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            int worldx = block.x + (block.xchunknum * Env.XBlockN) + (block.xareasnum * Env.XBlockN * Env.XChunkN);
            int worldz = block.z + (block.zchunknum * Env.ZBlockN) + (block.zareasnum * Env.ZBlockN * Env.ZChunkN);

            if(!Env.IsInsideWorld(worldx, block.y, worldz))
            {
                return;
            }

            int blocktype = Env.GetBlockType(worldx, block.y, worldz);
            if(block.blocktype == blocktype)
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            bool success = GSQLite.SetBlock(block, userid);

            if(success)
            {
                GSrv.SendToAll(MessageType.BroadcastSetBlock, GSrv.Serialize<SetBlockInfo>(block), NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void ResetBlockHandler(NetConnection connection, object data)
        {
            SetBlockInfo block = GSrv.Deserialize<SetBlockInfo>((byte[])data);

            if (!Players.IsAuthDone(connection))
            {
                return;
            }

            int worldx = block.x + (block.xchunknum * Env.XBlockN) + (block.xareasnum * Env.XBlockN * Env.XChunkN);
            int worldz = block.z + (block.zchunknum * Env.ZBlockN) + (block.zareasnum * Env.ZBlockN * Env.ZChunkN);

            if (!Env.IsInsideWorld(worldx, block.y, worldz))
            {
                return;
            }

            int userid = Players.GetUserID(connection);
            bool success = GSQLite.ResetBlock(block, userid);

            if (success)
            {
                GSrv.SendToAll(MessageType.BroadcastResetBlock, GSrv.Serialize<SetBlockInfo>(block), NetDeliveryMethod.ReliableOrdered);
            }
        }

        /*static public void LoginHandler(NetConnection connection, object data)
        {
            UserNameAndPassword info = GSrv.Deserialize<UserNameAndPassword>((byte[])data);

            PlayerInitData init = GSQLite.LoginUser(info.name, info.passwd);
            if (init != null)
            {
                Player player = new Player(connection, init);
                ObjectManager.AddObject(player, ObjectType.Player);

                GSrv.SendToAll(MessageType.Join, GSrv.Serialize<PlayerInitData>(init), NetDeliveryMethod.ReliableOrdered);

                InitDatas inits = ObjectManager.GetInitDatas();
                GSrv.Send(MessageType.LoginSuccess, GSrv.Serialize<InitDatas>(inits), connection, NetDeliveryMethod.ReliableOrdered);
            }
            else
            {
                GSrv.Send(MessageType.LoginFailed, "Login Failed", connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void RegisterHandler(NetConnection connection, object data)
        {
            UserNameAndPassword info = GSrv.Deserialize<UserNameAndPassword>((byte[])data);

            if (info.name.Length >= 1)
            {
                if (GSQLite.IsAlreadyExistUser(info.name))
                {
                    GSrv.Send(MessageType.RegisterFailed, "Name Already Used", connection, NetDeliveryMethod.ReliableOrdered);
                }
                else
                {
                    int userid = GSQLite.CreateUser(info.name, info.passwd);

                    PlayerInitData init = GSQLite.GetPlayer(info.name);
                    Player player = new Player(connection, init);
                    ObjectManager.AddObject(player, ObjectType.Player);

                    GSrv.SendToAll(MessageType.Join, GSrv.Serialize<PlayerInitData>(init), NetDeliveryMethod.ReliableOrdered);

                    InitDatas inits = ObjectManager.GetInitDatas();
                    GSrv.Send(MessageType.LoginSuccess, GSrv.Serialize<InitDatas>(inits), connection, NetDeliveryMethod.ReliableOrdered);
                }
            }
            else
            {
                GSrv.Send(MessageType.RegisterFailed, "Register Failed", connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        static public void PushHandler(NetConnection connection, object data)
        {
            PushData push = GSrv.Deserialize<PushData>((byte[])data);

            ObjectManager.Push(connection, push);
        }

        static public void SendMessageHandler(NetConnection connection, object data)
        {
            string text = (string)data;

            int userid = ObjectManager.GetUserID(connection);
            MessageData message = new MessageData();
            message.userid = userid;
            message.message = text;

            GSrv.SendToAll(MessageType.BroadcastMessage, GSrv.Serialize<MessageData>(message), NetDeliveryMethod.ReliableOrdered);
        }*/
    }
}
