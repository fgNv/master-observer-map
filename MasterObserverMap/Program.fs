open Suave
open System.IO

let solutionFolder = Directory.GetParent(__SOURCE_DIRECTORY__).ToString()
let myHomeFolder = Path.Combine(solutionFolder, "MasterObserverMapPresentation")

let config = 
    { defaultConfig with 
        homeFolder = Some(myHomeFolder) }
    
[<EntryPoint>]
let main argv =    
    startWebServer config Routes.app    
    0
