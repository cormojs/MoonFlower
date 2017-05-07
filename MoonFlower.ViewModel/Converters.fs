namespace MoonFlower.ViewModel

open System
open System.Globalization
open System.Windows.Data

type OptionsTypeConverter() =

    // from http://stackoverflow.com/questions/6289761
    let (|SomeObj|_|) =
      let ty = typedefof<option<_>>
      fun (a:obj) ->
        let aty = a.GetType()
        let v = aty.GetProperty("Value")
        if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
          if a = null then None
          else Some(v.GetValue(a, [| |]))
        else None

    interface IValueConverter with

        member x.Convert(value: obj, targetType: Type, parameter: obj, culture: CultureInfo) =
            match value with
            | null -> null
            | SomeObj(v) -> v
            | _ -> value

        member x.ConvertBack(value: obj, targetType: Type, parameter: obj, culture: CultureInfo) =
            match value with
            | null -> None :> obj
            | x -> Activator.CreateInstance(targetType, [| x |])