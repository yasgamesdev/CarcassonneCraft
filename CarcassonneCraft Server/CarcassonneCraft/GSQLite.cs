using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    public static class GSQLite
    {
        static SQLiteConnection con;

        public static void Open(string path)
        {
            con = new SQLiteConnection("Data Source=" + path);
            con.Open();

            CreateTable();
        }

        static void CreateTable()
        {
            using (SQLiteCommand command = con.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS users (userid INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, passwd BLOB NOT NULL);";
                command.ExecuteNonQuery();
            }

            using (SQLiteCommand command = con.CreateCommand())
            {
                command.CommandText = @"CREATE TABLE IF NOT EXISTS players (
                                            userid INTEGER PRIMARY KEY,
                                            xpos REAL NOT NULL,
                                            ypos REAL NOT NULL,
                                            zpos REAL NOT NULL,
                                            yrot REAL NOT NULL,
                                            animestate INTEGER NOT NULL);";
                command.ExecuteNonQuery();
            }
        }

        public static void SaveAll()
        {

        }

        public static void Close()
        {
            con.Close();
        }

        public static bool IsAlreadyExistUser(string name)
        {
            SQLiteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM users WHERE name = @name";
            cmd.Parameters.Add("name", System.Data.DbType.String);
            cmd.Parameters["name"].Value = name;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                return reader.HasRows;
            }
        }

        public static PlayerInitData LoginUser(string name, byte[] hash)
        {
            int userid;

            using (SQLiteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE name = @name AND passwd = @passwd";
                cmd.Parameters.Add("name", System.Data.DbType.String);
                cmd.Parameters.Add("passwd", System.Data.DbType.Binary);
                cmd.Parameters["name"].Value = name;
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
                    sync.xPos = reader.GetFloat(reader.GetOrdinal("xpos"));
                    sync.yPos = reader.GetFloat(reader.GetOrdinal("ypos"));
                    sync.zPos = reader.GetFloat(reader.GetOrdinal("zpos"));
                    sync.yRot = reader.GetFloat(reader.GetOrdinal("yrot"));
                    sync.animestate = reader.GetInt32(reader.GetOrdinal("animestate"));

                    PlayerInitData init = new PlayerInitData();
                    init.name = name;
                    init.sync = sync;

                    return init;
                }
            }
        }

        public static PlayerInitData GetPlayer(string name)
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
                    sync.xPos = reader.GetFloat(reader.GetOrdinal("xpos"));
                    sync.yPos = reader.GetFloat(reader.GetOrdinal("ypos"));
                    sync.zPos = reader.GetFloat(reader.GetOrdinal("zpos"));
                    sync.yRot = reader.GetFloat(reader.GetOrdinal("yrot"));
                    sync.animestate = reader.GetInt32(reader.GetOrdinal("animestate"));

                    PlayerInitData init = new PlayerInitData();
                    init.name = name;
                    init.sync = sync;

                    return init;
                }
            }
        }

        public static int CreateUser(string name, byte[] hash)
        {
            SQLiteCommand cmd = con.CreateCommand();

            cmd.CommandText = "INSERT INTO users (name, passwd) VALUES (@name, @passwd)";

            cmd.Parameters.Add("name", System.Data.DbType.String);
            cmd.Parameters.Add("passwd", System.Data.DbType.Binary);

            cmd.Parameters["name"].Value = name;
            cmd.Parameters["passwd"].Value = hash;
            cmd.ExecuteNonQuery();

            int userid = (int)con.LastInsertRowId;

            using (SQLiteCommand command = con.CreateCommand())
            {
                command.CommandText = "INSERT INTO players (userid, xpos, ypos, zpos, yrot, animestate) VALUES (@userid, @xpos, @ypos, @zpos, @yrot, @animestate)";

                command.Parameters.Add("userid", System.Data.DbType.Int32);
                command.Parameters.Add("xpos", System.Data.DbType.Single);
                command.Parameters.Add("ypos", System.Data.DbType.Single);
                command.Parameters.Add("zpos", System.Data.DbType.Single);
                command.Parameters.Add("yrot", System.Data.DbType.Single);
                command.Parameters.Add("animestate", System.Data.DbType.Int32);

                command.Parameters["userid"].Value = userid;
                command.Parameters["xpos"].Value = 0;
                command.Parameters["ypos"].Value = 0;
                command.Parameters["zpos"].Value = 0;
                command.Parameters["yrot"].Value = 180;
                command.Parameters["animestate"].Value = 0;

                command.ExecuteNonQuery();
            }

            return userid;
        }

        public static void SavePlayer(PlayerSyncData sync)
        {
            using (SQLiteCommand command = con.CreateCommand())
            {
                command.CommandText = "UPDATE players SET xpos = @xpos, ypos = @ypos, zpos = @zpos, yrot = @yrot, animestate = @animestate WHERE userid = @userid";

                command.Parameters.Add("userid", System.Data.DbType.Int32);
                command.Parameters.Add("xpos", System.Data.DbType.Single);
                command.Parameters.Add("ypos", System.Data.DbType.Single);
                command.Parameters.Add("zpos", System.Data.DbType.Single);
                command.Parameters.Add("yrot", System.Data.DbType.Single);
                command.Parameters.Add("animestate", System.Data.DbType.Int32);

                command.Parameters["userid"].Value = sync.userid;
                command.Parameters["xpos"].Value = sync.xPos;
                command.Parameters["ypos"].Value = sync.yPos;
                command.Parameters["zpos"].Value = sync.zPos;
                command.Parameters["yrot"].Value = sync.yRot;
                command.Parameters["animestate"].Value = sync.animestate;

                command.ExecuteNonQuery();
            }
        }
    }
}
