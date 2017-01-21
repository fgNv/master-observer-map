open System.IO

//System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
//if not (File.Exists "paket.exe") then 
//    (System.IO.Path.GetTempFileName(), "https://github.com/fsprojects/Paket/releases/download/3.13.3/paket.exe") |> fun (tmp, url) -> 
//        use wc = new System.Net.WebClient()
//        wc.DownloadFile(url, tmp)
//        System.IO.File.Move(tmp, System.IO.Path.GetFileName url)
//
//#r "paket.exe"
//Paket.Dependencies.Install(File.ReadAllText "paket.dependencies")

#r "packages/Owin/lib/net40/Owin.dll"
#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/Microsoft.AspNet.SignalR.Core/lib/net45/Microsoft.AspNet.SignalR.Core.dll"
#r "packages/Microsoft.AspNet.SignalR.SystemWeb/lib/net45/Microsoft.AspNet.SignalR.SystemWeb.dll"
#r "packages/Microsoft.Owin.Host.SystemWeb/lib/net45/Microsoft.Owin.Host.SystemWeb.dll"
#r "packages/Microsoft.Owin.Security/lib/net45/Microsoft.Owin.Security.dll"
#r "packages/Microsoft.AspNet.SignalR.Core/lib/net45/Microsoft.AspNet.SignalR.Core.dll"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "packages/Microsoft.Owin/lib/net45/Microsoft.Owin.dll"
#r "System.IO"
#load "MasterObserverMap/Domain.fs"
#load "MasterObserverMap/Hubs.fs"
#load "MasterObserverMap/SignalRConfiguration.fs"
#load "MasterObserverMap/Routes.fs"

#r "System.dll"
#r "mscorlib"
#r "System.Core.dll"

open Suave
open System

open System
open System.Security.Cryptography.X509Certificates
open System.Net.Security
open System.Net

let tempContentFolder = "AssetsTemp"

if Directory.Exists(tempContentFolder) then
    Directory.Delete(tempContentFolder, true)

Directory.CreateDirectory(tempContentFolder)

File.Copy(Path.Combine("MasterObserverMap", "index.html"), Path.Combine(tempContentFolder, "index.html"))
let scripts = Directory.GetFiles(Path.Combine("MasterObserverMap", "Scripts"))
Directory.CreateDirectory(Path.Combine(tempContentFolder, "Scripts"))
scripts |> Seq.iter (fun s -> File.Copy(s, Path.Combine(tempContentFolder, "Scripts", Path.GetFileName s)))

let myHomeFolder = Path.Combine(__SOURCE_DIRECTORY__, tempContentFolder)
let cfg = { defaultConfig with homeFolder = Some(myHomeFolder) }

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

System.Console.WriteLine("before launch service")

startWebServer cfg Routes.app

File.Delete(Path.Combine(tempContentFolder, "index.html"))
Directory.Delete(Path.Combine(tempContentFolder, "Scripts"))
