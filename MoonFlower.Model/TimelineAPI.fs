namespace MoonFlower.Model

open Mastonet
open Mastonet.Entities
open System

type TimelineAPI =
    | HomeTimeline
    | Favorites
    | PublicTimeline of bool
    member this.Get(client: MastodonClient): (Status seq) Async =
        let task =
            match this with
            | HomeTimeline -> client.GetHomeTimeline()
            | Favorites -> client.GetFavourites()
            | PublicTimeline b -> client.GetPublicTimeline(Nullable(), Nullable(), Nullable(), b)
        Async.AwaitTask task
