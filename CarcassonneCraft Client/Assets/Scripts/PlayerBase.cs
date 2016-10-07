using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public abstract class PlayerBase
    {
        public abstract void ReceiveLatestData(PlayerSyncData sync);
    }
}
