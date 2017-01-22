module SignalRConfiguration

open Newtonsoft.Json.Serialization
open Microsoft.AspNet.SignalR.Infrastructure
open Newtonsoft.Json
open Microsoft.AspNet.SignalR
open System.Collections.Generic
open System
open Microsoft.AspNet.SignalR.Hubs
open Hubs

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
                                
let firstLetterLowerCase (input : string) =
    Char.ToLowerInvariant(input.[0]).ToString() + input.Substring(1)

type CustomHubDescriptorProvider() =

        let getHubName (hubType : Type) =
            let name = ReflectionHelper.GetAttributeValue<HubNameAttribute,string>
                                (hubType, fun at -> at.HubName) |> Option.ofObj
                                
            match name with | Some n -> (n, true) 
                            | None -> (hubType.Name |> firstLetterLowerCase, false)   
                            
        let buildHubDescriptor (hubType : Type) =
                let result = new HubDescriptor()
                let name = getHubName hubType
                                
                result.NameSpecified <- snd name
                result.Name <- fst name
                result.HubType <- hubType 
                result

        let hubs = [typedefof<MapHub>]

        let hubsDictionary =
            dict[getHubName typedefof<MapHub> |> fst |> String.toLowerInvariant,
                 buildHubDescriptor typedefof<MapHub>]

        interface IHubDescriptorProvider with
                member this.GetHubs() = 
                        let result = hubs |> Seq.map buildHubDescriptor
                        new List<HubDescriptor>(result) :> IList<HubDescriptor>                                                     

                member this.TryGetHub(name : string, descriptor : byref<HubDescriptor>) =
                        
                        if hubsDictionary.ContainsKey <| name.ToLower() then
                            descriptor <- hubsDictionary.[name.ToLower()]
                            true
                        else
                            false

let configure() =
    let settings = new JsonSerializerSettings()
    settings.ContractResolver <- new SignalRContractResolver()
    let serializer = JsonSerializer.Create(settings)
    GlobalHost.DependencyResolver
              .Register(typedefof<IHubDescriptorProvider>, 
                        fun() -> new CustomHubDescriptorProvider() |> box)
    GlobalHost.DependencyResolver
              .Register(typedefof<JsonSerializer>, fun () -> box(serializer));

