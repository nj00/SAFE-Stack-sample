module Server

open Saturn
open Giraffe.Core
open Giraffe.ResponseWriters

open Shared


// Dapperの初期化。null←→option の変換設定
DataAccess.addOptionHandlers()
// Sqliteの型変換設定
Repository.SqliteTypeHandler.addTypeHandlers()


// WebApiルート(jwtによるセキュアなルート)
let apiRouter = router {
    not_found_handler (setStatusCode 404 >=> text "Api 404")
    pipe_through (Auth.requireAuthentication ChallengeType.JWT)
    forward "/ITaxonomyApi" Services.Taxonomies.apiRoute
}

// フリーWebApiルート
let publicRouter = router {
    not_found_handler  (setStatusCode 404 >=> text "Api 404")
    forward "/IAuthApi" Services.Auth.apiRoute
    forward "/ICounterApi" Services.Counter.apiRoute
}


// Topルーター
let topRouter = router {
    not_found_handler  (setStatusCode 404 >=> text "Api 404")
    forward "/public" publicRouter
    forward "/api" apiRouter
}

let app = application {
    use_jwt_authentication Services.Auth.secretKey Services.Auth.issuer

    url "http://0.0.0.0:8085"
    use_router topRouter
    memory_cache
    use_static "public"
    use_gzip
    app_config DbInit.Initialize
}

run app
