namespace MoonFlower.Model

open Mastonet
open Mastonet.Entities

open Newtonsoft.Json

open System
open System.Diagnostics
open System.Net
open System.Runtime.Serialization

[<JsonDictionary>]
type Registrations =
    (string * AppRegistration) list

type AppModel() =
    let appName = "MoonFlower"
    let mutable regists = Map.empty

    do
        ServicePointManager.SecurityProtocol <- SecurityProtocolType.Tls12

    [<JsonProperty("registrations")>]
    member val Registrations: Registrations = [] with get, set
    [<JsonProperty("accounts")>]
    member val Accounts: YourAccount list = [] with get, set
    [<JsonProperty("currentUser")>]
    member val CurrentUser: string = null with get, set

    member this.RegisterApp(host: string): Async<AuthenticationClient> =
        match this.Registrations |> List.tryFind (fun (i, _) -> i = host) with
        | Some (_, regist) ->
            async {
                regist.Instance <- host
                return AuthenticationClient(regist)
            }
        | None -> async {
            let client = AuthenticationClient(host)
            let! regist = client.CreateApp(appName, Scope.Read + Scope.Write + Scope.Follow)
                          |> Async.AwaitTask
            this.Registrations <- (host, regist) :: this.Registrations
            return client
        }

    member this.ConnectAccount(host: string, code: string) =
        match this.Registrations |> List.tryFind (fun (i, _) -> i = host) with
        | None -> async { return None }
        | Some (_, regist) -> async {
            let client = AuthenticationClient(regist)
            let! auth = client.ConnectWithCode(code) |> Async.AwaitTask
            let! account = this.AddAccount(host, regist, auth)
            return Some (auth, account)
        }

    member this.AddAccount(host: string, regist: AppRegistration, auth: Auth) =
        async {
            let client = MastodonClient(regist, auth)
            let! account = client.GetCurrentUser() |> Async.AwaitTask
            let yourAccount = {
                Host = host;
                Auth = auth;
                App = regist;
                Detail = account;
            }
            this.Accounts <- yourAccount :: this.Accounts
            return yourAccount
        }
    member this.LoadAvatar(): Async<unit> =
        this.Accounts
        |> List.map (fun acc -> acc.SaveAvatar())
        |> Async.Parallel
        |> Async.Ignore
    member this.GetUser(username: string): YourAccount option =
        this.Accounts
        |> List.tryFind (fun a -> a.FullName = username)

    member this.Toot(text: string, ?username: string): Async<Status> =
        let username' = defaultArg username this.CurrentUser
        match this.GetUser(username') with
        | None -> raise (ArgumentException("cannot find account"))
        | Some yourAccount ->
            yourAccount.Client.PostStatus(text, Visibility.Public)
            |> Async.AwaitTask
