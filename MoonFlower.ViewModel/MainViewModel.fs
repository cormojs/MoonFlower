namespace MoonFlower.ViewModel

open System.Configuration
open System.Diagnostics

open Newtonsoft.Json

open ViewModule
open ViewModule.FSharp
open Livet.Messaging
open MoonFlower.Model
open MoonFlower.ViewModel


type MainViewModel() as self =
    inherit MainViewModelBase()
    
    let config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
    let panes = self.Factory.Backing(<@ self.Panes @>, PaneViewModel() |> Seq.singleton)
    
    member this.Load() =
        let section = 
            match config.Sections.["AppConfigurationSection"] with
            | null -> 
                let section = AppConfigurationSection()
                section.AppModel <- AppModel()
                config.Sections.Add("AppConfigurationSection", section)
                section.SectionInformation.ForceSave <- true
                config.Save()
                section
            | _ -> config.Sections.["AppConfigurationSection"] :?> AppConfigurationSection
        this.App <- section.AppModel
        this.App.LoadAvatar()
        |> Async.RunSynchronously

        this.LoadTimeline()
        this.Reload()

    member this.Save() =
        let section = config.Sections.["AppConfigurationSection"] :?> AppConfigurationSection
        section.AppModel <- this.App
        JsonConvert.SerializeObject this.App |> Debug.WriteLine
        config.Save()
        sprintf "config saved to: %s" config.FilePath |> Debug.WriteLine
    member this.OAuth = OAuthViewModel(this, this.Messenger)
    member this.Tooter = TooterViewModel(this, this.Messenger)
    member this.Panes with get() = panes.Value
                      and set(v) = panes.Value <- v
    member this.LoadTimeline() =
        let account = this.App.GetUser this.App.CurrentUser
        match account with
        | Some acc ->
            Debug.WriteLine "adding timeline..."
            let timeline = TimelineViewModel(acc, HomeTimeline)
            timeline.Fetch()
            timeline.Title <- acc.FullName
            (Seq.item 0 this.Panes).Add timeline
            this.RaisePropertyChanged <@ this.Panes @>
            Debug.WriteLine "added"
        | None -> Debug.WriteLine (sprintf "No account to be loaded: %s" this.App.CurrentUser)
    member this.Reload() =
        this.RaisePropertyChanged(<@ this @>)
    member this.AccountAdded() =
        let account = this.App.GetUser this.App.CurrentUser
        this.Tooter.Accounts <- this.App.Accounts
        this.RaisePropertyChanged <@ this.Tooter @>
        this.Tooter.SelectedAccount <- account
        this.RaisePropertyChanged <@ this.Tooter @>
        this.LoadTimeline()
        this.Messenger.Raise <| InteractionMessage "Update"
