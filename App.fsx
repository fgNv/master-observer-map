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

open Suave
open System

let myHomeFolder = Path.Combine(__SOURCE_DIRECTORY__, "MasterObserverMap")
let cfg = { defaultConfig with homeFolder = Some(myHomeFolder) }
Console.WriteLine("before launch service")

startWebServer cfg Routes.app

