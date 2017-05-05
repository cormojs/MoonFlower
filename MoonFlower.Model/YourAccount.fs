namespace MoonFlower.Model

open Mastonet
open Mastonet.Entities

type YourAccount =
    {
        Auth: Auth; 
        App: AppRegistration;
        Detail: Account;
    }
    member this.Client = MastodonClient(this.App, this.Auth)
