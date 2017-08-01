namespace MoonFlower.Model


open Mastonet.Entities

type TimelineModel = 
    | SimpleTimeline of YourAccount * TimelineAPI
    member this.Fetch(): (Status seq) Async =
        match this with
        | SimpleTimeline (account, api) -> api.Get account.Client

