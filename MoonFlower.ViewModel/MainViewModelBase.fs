namespace MoonFlower.ViewModel

open MoonFlower.Model
open ViewModule
open Livet.Messaging

type MainViewModelBase() =
    inherit ViewModelBase()
    member val Messenger: InteractionMessenger = InteractionMessenger() with get
    member val App: AppModel = AppModel() with get, set
