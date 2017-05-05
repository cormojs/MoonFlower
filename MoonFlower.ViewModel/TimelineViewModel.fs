namespace MoonFlower.ViewModel

open ViewModule
open ViewModule.FSharp

// open MoonFlower.Model

type Account = { Connection: string }

type TimelineViewModel(account: Account) as self =
    inherit ViewModelBase()

    let title = self.Factory.Backing(<@ self.Title @>, "untitled")

    member this.Title
        with get() = title.Value
        and set(v) = title.Value <- v
    member this.ConnectedAccount = account
    member this.Statuses = [| "toot1"; "toot2"; "toot3" |]
