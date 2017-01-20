module SignalRConfiguration

open Newtonsoft.Json.Serialization
open Microsoft.AspNet.SignalR.Infrastructure
open Newtonsoft.Json
open Microsoft.AspNet.SignalR

type SignalRContractResolver() =
    let defaultContractSerializer = new DefaultContractResolver() :> IContractResolver
    let camelCaseContractResolver = new CamelCasePropertyNamesContractResolver() :> IContractResolver
    let assembly = typedefof<Connection>.Assembly

    interface IContractResolver with
        member x.ResolveContract(``type``: System.Type): JsonContract =         
            (if ``type``.Assembly = assembly then
                defaultContractSerializer
             else
                camelCaseContractResolver).ResolveContract(``type``)        

let configure() =
    let settings = new JsonSerializerSettings()
    settings.ContractResolver <- new SignalRContractResolver()
    let serializer = JsonSerializer.Create(settings)
    GlobalHost.DependencyResolver.Register(typedefof<JsonSerializer>, fun () -> box(serializer));

