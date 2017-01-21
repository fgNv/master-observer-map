open Suave
    
[<EntryPoint>]
let main argv =    
    startWebServer defaultConfig Routes.app    
    0
