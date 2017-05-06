namespace MoonFlower.Model

open Mastonet
open Mastonet.Entities

open Newtonsoft.Json

open System
open System.Diagnostics
open System.Net

type AppModel() =
    let appName = "MoonFlower"

    do
        ServicePointManager.SecurityProtocol <- SecurityProtocolType.Tls12

    [<JsonProperty("registrations")>]
    member val Registrations: AppRegistration list = [] with get, set
    [<JsonProperty("accounts")>]
    member val Accounts: YourAccount list = [] with get, set

    member this.RegisterApp(host: string): Async<AuthenticationClient> =
        match this.Registrations |> List.tryFind (fun r -> r.Instance = host) with
        | Some regist -> async { return AuthenticationClient(regist) }
        | None -> async {
            let client = AuthenticationClient(host)
            let! regist = client.CreateApp(appName, Scope.Read + Scope.Write + Scope.Follow)
                          |> Async.AwaitTask
            this.Registrations <- regist :: this.Registrations
            return client
        }

    member this.ConnectAccount(host: string, code: string) =
        match this.Registrations |> List.tryFind (fun r -> r.Instance = host) with
        | None -> async { return None }
        | Some regist -> async {
            let client = AuthenticationClient(regist)
            let! auth = client.ConnectWithCode(code) |> Async.AwaitTask
            let! account = this.AddAccount(regist, auth)
            return Some (auth, account)
        }

    member this.AddAccount(regist: AppRegistration, auth: Auth) =
        async {
            let client = MastodonClient(regist, auth)
            let! account = client.GetCurrentUser() |> Async.AwaitTask
            let yourAccount = { Auth = auth; App = regist; Detail = account; }
            this.Accounts <- yourAccount :: this.Accounts
            return yourAccount
        }