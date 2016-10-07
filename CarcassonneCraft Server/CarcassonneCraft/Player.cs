using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    class Player
    {
        public NetConnection connection { get; private set; }
        public PlayerInitData init { get; private set; }

        public Player(NetConnection connection, PlayerInitData init)
        {
            this.connection = connection;
            this.init = init;
        }

        /*public void Push(PushData data)
        {
            init.sync.xPos = data.xPos;
            init.sync.yPos = data.yPos;
            init.sync.zPos = data.zPos;
            init.sync.yRot = data.yRot;
            init.sync.animestate = data.animestate;
        }*/
    }
}
