namespace MoonFlower.ViewModel

open System
open System.Diagnostics
open System.IO
open System.Configuration
open System.Runtime.Serialization.Formatters.Binary
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open MoonFlower.Model

type AppConfigurationSection() =
    inherit ConfigurationSection()

    [<ConfigurationProperty("data", IsRequired = false, IsKey = false, DefaultValue = """{"registrations":[],"accounts":[]}""")>]
    member this.Data
        with get() = this.["data"] :?> String
        and set(v: String) =
            this.["data"] <- v

    member this.AppModel
        with get() =
            match this.Data with
            | null -> AppModel()
            | str ->
                try
                    JsonConvert.DeserializeObject<AppModel>(str)
                with | e ->
                    Debug.WriteLine str
                    Debug.WriteLine e
                    AppModel()
        and set(v: AppModel) = this.Data <- JsonConvert.SerializeObject(v)