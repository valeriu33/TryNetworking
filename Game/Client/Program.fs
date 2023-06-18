open System
open System.Runtime.Serialization
open FSharp.Json
open System.Threading
open LiteNetLib
open LiteNetLib.Utils
open Program

let listener = EventBasedNetListener();
let client = NetManager(listener);
client.Start() |> ignore

client.Connect("localhost" , 9050 , "SomeConnectionKey" ) |> ignore

listener.add_NetworkReceiveEvent
    (EventBasedNetListener.OnNetworkReceive (fun fromPeer reader deliveryMethod channel ->
        let newState = Json.deserialize<GameContext> (reader.GetString())
        Console.Clear()
        Renderer.renderMap newState
        printf "%d" client.ConnectedPeersCount
        reader.Recycle()
    ))

let rec gameLoop ()=
    if Console.KeyAvailable then
        match Console.ReadKey().Key with
        | ConsoleKey.W ->
            client.SendToAll(NetDataWriter.FromString(Json.serialize {Direction = Up}), DeliveryMethod.ReliableOrdered)
        | ConsoleKey.S ->
            client.SendToAll(NetDataWriter.FromString(Json.serialize {Direction = Down}), DeliveryMethod.ReliableOrdered)
        | ConsoleKey.A ->
            client.SendToAll(NetDataWriter.FromString(Json.serialize {Direction = Left}), DeliveryMethod.ReliableOrdered)
        | ConsoleKey.D ->
            client.SendToAll(NetDataWriter.FromString(Json.serialize {Direction = Right}), DeliveryMethod.ReliableOrdered)
        | ConsoleKey.Q ->
            client.Stop()
    client.PollEvents();
    Thread.Sleep(15)
    gameLoop()
    
gameLoop ()


