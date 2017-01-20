module Hubs

open Microsoft.AspNet.SignalR
open Domain

type IClientMapHub =
    abstract ReceiveCoordinates : Coordinates -> unit

type MapHub() =
    inherit Hub<IClientMapHub>()

    member this.SendCoordinates coordinates =
        this.Clients.Others.ReceiveCoordinates(coordinates)
