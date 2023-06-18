using LiteNetLib;
using LiteNetLib.Utils;

namespace ClassLibrary1;

public static class WorkAround
{
    public static void SendToAllFromCs(NetManager server, NetDataWriter writer)
    {
        server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
    }
}