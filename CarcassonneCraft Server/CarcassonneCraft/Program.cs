using Lidgren.Network;
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
            Players.Init();
            World.Init();

            GSQLite.Open(sqlpath);

            GSrv.Init();
            GSrv.SetConnectPacketHandler(ConnectHandler);
            GSrv.SetDisconnectPacketHandler(DisconnectHandler);
            GSrv.SetDebugPacketHandler(DebugHandler);
            /*GSrv.SetPacketHandler(MessageType.Login, DataType.Bytes, LoginHandler);
            GSrv.SetPacketHandler(MessageType.Register, DataType.Bytes, RegisterHandler);
            GSrv.SetPacketHandler(MessageType.Push, DataType.Bytes, PushHandler);
            GSrv.SetPacketHandler(MessageType.SendMessage, DataType.String, SendMessageHandler);*/
            GSrv.Listen("CarcassonneCraft0.1", port);

            while (!exit)
            {
                DateTime startTime = DateTime.Now;

                GSrv.Receive();

                sendCount++;
                if (sendCount == 2)
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

        }

        static public void DisconnectHandler(NetConnection connection, object data)
        {
            //ObjectManager.DeletePlayer(connection);
        }

        static public void DebugHandler(NetConnection connection, object data)
        {
            Console.WriteLine((string)data);
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
