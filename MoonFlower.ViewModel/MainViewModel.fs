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
    inherit ViewModelBase()

    let config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
    let mutable app = AppModel()
    let messenger = InteractionMessenger()
    
    do
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
        app <- section.AppModel

    member this.Save() =
        let section = config.Sections.["AppConfigurationSection"] :?> AppConfigurationSection
        section.AppModel <- app
        JsonConvert.SerializeObject app |> Debug.WriteLine
        config.Save()
        sprintf "config saved to: %s" config.FilePath |> Debug.WriteLine
    member val Messenger = messenger
    member this.OAuth = OAuthViewModel(app, messenger)
    member this.Tooter = TooterViewModel(app, messenger)
    member this.Panes = [| PaneViewModel() |]
    member this.Reload() =
        self.RaisePropertyChanged(<@ this @>)
