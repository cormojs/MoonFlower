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
    
    let messenger = InteractionMessenger()
    let config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
    
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
        this.Reload()

    member this.Save() =
        let section = config.Sections.["AppConfigurationSection"] :?> AppConfigurationSection
        section.AppModel <- this.App
        JsonConvert.SerializeObject this.App |> Debug.WriteLine
        config.Save()
        sprintf "config saved to: %s" config.FilePath |> Debug.WriteLine
    member this.OAuth = OAuthViewModel(this, messenger)
    member this.Tooter = TooterViewModel(this, messenger)
    member this.Panes = [| PaneViewModel() |]
    member this.Reload() =
        self.RaisePropertyChanged(<@ this @>)
