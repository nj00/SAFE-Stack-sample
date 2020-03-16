module Services.Auth

open System
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open Microsoft.IdentityModel.Tokens
open Saturn

open Microsoft.AspNetCore.Http
open Giraffe
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open FSharp.Control.Tasks.V2

open ApiCommon
open Shared


let secretKey = Guid.NewGuid().ToString()
let issuer = "nekoni.net"
let generateToken user =
    let expires = DateTime.UtcNow.AddHours(1.0)
    let claims = [|
        Claim(JwtRegisteredClaimNames.Sub, user);
        Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) |]
    claims
    |> Saturn.Auth.generateJWT (secretKey, SecurityAlgorithms.HmacSha256) issuer expires

let login(param: Login) = task {
    // ここは本来ならDBとチェックすることになるでしょう
    let ret = 
        match param.UserName, param.Password with
        | "guest", "guest" -> { UserName=param.UserName; Token=generateToken param.UserName }
        | _, _ -> failwith "ログインに失敗しました"
    return ret
}
let apiRoute:(HttpFunc -> HttpContext -> HttpFuncResult)  = 
    let api:IAuthApi = { 
        login = login >> Async.AwaitTask 
    }
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.publicRouteBuilder
    |> Remoting.withErrorHandler errorHandler
    |> Remoting.fromValue api
    |> Remoting.buildHttpHandler