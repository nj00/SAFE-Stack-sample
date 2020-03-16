module Janken.State

open Elmish
open Types

let init () : Model * Cmd<Msg> =
  { 
      Win = 0
      Lost = 0
      Result=""
  }, 
  []

let update msg model : Model * Cmd<Msg> =
  let rnd = System.Random()

  let intToChoice num =
    match num with
    | 1 -> Guu
    | 2 -> Choki
    | _ -> Paa

  let cpuChoice = rnd.Next(1, 4) |> intToChoice
  let playerChoice = msg

  match playerChoice, cpuChoice with
  | Guu, Choki
  | Choki, Paa
  | Paa, Guu
    -> { model with 
            Win = model.Win + 1
            Result = "あなたの勝ち"}, []

  | Guu, Paa
  | Choki, Guu
  | Paa, Choki
    -> { model with 
            Lost = model.Lost + 1
            Result = "あなたの負け"}, []
  | _
    -> { model with Result = "あいこ"}, []
