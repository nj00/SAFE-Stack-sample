module Home.View

open Fable.Core.JsInterop
open Fable.React
open Fulma
open Fable.FontAwesome

open Types

let render model dispatch =
  form [] [
    Field.div [ ]
        [ Label.label [ ] [ str "Username" ]
          Control.div [ Control.HasIconLeft
                        Control.HasIconRight ]
            [ Input.text [ Input.Color IsSuccess
                           Input.DefaultValue ""
                           Input.OnChange (fun ev -> !!ev.target?value |> ChangeStr |> dispatch )]
              Fa.i [ Fa.Solid.User 
                     Fa.Size Fa.FaSmall
                     Fa.PullLeft ]
                   []
              Fa.i [ Fa.Solid.Check 
                     Fa.Size Fa.FaSmall
                     Fa.PullRight] 
                   [] 
            ]
          Help.help [ Help.Color IsSuccess ]
            [ str (sprintf "Hello %s" model) ] ]
  ]
