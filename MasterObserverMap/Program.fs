open Owin
open Microsoft.Owin
open Microsoft.Owin.Builder
    
open Suave
open Suave.Owin
open Suave.Http
open Suave.Operators
open Suave.Filters
    
open System
open System.Threading
open Hubs
open Microsoft.AspNet.SignalR
open Domain
open Suave.Successful
    
let sendCoordinates coordinates =     
    let hubContext = GlobalHost.ConnectionManager.GetHubContext<MapHub, IClientMapHub> ()
    hubContext.Clients.All.ReceiveCoordinates(coordinates)              

[<EntryPoint>]
let main argv = 
    let builder = new AppBuilder()
    let builder = builder.MapSignalR()
    let owin = builder.Build() |> OwinApp.ofAppFunc ""

    let app =     
        choose [
            GET >=> path "/" >=> Files.browseFileHome "index.html"
            GET >=> pathScan "/coordinates/send/%f/%f" (fun(latitude, longitude) ->
                sendCoordinates({Latitude = latitude; Longitude = longitude})
                OK "ok"
            )
            GET >=> Files.browseHome
            owin
        ]

    SignalRConfiguration.configure()
    startWebServer defaultConfig app
    
    0 // return an integer exit code
