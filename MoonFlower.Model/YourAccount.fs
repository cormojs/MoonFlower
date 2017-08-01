namespace MoonFlower.Model

open System.Diagnostics
open System.Net
open Microsoft.FSharp.Control.WebExtensions
open System.IO
open System.Windows.Media
open Mastonet
open Mastonet.Entities
open Newtonsoft.Json


type YourAccount(auth: Auth, app: AppRegistration, detail_: Account) as self =
    new() =
        let auth = Auth()
        let app = AppRegistration()
        let detail = Account(AccountName = "john", DisplayName = "ジョン",AvatarUrl = "https://d258c7v9fkrdf.cloudfront.net/accounts/avatars/000/000/352/original/f628c7ee1dbb4b02.jpg")
        new YourAccount(auth, app, detail)
    [<JsonIgnore>]
    member this.Client =
        this.App.Instance <- this.Host
        MastodonClient(app, this.Auth)

    member val Auth: Auth = auth with get, set
    member val Host: string = "foo.com" with get, set
    member val App: AppRegistration = app with get, set
    member val Detail: Account = detail_ with get, set
    [<JsonIgnore>]
    member this.FullName = this.Detail.UserName + "@" + this.Host
    member this.SaveAvatar(): Async<unit> =
        async {
            let client = new WebClient()
            client.CachePolicy <- Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.CacheIfAvailable)
            let! binary = client.DownloadDataTaskAsync(this.Detail.AvatarUrl)
                          |> Async.AwaitTask
            Debug.WriteLine <| sprintf "cached: %s" this.Detail.AvatarUrl
            return (binary |> ignore)
        }
    [<JsonIgnore>]
    member this.AvatarImage: ImageSource option =
        let client = new WebClient()
        client.CachePolicy <- Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.CacheOnly)
        try
            let binary = client.DownloadData(this.Detail.AvatarUrl)
            use stream = new MemoryStream(binary)
            let image = Imaging.BitmapImage()
            image.BeginInit()
            image.CacheOption <- Imaging.BitmapCacheOption.OnLoad
            image.StreamSource <- stream
            image.EndInit()
            image.Freeze()
            Some (image :> ImageSource)
        with | :? WebException as e ->
            Debug.WriteLine <| sprintf "cache miss: %s" this.Detail.AvatarUrl
            None
    [<JsonIgnore>]
    member this.AvatarImageLoaded: bool = Option.isSome this.AvatarImage
    [<JsonIgnore>]
    member this.AvatarImageObj: obj =
        this.AvatarImage
        |> Option.map (fun i -> i :> obj)
        |> Option.toObj
