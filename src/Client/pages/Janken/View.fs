module Janken.View

open Feliz
open Feliz.Bulma

open Types

let render model dispatch =
  Html.form [
    Bulma.field.div [
      Bulma.label [prop.text "あなたの手"]
    ]
    Bulma.field.div [
      field.isGrouped
      
      prop.children [
        Bulma.control.p [
          Bulma.buttons [
            Bulma.button.button [
              color.isPrimary
              prop.onClick (fun _ -> dispatch Guu)
              prop.text "グー"
            ]
            Bulma.button.button [
              color.isInfo
              prop.onClick (fun _ -> dispatch Choki)
              prop.text "チョキ"
            ]
            Bulma.button.button [
              color.isDanger
              prop.onClick (fun _ -> dispatch Paa)
              prop.text "パー"
            ]
          ]
        ]
      ]
    ]
    Bulma.field.div [
      Bulma.label [prop.text "結果"]
      Bulma.control.p [prop.textf "%s" model.Result]
    ]
    Bulma.field.div [
      Bulma.label [prop.text "勝敗"]
      Bulma.control.p [prop.textf "%i 勝 %i 敗" model.Win model.Lost]
    ]
  ]
