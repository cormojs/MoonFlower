namespace MoonFlower.ViewModel

open ViewModule
open ViewModule.FSharp

open MoonFlower.ViewModel

type PaneViewModel() =
    inherit ViewModelBase()

    member this.Timelines = [| TimelineViewModel({ Connection = "test" }) |]
