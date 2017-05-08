namespace MoonFlower.ViewModel

open MoonFlower.Model
open Livet.Messaging

type IMainViewModel =
    abstract member App: AppModel with get, set
    abstract member Messenger: InteractionMessenger with get