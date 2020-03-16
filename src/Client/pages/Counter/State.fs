module Counter.State

open Elmish
open Fable.Remoting.Client
open Types
open Shared

let api : ICounterApi =
  Remoting.createApi()
  |> Remoting.withRouteBuilder Route.publicRouteBuilder
  |> Remoting.buildProxy<ICounterApi>

let init () : Model * Cmd<Msg> =
    let model = None
    let cmd =
        Cmd.OfAsync.either
            api.initialCounter
            ()
            (Ok >> Init)
            (Error >> Init)
    model, cmd


let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match model,  msg with
    | _, Init (Ok x) -> Some x, Cmd.none
    | Some x, Increment -> Some (x + 1), Cmd.none
    | Some x, Decrement -> Some (x - 1), Cmd.none
    | _ -> None, Cmd.none
