module Hubs

open Microsoft.AspNet.SignalR
open Domain
open System

type IClientMapHub =
    abstract ReceiveCoordinates : Coordinates -> unit
    abstract DefineReference : string -> unit

type MapHub() =
    inherit Hub<IClientMapHub>()

    member private this.CreateNewReference() = 
        let newReference = Guid.NewGuid().ToString()
        this.Groups.Add(this.Context.ConnectionId, newReference) 
                |> Async.AwaitTask 
                |> Async.RunSynchronously
        this.Clients.Caller.DefineReference newReference
    
    member this.SendCoordinates coordinates reference =
        this.Clients.Group(reference).ReceiveCoordinates(coordinates)

    override this.OnConnected() =
        let roleArg = Option.ofObj <| this.Context.QueryString.Get("role") 
        let referenceArg = Option.ofObj <| this.Context.QueryString.Get("reference") 

        match roleArg, referenceArg with
        | None, _  -> this.CreateNewReference()         
        | Some role, None when role = "undefined" -> this.CreateNewReference()
        | Some role, Some reference when role = "observer" -> 
            this.Groups.Add(this.Context.ConnectionId, reference) 
                |> Async.AwaitTask 
                |> Async.RunSynchronously
        | _, _ -> ()

        base.OnConnected()
