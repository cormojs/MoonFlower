namespace MoonFlower.ViewModel

open System.Diagnostics
open System.Collections.ObjectModel
open ViewModule
open ViewModule.FSharp
open ViewModule.Validation.FSharp
open MoonFlower.Model
open Livet.EventListeners
open Livet.Messaging

type TooterViewModel(parent: MainViewModelBase, messenger: InteractionMessenger) as self =
    inherit ViewModelBase()

    let tootText = self.Factory.Backing(<@ self.TootTextInput @>, "", notNullOrWhitespace)
    let selectedAccount = self.Factory.Backing(<@ self.SelectedAccount @>, None)
    let accounts = self.Factory.Backing(<@ self.Accounts @>, parent.App.Accounts)
    let selectedValue = self.Factory.Backing(<@ self.SelectedAccountFullName @>, "")

    member this.TootTextInput
        with get() = tootText.Value
        and set(v) = 
            tootText.Value <- v
            Debug.WriteLine v

    member this.SelectedAccount
        with get(): YourAccount option = selectedAccount.Value
        and set(v: YourAccount option) =
            selectedAccount.Value <- v

    member this.SelectedAccountFullName
        with get() = selectedValue.Value
        and set(v) =
            Debug.WriteLine v
            selectedValue.Value <- v

    member this.Toot text =
        match this.SelectedAccount with
        | Some account ->
            async {
                parent.App.CurrentUser <- account.FullName
                Debug.WriteLine text
                try
                    let! status = parent.App.Toot text
                    Debug.WriteLine status
                    this.TootTextInput <- ""
                with | e -> Debug.WriteLine e
            } |> Async.Start
        | None -> Debug.WriteLine "no account selected"

    member this.TootCommand =
        this.Factory.CommandSyncParamChecked(this.Toot, System.String.IsNullOrWhiteSpace >> not, [ <@ this.TootTextInput @> ])
    member this.Accounts with get() = accounts.Value
                         and set(v) = accounts.Value <- v