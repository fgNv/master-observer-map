module Routes

open Owin
open Microsoft.Owin
open Microsoft.Owin.Builder
    
open Suave
open Suave.Owin
open Suave.Http
open Suave.Operators
open Suave.Filters
    
open System
open Hubs
open Microsoft.AspNet.SignalR
open Domain
open Suave.Successful
open System.Net.Security
open System.Net
open System.Security.Cryptography.X509Certificates

let sendCoordinates coordinates =     
    let hubContext = GlobalHost.ConnectionManager.GetHubContext<MapHub, IClientMapHub> ()
    hubContext.Clients.All.ReceiveCoordinates(coordinates)  

let buildOwin() =
    let builder = new AppBuilder()
    let builder = builder.MapSignalR()
    builder.Build() |> OwinApp.ofAppFunc ""
    
let owin = buildOwin()

SignalRConfiguration.configure()

let delegation (o : Object) 
               (certificate : X509Certificate) 
               (chain : X509Chain) 
               (errors : SslPolicyErrors) = true

let certificateValidationCallback = RemoteCertificateValidationCallback(delegation)

ServicePointManager.ServerCertificateValidationCallback <- certificateValidationCallback

let app =     
    choose [
        GET >=> path "/" >=>             
            Files.browseFileHome "index.html"
        GET >=> pathScan "/coordinates/send/%f/%f" (fun(latitude, longitude) ->
            sendCoordinates({Latitude = latitude; Longitude = longitude})
            OK "ok"
        )
        GET >=> Files.browseHome
        owin
    ]