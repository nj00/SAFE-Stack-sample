module ViewHelper

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Browser.Types
open Elmish.Navigation
open Thoth.Elmish
open Fulma

open Shared
open System

// TODO: Keyboard.Codes.enterはどこ行ったのだろう...
let KeyCodeEnter = 13.

let goToUrl (e: MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore
let onEnter msg dispatch =
    function
    | (ev:KeyboardEvent) when ev.keyCode = KeyCodeEnter ->
        ev.preventDefault()
        dispatch msg
    | _ -> ()
    |> OnKeyDown


// Thoth.Elmish.Toastのレンダラ　https://github.com/thoth-org/Thoth.Elmish.Toast/blob/master/demo/src/Toast.fs
let renderToastWithFulma =
    { new Toast.IRenderer<Fa.IconOption> with
        member __.Toast children color =
            Notification.notification [ Notification.CustomClass color ]
                children
        member __.CloseButton onClick =
            Notification.delete [ Props [ OnClick onClick ] ]
                [ ]
        member __.InputArea children =
            Columns.columns [ Columns.IsGapless
                              Columns.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]
                              Columns.CustomClass "notify-inputs-area" ]
                children
        member __.Input (txt : string) (callback : (unit -> unit)) =
            Column.column [ ]
                [ Button.button [ Button.OnClick (fun _ -> callback ())
                                  Button.Color IsWhite ]
                    [ str txt ] ]
        member __.Title txt =
            // カスタマイズ(txtがブランクの場合は領域を取らないように)
            if System.String.IsNullOrEmpty(txt) then
                ofOption None
            else
                Heading.h5 []
                    [ str txt ]
        member __.Icon (icon : Fa.IconOption) =
            Icon.icon [ Icon.Size IsMedium ]
                [ Fa.i [ icon
                         Fa.Size Fa.Fa2x ]
                    [ ] ]  
        member __.SingleLayout title message =
            div [ ]
                [ title; message ]
        member __.Message txt =
            span [ ]
                [ str txt ]
        member __.SplittedLayout iconView title message =
            Columns.columns [ Columns.IsGapless
                              Columns.IsVCentered ]
                [ Column.column [ Column.Width (Screen.All, Column.Is2) ]
                    [ iconView ]
                  Column.column [ ]
                    [ title
                      message ] ]
        member __.StatusToColor status =
            match status with
            | Toast.Success -> "is-success"
            | Toast.Warning -> "is-warning"
            | Toast.Error -> "is-danger"
            | Toast.Info -> "is-info" }