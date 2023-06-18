open System
open ClassLibrary1
open FSharp.Json
open System.Threading
open LiteNetLib;
open LiteNetLib.Utils
open Program;

let listener = EventBasedNetListener();
let server = NetManager(listener)
server.Start(9050) |> ignore

let mutable state: GameContext =
    init() 

listener.add_ConnectionRequestEvent
    (EventBasedNetListener.OnConnectionRequest (fun request ->
        if server.ConnectedPeersCount < 10 then
            request.AcceptIfKey("SomeConnectionKey") |> ignore
        else
            request.Reject()
        // printf "%O" server.
    ))

listener.add_PeerConnectedEvent
    (EventBasedNetListener.OnPeerConnected (fun peer ->
        printf "We got connection: %O\n" peer.EndPoint
        
        state <- {state with Players =
            state.Players @ [{Id = peer.Id; Position = {x = 1; y = 1}; Team = if peer.Id % 2 = 1 then Red else Blue}] }
        server.SendToAll(NetDataWriter.FromString(Json.serialize state), DeliveryMethod.ReliableOrdered)
    ))

listener.add_NetworkReceiveEvent
    (EventBasedNetListener.OnNetworkReceive (fun peer reader byte method ->
        let direction = Json.deserialize<Move> (reader.GetString())
        printf "Got Message: %d\n" peer.Id
        reader.Recycle()
        
        match direction with
        | {Direction = Up} ->
            state <- {state with Players = state.Players
                                           |> List.map (fun p ->
                                               if p.Id = peer.Id then
                                                   {p with Position = {p.Position with x = p.Position.x - 1} }
                                               else
                                                   p
                                               ) } 
        | {Direction = Down} ->
            state <- {state with Players = state.Players
                                           |> List.map (fun p ->
                                               if p.Id = peer.Id then
                                                   {p with Position = {p.Position with x = p.Position.x + 1} }
                                               else
                                                   p
                                               ) } 
        | {Direction = Left} ->
            state <- {state with Players = state.Players
                                           |> List.map (fun p ->
                                               if p.Id = peer.Id then
                                                   {p with Position = {p.Position with y = p.Position.y - 1} }
                                               else
                                                   p
                                               ) } 
        | {Direction = Right} ->
            state <- {state with Players = state.Players
                                           |> List.map (fun p ->
                                               if p.Id = peer.Id then
                                                   {p with Position = {p.Position with y = p.Position.y + 1} }
                                               else
                                                   p
                                               ) } 
        WorkAround.SendToAllFromCs(server, NetDataWriter.FromString(Json.serialize state))
    ))

let rec serverLoop () =
    if Console.KeyAvailable then
        server.Stop()
        
    server.PollEvents()
    Thread.Sleep(15)
    serverLoop ()

serverLoop ()