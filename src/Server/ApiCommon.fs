module ApiCommon

open System
open Microsoft.AspNetCore.Http
open Fable.Remoting.Server
open Shared

let errorHandler (ex: Exception) (routeInfo: RouteInfo<HttpContext>) = 
    // do some logging
    printfn "Error at %s on method %s" routeInfo.path routeInfo.methodName
    // decide whether or not you want to propagate the error to the client
    match ex with
    | x ->
        let err = 
            if x.InnerException = null then { errorMsg = x.Message }
            else { errorMsg = x.InnerException.Message }
        Propagate err
