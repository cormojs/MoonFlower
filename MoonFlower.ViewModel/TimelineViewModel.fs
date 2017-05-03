namespace MoonFlower.ViewModel

open ViewModule
open ViewModule.FSharp

type TimelineViewModel() as self =
    inherit ViewModelBase()

    let title = self.Factory.Backing(<@ self.Title @>, "untitled")

    member this.Title
        with get() = title.Value
        and set(v) = title.Value <- v

    member this.Statuses = [| "toot1"; "toot2"; "toot3" |]
