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
               (errors : SslPolicyErrors) = 
    try
        System.Console.WriteLine("verifying certificate")
        match errors with
            | SslPolicyErrors.None -> 
                System.Console.WriteLine("no error")
                true
            | _ -> System.Console.WriteLine("verifying errors")
                   chain.ChainStatus |> 
                   Seq.fold (fun acc (c : X509ChainStatus) -> 
                        if not <| acc then 
                            false 
                        else
                            chain.ChainPolicy.RevocationFlag <- X509RevocationFlag.EntireChain
                            chain.ChainPolicy.RevocationMode <- X509RevocationMode.Online
                            chain.ChainPolicy.UrlRetrievalTimeout <- new TimeSpan (0, 1, 0)
                            chain.ChainPolicy.VerificationFlags <- X509VerificationFlags.AllFlags
                            chain.Build (certificate :?> X509Certificate2) ) true 
    with
        | error ->
            System.Console.WriteLine("expcetion occurred -> " + error.Message)                                    
            false

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