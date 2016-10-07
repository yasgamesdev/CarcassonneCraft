using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public enum MessageType
    {
        Connect,
        Disconnect,
        Debug,
        Login,
        LoginSuccess,
        LoginFailed,
        Register,
        RegisterSuccess,
        RegisterFailed,
        Join,
        Snapshot,
        Push,
        SendMessage,
        BroadcastMessage,
    }
}
