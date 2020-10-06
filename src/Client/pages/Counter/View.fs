module Counter.View

open Feliz
open Feliz.Bulma

open Types

let show = function
| Some x -> string x
| None -> "Loading..."


let render (model:Model) dispatch =
  Html.form [
    Bulma.field.div [
      field.isGrouped
      prop.children [
        Bulma.control.p [
          Bulma.input.text [
            prop.disabled true
            prop.value (show model)
          ]
        ]
        Bulma.control.p [
          Bulma.button.button [
            color.isInfo
            prop.onClick (fun _ -> dispatch Increment)
            prop.text "+"
          ]
        ]
        Bulma.control.p [
          Bulma.button.button [
            color.isInfo
            prop.onClick (fun _ -> dispatch Decrement)
            prop.text "-"
          ]
        ]
      ]
    ]
  ]
