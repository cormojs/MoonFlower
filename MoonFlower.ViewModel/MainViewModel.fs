namespace MoonFlower.ViewModel

open ViewModule
open ViewModule.FSharp
open MoonFlower.Model
open MoonFlower.ViewModel

type MainViewModel() as self =
    inherit ViewModelBase()

    let inputText = self.Factory.Backing(<@ self.InputText @>, "")
    let app = AppModel()

    member this.OAuth = OAuthViewModel(app)
    member this.Panes = [| PaneViewModel() |]
    member this.InputText
        with get() = inputText.Value
        and set(v) = inputText.Value <- v
