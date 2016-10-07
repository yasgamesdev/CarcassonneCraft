using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    public static class Players
    {
        static PlayersCashe cashe = new PlayersCashe();
        static Dictionary<int, Player> players = new Dictionary<int, Player>();

        public static void Init()
        {
        }
    }
}
