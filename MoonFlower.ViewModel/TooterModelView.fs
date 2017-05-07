namespace MoonFlower.ViewModel

open System.Diagnostics
open System.Collections.ObjectModel
open ViewModule
open ViewModule.FSharp
open ViewModule.Validation.FSharp
open MoonFlower.Model
open Livet.EventListeners
open Livet.Messaging

type TooterViewModel(app: AppModel, messenger: InteractionMessenger) as self =
    inherit ViewModelBase()

    let tootText = self.Factory.Backing(<@ self.TootTextInput @>, "", notNullOrWhitespace)

    member this.TootTextInput
        with get() = tootText.Value
        and set(v) = 
            Debug.WriteLine v
            tootText.Value <- v

    member val SelectedAccount: YourAccount option = None with get, set
    member this.Toot() =
        match this.SelectedAccount with
        | Some account ->
            async {
                app.CurrentUser <- account.FullName
                Debug.WriteLine this.TootTextInput
                try
                    let! status = app.Toot(this.TootTextInput)
                    Debug.WriteLine status
                    this.TootTextInput <- ""
                with | e -> Debug.WriteLine e
            } |> Async.Start
        | None -> Debug.WriteLine "no account selected"
    
    member val Accounts = app.Accounts with get, set
