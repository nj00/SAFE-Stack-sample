module App.LoginForm

open System
open Elmish
open Fable.React
open Fable.React.Props
open Fable.Remoting.Client
open Thoth.Json
open Fulma

open Shared

type LoginState =
    | LoggedOut
    | LoggedIn of UserData

type Model = {
    State : LoginState
    Login : Login
    ErrorMsg : string }

type Msg =
    | LoginModelChanged of Login
    | ClickLogin
    | LoginResult of Result<UserData, exn>

let api : IAuthApi =
  Remoting.createApi()
  |> Remoting.withRouteBuilder Route.publicRouteBuilder
  |> Remoting.buildProxy<IAuthApi>


let init (user:UserData option) =
    match user with
    | None ->
        { Login = { UserName = ""; Password = "" }
          State = LoggedOut
          ErrorMsg = "" }, Cmd.none
    | Some user ->
        { Login = { UserName = user.UserName; Password = "" }
          State = LoggedIn user
          ErrorMsg = "" }, Cmd.none

let update (msg:Msg) model : Model*Cmd<Msg> =
    match msg with
    | LoginModelChanged login ->
        { model with Login = login }, Cmd.none
    | ClickLogin ->
        let cmd =
            Cmd.OfAsync.either
                api.login
                model.Login
                (Ok >> LoginResult)
                (Error >> LoginResult)
        model, cmd
    | LoginResult (Ok user) ->
        { model with State = LoggedIn user; Login = { model.Login with Password = "" } }, Cmd.none
    | LoginResult (Error exn) ->
        match exn with
        | :? ProxyRequestException as ex -> 
            printfn "%s" ex.ResponseText
            let response = Decode.Auto.unsafeFromString<ErrorResponse> ex.ResponseText
            { model with ErrorMsg = string (response.error.errorMsg) }, Cmd.none
        | _ ->
            { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model dispatch = 
    Modal.modal [ Modal.IsActive (model.State = LoggedOut) ]
        [ Modal.background [ ] [ ]
          Modal.Card.card [ ]
            [ Modal.Card.head [ ]
                [ Modal.Card.title [ ]
                    [ str "Login" ] ]
              Modal.Card.body [ ]
                [ 
                    form [ ]
                        [ Field.div [ ]
                            [ Control.div [ ]
                                [ Input.text
                                    [ Input.Placeholder "Your Id"
                                      Input.Value model.Login.UserName
                                      Input.OnChange (fun ev -> LoginModelChanged {model.Login with UserName = ev.Value} |> dispatch )
                                      Input.Props [ AutoFocus true ] 
                                    ] ] ]
                          Field.div [ ]
                            [ Control.div [ ]
                                [ Input.password
                                    [ Input.Placeholder "Your Password" 
                                      Input.Value model.Login.Password
                                      Input.OnChange (fun ev -> LoginModelChanged {model.Login with Password = ev.Value} |> dispatch)
                                    ] ] ]

                          Field.div [ Field.Props [ Hidden (String.IsNullOrEmpty model.ErrorMsg) ] ]
                            [ Message.message [ Message.Color IsDanger ]
                                             [ Message.body [ ]
                                                [ str model.ErrorMsg ] ] ]
                        ] 
                ]
              Modal.Card.foot [ ]
                [ Button.button 
                    [ Button.Color IsSuccess 
                      Button.OnClick (fun _ -> ClickLogin |> dispatch)
                    ] 
                    [ str "Login" ] ] 
            ] 
        ]