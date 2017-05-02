module main

open System
open FsXaml

type App = XAML<"App.xaml">

[<STAThread>]
[<EntryPoint>]
let Main argv =
    App().Root.Run()