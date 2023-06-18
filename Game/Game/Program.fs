module Program

open System
open System.Collections.Generic
open LiteNetLib.Utils
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core

type Direction =
    | Up
    | Down
    | Left
    | Right

type Move ={
    Direction: Direction
}

type Position = { x: int; y: int }

type Team =
    | Red
    | Blue

type Player =
    {
        Id: int
        Position: Position
        Team: Team
    }

type MapPixel =
    | Empty
    | Player of Player

type GameContext = {
    ExitGame: bool
    Map: MapPixel [][] // JSON convert doesnt support multi-dimensional arrays
    Players: Player list
}

// module Serialization =
//     let serialize (writer: NetDataWriter) (gameContext: GameContext) =
//         let netPacketProcessor = new NetPacketProcessor();
//         netPacketProcessor.RegisterNestedType<Player>()
//         writer.Put();
//         writer.PutArray(gameContext.Players);

module Renderer =
    let drawGameOver () = ()
    
    let printColor (team: Team) (pixelForm: string)=
        match team with
        | Red ->
            Console.ForegroundColor <- ConsoleColor.Red;
            printf "%s" pixelForm;
            Console.ForegroundColor <- ConsoleColor.White;
        | Blue ->
            Console.ForegroundColor <- ConsoleColor.Blue;
            printf "%s" pixelForm;
            Console.ForegroundColor <- ConsoleColor.White
        
    let renderMap gameContext =
        
        for x = 0 to Array.length gameContext.Map - 1 do
            for y = 0 to Array.length gameContext.Map[x] - 1 do
                let mapPixel = gameContext.Map[x][y]
                let playerCoordinates = gameContext.Players |> List.map (fun p -> (p.Position.x, p.Position.y))
                match mapPixel with
                | MapPixel.Empty when playerCoordinates |> List.contains (x, y) ->
                    let color = (gameContext.Players |> List.find (fun p -> p.Position = { x = x; y = y })).Team
                    printColor color "□ "
                | MapPixel.Empty ->
                        printf ". "
            printf "\n"

let init() =
    // let map1 = Array2D.init 10 16 (fun _ _ -> MapPixel.Empty)
    let map = Array.init 16 (fun _ -> Array.init 10 (fun _ -> MapPixel.Empty))
    {
        ExitGame = false
        Map = map
        Players = []
    }

let gameLoop gameContext =
    
    if gameContext.ExitGame then
        Renderer.drawGameOver ()
        Environment.Exit 0
        
    Console.Clear()
    Renderer.renderMap gameContext
    Threading.Thread.Sleep(150)

let main () = 1;