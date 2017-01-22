#I "packages/Owin/lib/net40"
#r "packages/Owin/lib/net40/Owin.dll"

#I "packages/Suave/lib/net40"
#r "packages/Suave/lib/net40/Suave.dll"

#I "packages/Microsoft.AspNet.SignalR.Core/lib/net45"
#r "packages/Microsoft.AspNet.SignalR.Core/lib/net45/Microsoft.AspNet.SignalR.Core.dll"

#I "packages/Microsoft.AspNet.SignalR.SystemWeb/lib/net45"
#r "packages/Microsoft.AspNet.SignalR.SystemWeb/lib/net45/Microsoft.AspNet.SignalR.SystemWeb.dll"

#I "packages/Microsoft.Owin.Host.SystemWeb/lib/net45"
#r "packages/Microsoft.Owin.Host.SystemWeb/lib/net45/Microsoft.Owin.Host.SystemWeb.dll"

#I "packages/Microsoft.Owin.Security/lib/net45"
#r "packages/Microsoft.Owin.Security/lib/net45/Microsoft.Owin.Security.dll"

#I "packages/Microsoft.AspNet.SignalR.Core/lib/net45"
#r "packages/Microsoft.AspNet.SignalR.Core/lib/net45/Microsoft.AspNet.SignalR.Core.dll"

#I "packages/Newtonsoft.Json/lib/net45"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

#I "packages/Microsoft.Owin/lib/net45"
#r "packages/Microsoft.Owin/lib/net45/Microsoft.Owin.dll"

#load "MasterObserverMap/Domain.fs"
#load "MasterObserverMap/Hubs.fs"
#load "MasterObserverMap/SignalRConfiguration.fs"
#load "MasterObserverMap/Routes.fs"

open Suave
open System
open System.Net
open System.IO

let frontEndDirectory = Path.Combine(__SOURCE_DIRECTORY__, "MasterObserverMapPresentation")

let config = 
    let port = System.Environment.GetEnvironmentVariable("PORT")
    let ip127  = IPAddress.Parse("127.0.0.1")
    let ipZero = IPAddress.Parse("0.0.0.0")

    { defaultConfig with 
        bindings=[ (if port = null then HttpBinding.create HTTP ip127 (uint16 8080)
                    else HttpBinding.create HTTP ipZero (uint16 port)) ]
        homeFolder = Some(frontEndDirectory) }
        
startWebServer config Routes.app

