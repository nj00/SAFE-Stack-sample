module Janken.View

open Fable.React
open Fulma

open Types

let render model dispatch =
    form [] [
      Field.div [] [ Label.label [ ] [ str "あなたの手" ] ]
      Field.div [ Field.IsGrouped ]
          [ Control.p [ ]
              [ Button.a
                  [ Button.Color IsInfo
                    Button.OnClick (fun _ -> dispatch Guu) ]
                  [ str "グー" ] ]
            Control.p [ ]
              [ Button.a
                  [ Button.Color IsInfo
                    Button.OnClick (fun _ -> dispatch Choki) ]
                  [ str "チョキ" ] ]
            Control.p [ ]
              [ Button.a
                  [ Button.Color IsInfo
                    Button.OnClick (fun _ -> dispatch Paa) ]
                  [ str "パー" ] ] ]
      Field.div []
        [ Label.label [] [str "結果"]
          Control.p [] [str (sprintf "%s" model.Result)]]
      Field.div []
        [ Label.label [] [str "勝敗"]
          Control.p [] [str (sprintf "%i 勝 %i 敗" model.Win model.Lost)]]
    ]