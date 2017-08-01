namespace MoonFlower.ViewModel

open System.Configuration
open System.Diagnostics
open Newtonsoft.Json
open ViewModule
open ViewModule.FSharp
open ViewModule.Validation.FSharp
open Livet.Messaging
open MoonFlower.Model


type OAuthViewModel(parent: MainViewModelBase, messenger: InteractionMessenger) as self =
    inherit ViewModelBase()

    let hostName = self.Factory.Backing(<@ self.HostName @>, "", notNullOrWhitespace)
    let code = self.Factory.Backing(<@ self.Code @>, "", notNullOrWhitespace)
    let appSettings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings

    member this.HostName
        with get() = hostName.Value
        and set(v) = hostName.Value <- v

    member this.Code
        with get() = code.Value
        and set(v) = code.Value <- v

    member this.OpenAuthPage() = 
        async {
            try
                let! client = parent.App.RegisterApp(this.HostName)
                let url = client.OAuthUrl()
                Process.Start(url) |> ignore
            with | e -> Debug.WriteLine e
        } |> Async.Start

    member this.AddAccount() =
        async {
            let! result = parent.App.ConnectAccount(this.HostName, this.Code)
            sprintf "add %s, %s" this.HostName this.Code |> Debug.WriteLine
            match result with
            | None ->
                JsonConvert.SerializeObject parent.App
                |> sprintf "no registration found: %s"
                |> Debug.WriteLine
            | Some (auth, account) ->
                Debug.WriteLine <| sprintf "added account: %s" (account.ToString())
                Debug.WriteLine <| JsonConvert.SerializeObject parent.App
                parent.App.CurrentUser <- account.FullName
                messenger.RaiseAsync(InteractionMessage("AccountUpdate"))
                |> Async.AwaitTask
                |> ignore
        } |> Async.Start


    member this.GetAccounts() = parent.App.Accounts
