namespace MoonFlower.ViewModel

open ViewModule
open ViewModule.FSharp

type OAuthViewModel() as self =
    inherit ViewModelBase()

    let hostName = self.Factory.Backing(<@ self.HostName @>, "")
    let code = self.Factory.Backing(<@ self.Code @>, "")

    member this.HostName
        with get() = hostName.Value
        and set(v) = hostName.Value <- v

    member this.Code
        with get() = code.Value
        and set(v) = code.Value <- v

    member this.OpenAuthPage = do
        System.Diagnostics.Process.Start("https://www.google.com")
        |> ignore

