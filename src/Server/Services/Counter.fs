module Services.Counter

open System.Threading.Tasks

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2
open Giraffe
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared

let getInitCounter () : Task<Counter> = task { return 42 }
let apiRoute:(HttpFunc -> HttpContext -> HttpFuncResult)  = 
    let api:ICounterApi = { 
        initialCounter = getInitCounter >> Async.AwaitTask
    }
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.publicRouteBuilder
    |> Remoting.fromValue api
    |> Remoting.buildHttpHandler