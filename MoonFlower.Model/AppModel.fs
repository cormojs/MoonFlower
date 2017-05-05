namespace MoonFlower.Model

open Mastonet
open Mastonet.Entities

open System.Diagnostics
open System.Net

type AppModel() =
    let appName = "MoonFlower"

    do
        ServicePointManager.SecurityProtocol <- SecurityProtocolType.Tls12

    member val Registrations: Map<string, AppRegistration>= Map.empty with get, set
    member this.RegisterApp(host: string): Async<AuthenticationClient> =
        match this.Registrations |> Map.tryFind host with
        | Some regist -> async { return AuthenticationClient(regist) }
        | None -> async {
            let url = sprintf "https://%s" host
            Debug.WriteLine url
            let client = AuthenticationClient(host)
            let! regist = client.CreateApp(appName, Scope.Read + Scope.Write + Scope.Follow)
                          |> Async.AwaitTask
            this.Registrations <- this.Registrations.Add(host, regist)
            return client
        }
