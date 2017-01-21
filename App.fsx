open System.IO

#r "packages/Owin/lib/net40/Owin.dll"
#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/Microsoft.AspNet.SignalR.Core/lib/net45/Microsoft.AspNet.SignalR.Core.dll"
#r "packages/Microsoft.AspNet.SignalR.SystemWeb/lib/net45/Microsoft.AspNet.SignalR.SystemWeb.dll"
#r "packages/Microsoft.Owin.Host.SystemWeb/lib/net45/Microsoft.Owin.Host.SystemWeb.dll"
#r "packages/Microsoft.Owin.Security/lib/net45/Microsoft.Owin.Security.dll"
#r "packages/Microsoft.AspNet.SignalR.Core/lib/net45/Microsoft.AspNet.SignalR.Core.dll"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "packages/Microsoft.Owin/lib/net45/Microsoft.Owin.dll"
#r "packages/FAKE/tools/FakeLib.dll"
#r "System.IO"

#load "MasterObserverMap/Domain.fs"
#load "MasterObserverMap/Hubs.fs"
#load "MasterObserverMap/SignalRConfiguration.fs"
#load "MasterObserverMap/Routes.fs"

open Suave
open System
open System.Net
open Fake
open Fake.NpmHelper

let frontEndDirectory = Path.Combine(__SOURCE_DIRECTORY__, "MasterObserverMapPresentation")

Target "NpmInstall" (fun _ ->
  Npm (fun p ->
        { p with
            Command = Install Standard
            WorkingDirectory = frontEndDirectory
            NpmFilePath = Path.Combine(__SOURCE_DIRECTORY__, "packages", "Npm.js", "tools", "npm.cmd")
        })
 )
 
RunTargetOrDefault "NpmInstall"

let config = 
    let port = System.Environment.GetEnvironmentVariable("PORT")
    let ip127  = IPAddress.Parse("127.0.0.1")
    let ipZero = IPAddress.Parse("0.0.0.0")

    { defaultConfig with 
        bindings=[ (if port = null then HttpBinding.create HTTP ip127 (uint16 8080)
                    else HttpBinding.create HTTP ipZero (uint16 port)) ]
        homeFolder = Some(frontEndDirectory) }
        
startWebServer config Routes.app

