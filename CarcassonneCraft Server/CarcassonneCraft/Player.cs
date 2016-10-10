using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    public class Player
    {
        public NetConnection connection { get; private set; }
        public PlayerInitData init { get; private set; }

        public Player(NetConnection connection, PlayerInitData init)
        {
            this.connection = connection;
            this.init = init;
        }

        public void Push(PushData data)
        {
            init.sync.xpos = data.xpos;
            init.sync.ypos = data.ypos;
            init.sync.zpos = data.zpos;
            init.sync.xrot = data.xrot;
            init.sync.yrot = data.yrot;
            init.sync.animestate = data.animestate;
        }
    }
}
