namespace MoonFlower.ViewModel

open ViewModule
open ViewModule.FSharp
open MoonFlower.ViewModel

type MainViewModel() as self =
    inherit ViewModelBase()

    let inputText = self.Factory.Backing(<@ self.InputText @>, "")

    member this.OAuth = OAuthViewModel()
    member this.Panes = [| PaneViewModel() |]
    member this.InputText
        with get() = inputText.Value
        and set(v) = inputText.Value <- v
