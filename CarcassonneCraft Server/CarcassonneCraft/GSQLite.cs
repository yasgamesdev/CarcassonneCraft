using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    public static class GSQLite
    {
        static SQLiteConnection con;

        public static void Open(string path)
        {
            if(System.IO.File.Exists(path))
            {
                con = new SQLiteConnection("Data Source=" + path);
                con.Open();
            }
            else
            {
                con = new SQLiteConnection("Data Source=" + path);
                con.Open();
                CreateTable();

                //CreateDummyAccount();
            }  
        }

        static void CreateTable()
        {
            //users
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE users (userid INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT NOT NULL, passwd BLOB NOT NULL);";
                cmd.ExecuteNonQuery();
            }
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                SHA256 mySHA256 = SHA256Managed.Create();
                byte[] byteValue = Encoding.UTF8.GetBytes(new Random().Next().ToString());
                byte[] hash = mySHA256.ComputeHash(byteValue);

                cmd.CommandText = @"INSERT INTO users (username, passwd) VALUES (@username, @passwd)";

                cmd.Parameters.Add("username", System.Data.DbType.String);
                cmd.Parameters.Add("passwd", System.Data.DbType.Binary);

                cmd.Parameters["username"].Value = "root";
                cmd.Parameters["passwd"].Value = hash;

                cmd.ExecuteNonQuery();
            }

            //players
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE players (
                                            userid INTEGER PRIMARY KEY,
                                            xpos REAL NOT NULL,
                                            ypos REAL NOT NULL,
                                            zpos REAL NOT NULL,
                                            xrot REAL NOT NULL,
                                            yrot REAL NOT NULL,
                                            animestate INTEGER NOT NULL);";
                cmd.ExecuteNonQuery();
            }
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO players (userid, xpos, ypos, zpos, xrot, yrot, animestate) 
                                        VALUES (@userid, @xpos, @ypos, @zpos, @xrot, @yrot, @animestate)";

                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters.Add("xpos", System.Data.DbType.Single);
                cmd.Parameters.Add("ypos", System.Data.DbType.Single);
                cmd.Parameters.Add("zpos", System.Data.DbType.Single);
                cmd.Parameters.Add("xrot", System.Data.DbType.Single);
                cmd.Parameters.Add("yrot", System.Data.DbType.Single);
                cmd.Parameters.Add("animestate", System.Data.DbType.Int32);

                cmd.Parameters["userid"].Value = 1;
                cmd.Parameters["xpos"].Value = 0;
                cmd.Parameters["ypos"].Value = 0;
                cmd.Parameters["zpos"].Value = 0;
                cmd.Parameters["xrot"].Value = 0;
                cmd.Parameters["yrot"].Value = 0;
                cmd.Parameters["animestate"].Value = 0;

                cmd.ExecuteNonQuery();
            }

            //selects
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE selects (
                                            selectid INTEGER PRIMARY KEY AUTOINCREMENT,
                                            userid INTEGER NOT NULL,
                                            selectindex INTEGER NOT NULL,
                                            areaid INTEGER NOT NULL);";
                cmd.ExecuteNonQuery();
            }

            List<int> selects = new List<int>();
            Env.CreateDefaultSelects(selects);

            for (int i = 0; i < selects.Count; i++)
            {
                using (SQLiteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO selects (userid, selectindex, areaid) 
                                        VALUES (@userid, @selectindex, @areaid)";

                    cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                    cmd.Parameters.Add("selectindex", System.Data.DbType.Int32);
                    cmd.Parameters.Add("areaid", System.Data.DbType.Int32);

                    cmd.Parameters["userid"].Value = 1;
                    cmd.Parameters["selectindex"].Value = i;
                    cmd.Parameters["areaid"].Value = selects[i];

                    cmd.ExecuteNonQuery();
                }
            }

            //areas
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE areas (
                                            areaid INTEGER PRIMARY KEY AUTOINCREMENT,
                                            areaname TEXT NOT NULL,
                                            userid INTEGER NOT NULL,
                                            xareasnum INTEGER NOT NULL,
                                            zareasnum INTEGER NOT NULL);";
                cmd.ExecuteNonQuery();
            }

            for(int z = 0; z<Env.ZAreasN; z++)
            {
                for (int x = 0; x < Env.XAreasN; x++)
                {
                    using (SQLiteCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO areas (areaname, userid, xareasnum, zareasnum) 
                                        VALUES (@areaname, @userid, @xareasnum, @zareasnum)";

                        cmd.Parameters.Add("areaname", System.Data.DbType.String);
                        cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                        cmd.Parameters.Add("xareasnum", System.Data.DbType.Int32);
                        cmd.Parameters.Add("zareasnum", System.Data.DbType.Int32);

                        cmd.Parameters["areaname"].Value = "DefaultTerrain" + (x + (z * Env.XAreasN) + 1).ToString();
                        cmd.Parameters["userid"].Value = 1;
                        cmd.Parameters["xareasnum"].Value = x;
                        cmd.Parameters["zareasnum"].Value = z;

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            //editors
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE editors (
                                            editorid INTEGER PRIMARY KEY AUTOINCREMENT,
                                            areaid INTEGER NOT NULL,
                                            userid INTEGER NOT NULL);";
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < selects.Count; i++)
            {
                using (SQLiteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO editors (areaid, userid) 
                                        VALUES (@areaid, @userid)";

                    cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                    cmd.Parameters.Add("userid", System.Data.DbType.Int32);

                    cmd.Parameters["areaid"].Value = selects[i];
                    cmd.Parameters["userid"].Value = 1;

                    cmd.ExecuteNonQuery();
                }
            }

            //rates
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE rates (
                                            rateid INTEGER PRIMARY KEY AUTOINCREMENT,
                                            areaid INTEGER NOT NULL,
                                            userid INTEGER NOT NULL);";
                cmd.ExecuteNonQuery();
            }

            //diffs
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE diffs (
                                            diffid INTEGER PRIMARY KEY AUTOINCREMENT,
                                            areaid INTEGER NOT NULL,
                                            x INTEGER NOT NULL,
                                            y INTEGER NOT NULL,
                                            z INTEGER NOT NULL,
                                            blocktype INTEGER NOT NULL,
                                            xchunknum INTEGER NOT NULL,
                                            zchunknum INTEGER NOT NULL);";
                cmd.ExecuteNonQuery();
            }
        }

        public static void CreateAccount(string name, byte[] hash)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO users (username, passwd) VALUES (@username, @passwd)";

                cmd.Parameters.Add("username", System.Data.DbType.String);
                cmd.Parameters.Add("passwd", System.Data.DbType.Binary);

                cmd.Parameters["username"].Value = name;
                cmd.Parameters["passwd"].Value = hash;

                cmd.ExecuteNonQuery();
            }

            int userid = (int)con.LastInsertRowId;

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO players (userid, xpos, ypos, zpos, xrot, yrot, animestate) 
                                        VALUES (@userid, @xpos, @ypos, @zpos, @xrot, @yrot, @animestate)";

                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters.Add("xpos", System.Data.DbType.Single);
                cmd.Parameters.Add("ypos", System.Data.DbType.Single);
                cmd.Parameters.Add("zpos", System.Data.DbType.Single);
                cmd.Parameters.Add("xrot", System.Data.DbType.Single);
                cmd.Parameters.Add("yrot", System.Data.DbType.Single);
                cmd.Parameters.Add("animestate", System.Data.DbType.Int32);

                cmd.Parameters["userid"].Value = userid;
                cmd.Parameters["xpos"].Value = 256.0f;
                cmd.Parameters["ypos"].Value = 33.0f;
                cmd.Parameters["zpos"].Value = 256.0f;
                cmd.Parameters["xrot"].Value = 0;
                cmd.Parameters["yrot"].Value = 0;
                cmd.Parameters["animestate"].Value = 0;

                cmd.ExecuteNonQuery();
            }

            List<int> selects = new List<int>();
            Env.CreateDefaultSelects(selects);
            for (int i = 0; i < selects.Count; i++)
            {
                using (SQLiteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO selects (userid, selectindex, areaid) 
                                        VALUES (@userid, @selectindex, @areaid)";

                    cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                    cmd.Parameters.Add("selectindex", System.Data.DbType.Int32);
                    cmd.Parameters.Add("areaid", System.Data.DbType.Int32);

                    cmd.Parameters["userid"].Value = userid;
                    cmd.Parameters["selectindex"].Value = i;
                    cmd.Parameters["areaid"].Value = selects[i];

                    cmd.ExecuteNonQuery();
                }
            }

            //return userid;
        }

        public static void CreateDummyAccount()
        {
            byte[] hash = new byte[32];
            CreateAccount("hiro", hash);
            CreateAccount("yas", hash);
        }

        public static void Close()
        {
            con.Close();
        }

        public static bool IsAlreadyExistUser(string name)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE username = @username";
                cmd.Parameters.Add("username", System.Data.DbType.String);
                cmd.Parameters["username"].Value = name;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

        public static PlayerInitData LoginUser(string username, byte[] hash)
        {
            int userid;

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE username = @username AND passwd = @passwd";
                cmd.Parameters.Add("username", System.Data.DbType.String);
                cmd.Parameters.Add("passwd", System.Data.DbType.Binary);
                cmd.Parameters["username"].Value = username;
                cmd.Parameters["passwd"].Value = hash;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        userid = reader.GetInt32(reader.GetOrdinal("userid"));
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            PlayerInitData init;
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM players WHERE userid = @userid";
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    PlayerSyncData sync = new PlayerSyncData();
                    sync.userid = userid;
                    sync.xpos = reader.GetFloat(reader.GetOrdinal("xpos"));
                    sync.ypos = reader.GetFloat(reader.GetOrdinal("ypos"));
                    sync.zpos = reader.GetFloat(reader.GetOrdinal("zpos"));
                    sync.xrot = reader.GetFloat(reader.GetOrdinal("xrot"));
                    sync.yrot = reader.GetFloat(reader.GetOrdinal("yrot"));
                    sync.animestate = reader.GetInt32(reader.GetOrdinal("animestate"));

                    init = new PlayerInitData();
                    init.username = username;
                    init.sync = sync;
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM selects WHERE userid = @userid";
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    int[] array = new int[Env.XAreasN * Env.ZAreasN];
                    while (reader.Read())
                    {
                        int selectindex = reader.GetInt32(reader.GetOrdinal("selectindex"));
                        int areaid = reader.GetInt32(reader.GetOrdinal("areaid"));

                        array[selectindex] = areaid;
                    }

                    init.selects.AddRange(array);
                    return init;
                }
            }
        }

        public static AreaInfo GetAreaInfo(int areaid, int userid)
        {
            AreaInfo info = new AreaInfo();
            info.areaid = areaid;

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM areas WHERE areaid = @areaid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = areaid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        info.areaname = reader.GetString(reader.GetOrdinal("areaname"));
                        info.userid = reader.GetInt32(reader.GetOrdinal("userid"));
                        info.xareasnum = reader.GetInt32(reader.GetOrdinal("xareasnum"));
                        info.zareasnum = reader.GetInt32(reader.GetOrdinal("zareasnum"));
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            //username
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE userid = @userid";
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["userid"].Value = info.userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    info.username = reader.GetString(reader.GetOrdinal("username"));
                }
            }

            //rating
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM rates WHERE areaid = @areaid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = areaid;

                info.rating = Convert.ToInt32(cmd.ExecuteScalar());
            }

            //rated
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM rates WHERE areaid = @areaid AND userid = @userid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = areaid;
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    info.rated = reader.HasRows;
                }
            }

            //editusers
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM editors WHERE areaid = @areaid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = areaid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int editorUserID = reader.GetInt32(reader.GetOrdinal("userid"));
                        string editorUserName = GSQLite.GetUserName(editorUserID);

                        UserInfo userinfo = new UserInfo();
                        userinfo.userid = editorUserID;
                        userinfo.username = editorUserName;

                        info.editusers.Add(userinfo);
                    }
                }
            }

            return info;
        }

        public static string GetUserName(int userid)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE userid = @userid";
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    return reader.GetString(reader.GetOrdinal("username"));
                }
            }
        }

        public static Chunk GetChunkDiffs(RequestChunkInfo request)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM areas WHERE areaid = @areaid AND xareasnum = @xareasnum AND zareasnum = @zareasnum";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("xareasnum", System.Data.DbType.Int32);
                cmd.Parameters.Add("zareasnum", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = request.areaid;
                cmd.Parameters["xareasnum"].Value = request.xareasnum;
                cmd.Parameters["zareasnum"].Value = request.zareasnum;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        reader.Read();

                        return null;
                    }
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM diffs WHERE areaid = @areaid AND xchunknum = @xchunknum AND zchunknum = @zchunknum";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("xchunknum", System.Data.DbType.Int32);
                cmd.Parameters.Add("zchunknum", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = request.areaid;
                cmd.Parameters["xchunknum"].Value = request.xchunknum;
                cmd.Parameters["zchunknum"].Value = request.zchunknum;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    Chunk chunk = new Chunk();
                    chunk.areaid = request.areaid;
                    chunk.xareasnum = request.xareasnum;
                    chunk.zareasnum = request.zareasnum;
                    chunk.xchunknum = request.xchunknum;
                    chunk.zchunknum = request.zchunknum;

                    while (reader.Read())
                    {
                        int x = reader.GetInt32(reader.GetOrdinal("x"));
                        int y = reader.GetInt32(reader.GetOrdinal("y"));
                        int z = reader.GetInt32(reader.GetOrdinal("z"));
                        int blocktype = reader.GetInt32(reader.GetOrdinal("blocktype"));

                        Block block = new Block();
                        block.x = x;
                        block.y = y;
                        block.z = z;
                        block.blocktype = blocktype;
                        chunk.diffs.Add(block);
                    }

                    return chunk;
                }
            }
        }

        public static AreaInfos GetAllAreaInfo(RequestAllAreaInfo request, int userid)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM areas WHERE xareasnum = @xareasnum AND zareasnum = @zareasnum";
                cmd.Parameters.Add("xareasnum", System.Data.DbType.Int32);
                cmd.Parameters.Add("zareasnum", System.Data.DbType.Int32);
                cmd.Parameters["xareasnum"].Value = request.xareasnum;
                cmd.Parameters["zareasnum"].Value = request.zareasnum;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    AreaInfos infos = new AreaInfos();
                    while (reader.Read())
                    {
                        int areaid = reader.GetInt32(reader.GetOrdinal("areaid"));
                        infos.infos.Add(GetAreaInfo(areaid, userid));
                    }

                    return infos;
                }
            }
        }

        public static void AcceptGoodRequest(PressGoodInfo push, int userid)
        {
            if(push.good)
            {
                using (SQLiteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM rates WHERE areaid = @areaid AND userid = @userid";
                    cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                    cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                    cmd.Parameters["areaid"].Value = push.areaid;
                    cmd.Parameters["userid"].Value = userid;

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return;
                        }
                    }
                }

                using (SQLiteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO rates (areaid, userid) VALUES (@areaid, @userid)";

                    cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                    cmd.Parameters.Add("userid", System.Data.DbType.Int32);

                    cmd.Parameters["areaid"].Value = push.areaid;
                    cmd.Parameters["userid"].Value = userid;

                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (SQLiteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM rates WHERE areaid = @areaid AND userid = @userid";

                    cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                    cmd.Parameters.Add("userid", System.Data.DbType.Int32);

                    cmd.Parameters["areaid"].Value = push.areaid;
                    cmd.Parameters["userid"].Value = userid;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static AreaInfo Fork(RequestForkInfo fork, int userid)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM areas WHERE areaid = @areaid AND xareasnum = @xareasnum AND zareasnum = @zareasnum";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("xareasnum", System.Data.DbType.Int32);
                cmd.Parameters.Add("zareasnum", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = fork.areaid;
                cmd.Parameters["xareasnum"].Value = fork.xareasnum;
                cmd.Parameters["zareasnum"].Value = fork.zareasnum;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }
                }
            }

            int newAreaID;
            //areas
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO areas (areaname, userid, xareasnum, zareasnum) 
                                        VALUES (@areaname, @userid, @xareasnum, @zareasnum)";

                cmd.Parameters.Add("areaname", System.Data.DbType.String);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters.Add("xareasnum", System.Data.DbType.Int32);
                cmd.Parameters.Add("zareasnum", System.Data.DbType.Int32);

                cmd.Parameters["areaname"].Value = fork.forkname;
                cmd.Parameters["userid"].Value = userid;
                cmd.Parameters["xareasnum"].Value = fork.xareasnum;
                cmd.Parameters["zareasnum"].Value = fork.zareasnum;

                cmd.ExecuteNonQuery();

                newAreaID = (int)con.LastInsertRowId;
            }

            //editors
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO editors (areaid, userid) 
                                        VALUES (@areaid, @userid)";

                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);

                cmd.Parameters["areaid"].Value = newAreaID;
                cmd.Parameters["userid"].Value = userid;

                cmd.ExecuteNonQuery();
            }

            //diffs
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM diffs WHERE areaid = @areaid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = fork.areaid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int x = reader.GetInt32(reader.GetOrdinal("x"));
                        int y = reader.GetInt32(reader.GetOrdinal("y"));
                        int z = reader.GetInt32(reader.GetOrdinal("z"));
                        int blocktype = reader.GetInt32(reader.GetOrdinal("blocktype"));
                        int xchunknum = reader.GetInt32(reader.GetOrdinal("xchunknum"));
                        int zchunknum = reader.GetInt32(reader.GetOrdinal("zchunknum"));

                        using (SQLiteCommand insertcmd = con.CreateCommand())
                        {
                            insertcmd.CommandText = @"INSERT INTO diffs (areaid, x, y, z, blocktype, xchunknum, zchunknum) 
                                        VALUES (@areaid, @x, @y, @z, @blocktype, @xchunknum, @zchunknum)";

                            insertcmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                            insertcmd.Parameters.Add("x", System.Data.DbType.Int32);
                            insertcmd.Parameters.Add("y", System.Data.DbType.Int32);
                            insertcmd.Parameters.Add("z", System.Data.DbType.Int32);
                            insertcmd.Parameters.Add("blocktype", System.Data.DbType.Int32);
                            insertcmd.Parameters.Add("xchunknum", System.Data.DbType.Int32);
                            insertcmd.Parameters.Add("zchunknum", System.Data.DbType.Int32);

                            insertcmd.Parameters["areaid"].Value = newAreaID;
                            insertcmd.Parameters["x"].Value = x;
                            insertcmd.Parameters["y"].Value = y;
                            insertcmd.Parameters["z"].Value = z;
                            insertcmd.Parameters["blocktype"].Value = blocktype;
                            insertcmd.Parameters["xchunknum"].Value = xchunknum;
                            insertcmd.Parameters["zchunknum"].Value = zchunknum;

                            insertcmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return GetAreaInfo(newAreaID, userid);
        }

        public static void Select(SelectInfo select, int userid)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "UPDATE selects SET areaid = @areaid WHERE userid = @userid AND selectindex = @selectindex";

                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters.Add("selectindex", System.Data.DbType.Int32);

                cmd.Parameters["areaid"].Value = select.areaid;
                cmd.Parameters["userid"].Value = userid;
                cmd.Parameters["selectindex"].Value = select.selectindex;

                cmd.ExecuteNonQuery();
            }
        }

        public static AreaInfo AddEditor(EditorInfo editor, int userid)
        {
            int addUserID;
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE username = @username";
                cmd.Parameters.Add("username", System.Data.DbType.String);
                cmd.Parameters["username"].Value = editor.username;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }
                    else
                    {
                        reader.Read();

                        addUserID = reader.GetInt32(reader.GetOrdinal("userid"));
                    }
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM areas WHERE areaid = @areaid AND userid = @userid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = editor.areaid;
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM editors WHERE areaid = @areaid AND userid = @userid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = editor.areaid;
                cmd.Parameters["userid"].Value = addUserID;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return null;
                    }
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO editors (areaid, userid) 
                                        VALUES (@areaid, @userid)";

                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);

                cmd.Parameters["areaid"].Value = editor.areaid;
                cmd.Parameters["userid"].Value = addUserID;

                cmd.ExecuteNonQuery();
            }

            return GetAreaInfo(editor.areaid, userid);
        }

        public static AreaInfo RemoveEditor(EditorInfo editor, int userid)
        {
            int removeUserID;
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE username = @username";
                cmd.Parameters.Add("username", System.Data.DbType.String);
                cmd.Parameters["username"].Value = editor.username;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }
                    else
                    {
                        reader.Read();

                        removeUserID = reader.GetInt32(reader.GetOrdinal("userid"));
                    }
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM areas WHERE areaid = @areaid AND userid = @userid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = editor.areaid;
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"DELETE FROM editors WHERE areaid = @areaid AND userid = @userid";

                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);

                cmd.Parameters["areaid"].Value = editor.areaid;
                cmd.Parameters["userid"].Value = removeUserID;

                cmd.ExecuteNonQuery();
            }

            return GetAreaInfo(editor.areaid, userid);
        }

        public static bool SetBlock(SetBlockInfo block, int userid)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM editors WHERE areaid = @areaid AND userid = @userid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = block.areaid;
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return false;
                    }
                }
            }

            bool update;
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM diffs
                                    WHERE areaid = @areaid AND x = @x AND y = @y AND z = @z AND xchunknum = @xchunknum AND zchunknum = @zchunknum";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("x", System.Data.DbType.Int32);
                cmd.Parameters.Add("y", System.Data.DbType.Int32);
                cmd.Parameters.Add("z", System.Data.DbType.Int32);
                cmd.Parameters.Add("xchunknum", System.Data.DbType.Int32);
                cmd.Parameters.Add("zchunknum", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = block.areaid;
                cmd.Parameters["x"].Value = block.x;
                cmd.Parameters["y"].Value = block.y;
                cmd.Parameters["z"].Value = block.z;
                cmd.Parameters["xchunknum"].Value = block.xchunknum;
                cmd.Parameters["zchunknum"].Value = block.zchunknum;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        update = true;
                    }
                    else
                    {
                        update = false;
                    }
                }
            }

            if(update)
            {
                using (SQLiteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE diffs SET blocktype = @blocktype
                                        WHERE areaid = @areaid AND x = @x AND y = @y AND z = @z AND xchunknum = @xchunknum AND zchunknum = @zchunknum";

                    cmd.Parameters.Add("blocktype", System.Data.DbType.Int32);
                    cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                    cmd.Parameters.Add("x", System.Data.DbType.Int32);
                    cmd.Parameters.Add("y", System.Data.DbType.Int32);
                    cmd.Parameters.Add("z", System.Data.DbType.Int32);
                    cmd.Parameters.Add("xchunknum", System.Data.DbType.Int32);
                    cmd.Parameters.Add("zchunknum", System.Data.DbType.Int32);

                    cmd.Parameters["blocktype"].Value = block.blocktype;
                    cmd.Parameters["areaid"].Value = block.areaid;
                    cmd.Parameters["x"].Value = block.x;
                    cmd.Parameters["y"].Value = block.y;
                    cmd.Parameters["z"].Value = block.z;
                    cmd.Parameters["xchunknum"].Value = block.xchunknum;
                    cmd.Parameters["zchunknum"].Value = block.zchunknum;

                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (SQLiteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO diffs (areaid, x, y, z, blocktype, xchunknum, zchunknum) 
                                        VALUES (@areaid, @x, @y, @z, @blocktype, @xchunknum, @zchunknum)";
                    
                    cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                    cmd.Parameters.Add("x", System.Data.DbType.Int32);
                    cmd.Parameters.Add("y", System.Data.DbType.Int32);
                    cmd.Parameters.Add("z", System.Data.DbType.Int32);
                    cmd.Parameters.Add("blocktype", System.Data.DbType.Int32);
                    cmd.Parameters.Add("xchunknum", System.Data.DbType.Int32);
                    cmd.Parameters.Add("zchunknum", System.Data.DbType.Int32);
                    
                    cmd.Parameters["areaid"].Value = block.areaid;
                    cmd.Parameters["x"].Value = block.x;
                    cmd.Parameters["y"].Value = block.y;
                    cmd.Parameters["z"].Value = block.z;
                    cmd.Parameters["blocktype"].Value = block.blocktype;
                    cmd.Parameters["xchunknum"].Value = block.xchunknum;
                    cmd.Parameters["zchunknum"].Value = block.zchunknum;

                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }

        public static bool ResetBlock(SetBlockInfo block, int userid)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM editors WHERE areaid = @areaid AND userid = @userid";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = block.areaid;
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return false;
                    }
                }
            }
            

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM diffs
                                    WHERE areaid = @areaid AND x = @x AND y = @y AND z = @z AND xchunknum = @xchunknum AND zchunknum = @zchunknum";
                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("x", System.Data.DbType.Int32);
                cmd.Parameters.Add("y", System.Data.DbType.Int32);
                cmd.Parameters.Add("z", System.Data.DbType.Int32);
                cmd.Parameters.Add("xchunknum", System.Data.DbType.Int32);
                cmd.Parameters.Add("zchunknum", System.Data.DbType.Int32);
                cmd.Parameters["areaid"].Value = block.areaid;
                cmd.Parameters["x"].Value = block.x;
                cmd.Parameters["y"].Value = block.y;
                cmd.Parameters["z"].Value = block.z;
                cmd.Parameters["xchunknum"].Value = block.xchunknum;
                cmd.Parameters["zchunknum"].Value = block.zchunknum;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return false;
                    }
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"DELETE FROM diffs
                                    WHERE areaid = @areaid AND x = @x AND y = @y AND z = @z AND xchunknum = @xchunknum AND zchunknum = @zchunknum";

                cmd.Parameters.Add("areaid", System.Data.DbType.Int32);
                cmd.Parameters.Add("x", System.Data.DbType.Int32);
                cmd.Parameters.Add("y", System.Data.DbType.Int32);
                cmd.Parameters.Add("z", System.Data.DbType.Int32);
                cmd.Parameters.Add("xchunknum", System.Data.DbType.Int32);
                cmd.Parameters.Add("zchunknum", System.Data.DbType.Int32);

                cmd.Parameters["areaid"].Value = block.areaid;
                cmd.Parameters["x"].Value = block.x;
                cmd.Parameters["y"].Value = block.y;
                cmd.Parameters["z"].Value = block.z;
                cmd.Parameters["xchunknum"].Value = block.xchunknum;
                cmd.Parameters["zchunknum"].Value = block.zchunknum;

                cmd.ExecuteNonQuery();

                return true;
            }
        }

        /*public static PlayerInitData GetPlayer(string name)
        {
            int userid;

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE name = @name";
                cmd.Parameters.Add("name", System.Data.DbType.String);
                cmd.Parameters["name"].Value = name;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    
                    userid = reader.GetInt32(reader.GetOrdinal("userid"));
                }
            }

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM players WHERE userid = @userid";
                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters["userid"].Value = userid;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    PlayerSyncData sync = new PlayerSyncData();
                    sync.userid = userid;
                    sync.xpos = reader.GetFloat(reader.GetOrdinal("xpos"));
                    sync.ypos = reader.GetFloat(reader.GetOrdinal("ypos"));
                    sync.zpos = reader.GetFloat(reader.GetOrdinal("zpos"));
                    sync.yrot = reader.GetFloat(reader.GetOrdinal("yrot"));
                    sync.animestate = reader.GetInt32(reader.GetOrdinal("animestate"));

                    PlayerInitData init = new PlayerInitData();
                    init.username = name;
                    init.sync = sync;

                    return init;
                }
            }
        }*/

        public static void SavePlayerSyncData(PlayerSyncData sync)
        {
            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"UPDATE players SET xpos = @xpos, ypos = @ypos, zpos = @zpos, xrot = @xrot, yrot = @yrot, animestate = @animestate WHERE userid = @userid";

                cmd.Parameters.Add("userid", System.Data.DbType.Int32);
                cmd.Parameters.Add("xpos", System.Data.DbType.Single);
                cmd.Parameters.Add("ypos", System.Data.DbType.Single);
                cmd.Parameters.Add("zpos", System.Data.DbType.Single);
                cmd.Parameters.Add("xrot", System.Data.DbType.Single);
                cmd.Parameters.Add("yrot", System.Data.DbType.Single);
                cmd.Parameters.Add("animestate", System.Data.DbType.Int32);

                cmd.Parameters["userid"].Value = sync.userid;
                cmd.Parameters["xpos"].Value = sync.xpos;
                cmd.Parameters["ypos"].Value = sync.ypos;
                cmd.Parameters["zpos"].Value = sync.zpos;
                cmd.Parameters["xrot"].Value = sync.xrot;
                cmd.Parameters["yrot"].Value = sync.yrot;
                cmd.Parameters["animestate"].Value = sync.animestate;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
