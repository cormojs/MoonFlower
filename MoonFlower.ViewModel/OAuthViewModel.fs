namespace MoonFlower.ViewModel

open System.Diagnostics

open ViewModule
open ViewModule.FSharp
open ViewModule.Validation.FSharp

open MoonFlower.Model

type OAuthViewModel(app: AppModel) as self =
    inherit ViewModelBase()

    let hostName = self.Factory.Backing(<@ self.HostName @>, "", notNullOrWhitespace)
    let code = self.Factory.Backing(<@ self.Code @>, "", notNullOrWhitespace)

    member this.HostName
        with get() = hostName.Value
        and set(v) = hostName.Value <- v

    member this.Code
        with get() = code.Value
        and set(v) = code.Value <- v

    member val CurrentUser: (MoonFlower.Model.YourAccount option) = None with get, set

    member this.OpenAuthPage() = 
        async {
            try
                let! client = app.RegisterApp(this.HostName)
                let url = client.OAuthUrl()
                Process.Start(url) |> ignore
            with | e -> Debug.WriteLine e
        } |> Async.Start

    member this.AddAccount() =
        async {
            let! result = app.ConnectAccount(this.HostName, this.Code)
            match result with
            | None -> Debug.WriteLine "authentication failed"
            | Some (auth, account) ->
                this.CurrentUser <- Some account
        }

    member this.GetAccounts() = app.Accounts
