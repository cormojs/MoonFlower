namespace MoonFlower.Model

open Mastonet
open Mastonet.Entities

open System.Diagnostics
open System.Net

type AppModel() =
    let appName = "MoonFlower"

    do
        ServicePointManager.SecurityProtocol <- SecurityProtocolType.Tls12

    member val Registrations: Map<string, AppRegistration> = Map.empty with get, set
    member val Accounts: Map<string, YourAccount> = Map.empty with get, set

    member this.RegisterApp(host: string): Async<AuthenticationClient> =
        match this.Registrations |> Map.tryFind host with
        | Some regist -> async { return AuthenticationClient(regist) }
        | None -> async {
            let client = AuthenticationClient(host)
            let! regist = client.CreateApp(appName, Scope.Read + Scope.Write + Scope.Follow)
                          |> Async.AwaitTask
            this.Registrations <- this.Registrations.Add(host, regist)
            return client
        }

    member this.ConnectAccount(host: string, code: string) =
        match this.Registrations |> Map.tryFind host with
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
            this.Accounts <- this.Accounts.Add(auth.AccessToken, yourAccount)
            return yourAccount
        }