namespace MoonFlower.Model

open Mastonet
open Mastonet.Entities

type YourAccount =
    {
        Host: string;
        Auth: Auth; 
        App: AppRegistration;
        Detail: Account;
    }
    member this.Client =
        let app = this.App
        app.Instance <- this.Host
        MastodonClient(app, this.Auth)
    member this.FullName = this.Detail.UserName + "@" + this.Host
