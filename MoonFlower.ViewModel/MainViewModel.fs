namespace MoonFlower.ViewModel

open System.Configuration
open System.Diagnostics

open Newtonsoft.Json

open ViewModule
open ViewModule.FSharp

open MoonFlower.Model
open MoonFlower.ViewModel

type MainViewModel() as self =
    inherit ViewModelBase()

    let inputText = self.Factory.Backing(<@ self.InputText @>, "")
    let config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
    
    member this.Load() =
        JsonConvert.SerializeObject(AppModel())
        |> Debug.WriteLine
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
        self.App <- section.AppModel

    member val App: AppModel = AppModel() with get, set

    member this.Save() =
        let section = config.Sections.["AppConfigurationSection"] :?> AppConfigurationSection
        section.AppModel <- this.App
        config.Save()
        sprintf "config saved to: %s" config.FilePath |> Debug.WriteLine
    member this.OAuth = OAuthViewModel(this.App)
    member this.Panes = [| PaneViewModel() |]
    member this.InputText
        with get() = inputText.Value
        and set(v) = inputText.Value <- v
