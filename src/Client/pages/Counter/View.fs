module Counter.View

open Fable.React
open Fulma

open Types

let show = function
| Some x -> string x
| None -> "Loading..."


let root (model:Model) dispatch =
    form [] [
      Field.div [ Field.IsGrouped ]
          [ Control.p [ ]
              [ Input.text
                  [ Input.Disabled true
                    Input.Value (show model) ] ]
            Control.p [ ]
              [ Button.a
                  [ Button.Color IsInfo
                    Button.OnClick (fun _ -> dispatch Increment) ]
                  [ str "+" ] ]
            Control.p [ ]
              [ Button.a
                  [ Button.Color IsInfo
                    Button.OnClick (fun _ -> dispatch Decrement) ]
                  [ str "-" ] ] ]
    ]