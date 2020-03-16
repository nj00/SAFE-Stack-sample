open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

// Dapperの初期化。null←→option の変換設定
DataAccess.addOptionHandlers()
// Sqliteの型変換設定
Repository.SqliteTypeHandler.addTypeHandlers()

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

// WebApiルート(jwtによるセキュアなルート)
let apiRouter = router {
    not_found_handler (text "404")
    pipe_through (Auth.requireAuthentication ChallengeType.JWT)
    forward "/ITaxonomyApi" Services.Taxonomies.apiRoute
}

// フリーWebApiルート
let publicRouter = router {
    not_found_handler (text "404")
    forward "/IAuthApi" Services.Auth.apiRoute
    forward "/ICounterApi" Services.Counter.apiRoute
}


// Topルーター
let topRouter = router {
    not_found_handler (text "404")
    forward "/public" publicRouter
    forward "/api" apiRouter
}

let app = application {
    use_jwt_authentication Services.Auth.secretKey Services.Auth.issuer

    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router topRouter
    memory_cache
    use_static publicPath
    use_gzip
    app_config DbInit.Initialize
}

run app
