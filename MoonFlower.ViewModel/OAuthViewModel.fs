namespace MoonFlower.ViewModel

open System.Diagnostics

open ViewModule
open ViewModule.FSharp

open MoonFlower.Model

type OAuthViewModel(app: AppModel) as self =
    inherit ViewModelBase()

    let hostName = self.Factory.Backing(<@ self.HostName @>, "")
    let code = self.Factory.Backing(<@ self.Code @>, "")

    member this.HostName
        with get() = hostName.Value
        and set(v) = hostName.Value <- v

    member this.Code
        with get() = code.Value
        and set(v) = code.Value <- v

    member this.OpenAuthPage() = 
        Debug.WriteLine (sprintf "open auth page %s" this.HostName)
        async {
            try
                let! client = app.RegisterApp(this.HostName)
                let url = client.OAuthUrl()
                Process.Start(url) |> ignore
            with | e -> Debug.WriteLine e
        } |> Async.Start
