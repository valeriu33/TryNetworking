using ClassLibrary1;
using LiteNetLib;
using LiteNetLib.Utils;

EventBasedNetListener listener = new EventBasedNetListener();
NetManager server = new NetManager(listener);
server.Start(9050 /* port */);

listener.ConnectionRequestEvent += request =>
{
    if(server.ConnectedPeersCount < 10 /* max connections */)
        request.AcceptIfKey("SomeConnectionKey");
    else
        request.Reject();
};

listener.PeerConnectedEvent += peer =>
{
    Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
    NetDataWriter writer = new NetDataWriter();                 // Create writer class
    writer.Put("Hello client!");                                // Put some string
    peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
};

listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
{
    
    
    Console.WriteLine("We got connection: {0}", peer.EndPoint);
    server.SendToAll(NetDataWriter.FromString("test"), DeliveryMethod.ReliableOrdered);
};

while (!Console.KeyAvailable)
{
    server.PollEvents();
    Thread.Sleep(15);
}
server.Stop();