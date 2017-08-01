namespace MoonFlower.ViewModel


open ViewModule
open ViewModule.FSharp

open MoonFlower.ViewModel

type PaneViewModel() as self =
    inherit ViewModelBase()

    let tls = self.Factory.Backing(<@ self.Timelines @>, Seq.empty)

    member this.Timelines with get() = tls.Value
                          and set(v) = tls.Value <- v

    member this.Add(timeline: TimelineViewModel): unit =
        this.Timelines <- Seq.singleton timeline
                          |> Seq.append this.Timelines