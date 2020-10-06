module Home.View

open Fable.FontAwesome

open Feliz
open Feliz.Bulma

open Types

let render model dispatch =
  Html.form [
    Bulma.field.div [
      Bulma.label [ prop.text "UserName"]
      Bulma.control.div [
        control.hasIconsLeft; control.hasIconsRight
        prop.children [
          Bulma.input.text [ 
            color.isSuccess
            prop.defaultValue ""
            prop.onChange (ChangeStr >> dispatch)
          ]
          Bulma.icon [
            icon.isLeft; icon.isSmall
            prop.children [Fa.i [ Fa.Solid.User ] []]
          ]
          Bulma.icon [
            icon.isRight; icon.isSmall
            prop.children [Fa.i [ Fa.Solid.Check ] []]
          ]
        ]
      ]
      Bulma.help [
        color.isSuccess
        prop.textf "Hello %s" model
      ]
    ]
  ]
