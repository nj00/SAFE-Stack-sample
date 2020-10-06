module App.State

open Elmish
open Elmish.Navigation
open Fable.FontAwesome
open Browser.Dom
open Fable.Remoting.Client
open Thoth.Elmish

open Pages
open Shared

open App
open App.Types
open App.Notification

let urlUpdate (result: Page option) (model: App.Types.Model) =
    match result with
    | None ->
        console.error("Error parsing url: " + window.location.href)
        model, Navigation.modifyUrl (toPageUrl model.CurrentPage) 
    | Some Page.Home ->
        let m, cmd = Home.State.init()
        { model with PageModel = HomeModel m }, Cmd.map HomeMsg cmd
    | Some Page.Counter ->
        let m, cmd = Counter.State.init()
        { model with PageModel = CounterModel m }, Cmd.map CounterMsg cmd
    | Some Page.Janken ->
        let m, cmd = Janken.State.init()
        { model with PageModel = JankenModel m }, Cmd.map JankenMsg cmd
    | Some Page.Taxonomies ->
        let jwt = 
            match model.UserData with 
            | Some user -> user.Token
            | None -> ""
        let m, cmd = Taxonomies.State.init jwt
        { model with PageModel = TaxonomiesModel m }, Cmd.map TaxonomiesMsg cmd

let saveUserCmd user =
    Cmd.OfFunc.either
        UserStorage.save 
        user 
        (fun _ -> LoggedIn user) 
        StorageFailure

let removeUserCmd () =
    Cmd.OfFunc.either
        UserStorage.remove 
        ()
        (fun _ -> LoggedOut) 
        StorageFailure

let init result =
    let user = UserStorage.load()
    let (login, _) = LoginForm.init user
    let (home, _) = Home.State.init()
    let (model, cmd) =
      urlUpdate result
        { Note = ""
          LoginModel = login
          UserData = user
          PageModel = HomeModel home }
    model, cmd

let update msg model =
    match msg, model.PageModel with
    // 通知メッセージ
    | NotificationMsg msg, _ ->
        let errorToast note = 
            Toast.message note.Message
            |> Toast.position Toast.TopRight
            |> Toast.withCloseButton
            |> Toast.noTimeout
            |> Toast.icon Fa.Solid.TimesCircle
            |> Toast.error
        let warningToast note = 
            Toast.message note.Message
            |> Toast.position Toast.TopRight
            |> Toast.withCloseButton
            |> Toast.title note.Title
            |> Toast.noTimeout
            |> Toast.icon Fa.Solid.ExclamationTriangle
            |> Toast.warning
        let successToast note = 
            Toast.message note.Message
            |> Toast.position Toast.TopRight
            |> Toast.withCloseButton
            |> Toast.title note.Title
            |> Toast.icon Fa.Solid.CheckCircle
            |> Toast.success
        let infoToast note = 
            Toast.message note.Message
            |> Toast.position Toast.TopRight
            |> Toast.withCloseButton
            |> Toast.title note.Title
            |> Toast.icon Fa.Solid.InfoCircle
            |> Toast.info
        match msg with
        | MsgType.Error note ->
            model, errorToast note
        | MsgType.Warning note ->
            model, warningToast note
        | MsgType.Success note ->
            model, successToast note
        | MsgType.Info note ->
            model, infoToast note
    // 例外メッセージ
    | ErrorMsg exn, _ ->
        let notify (exn:exn) = 
            Cmd.ofMsg (NotificationMsg (MsgType.Error { Note.Title = ""; Message = exn.Message }))
        match exn with
        | :? ProxyRequestException as ex -> 
            match ex.StatusCode with
            | _ -> 
                { model with Note = ex.Message } , notify exn
        | _ ->
            { model with Note = exn.Message } , notify exn

    // ブラウザストレージアクセスエラー
    | StorageFailure e, _ ->
        printfn "Unable to access local storage: %A" e
        model, Cmd.ofMsg (ErrorMsg e)

    // ログイン
    | LoginMsg msg, _ ->
        let (loginModel, cmd) = LoginForm.update msg model.LoginModel
        match loginModel.State with
        | LoginForm.LoggedOut ->
            { model with LoginModel = loginModel }, Cmd.map LoginMsg cmd
        | LoginForm.LoggedIn user ->
            { model with LoginModel = loginModel }, saveUserCmd user
    | LoggedIn newUser, _ ->
        let page = 
            match model.PageModel with
            | TaxonomiesModel m -> TaxonomiesModel {m with Jwt = newUser.Token}
            | _ -> model.PageModel
        { model with UserData = Some newUser; PageModel = page }, Cmd.none

    // ログアウト
    | Logout, _ ->
        model, removeUserCmd()
    | LoggedOut, _ ->
        let (login, _) = LoginForm.init None
        { model with LoginModel = login; UserData = None }, Cmd.none

    // APIエラー
    | ApiError exn, _ ->    
        match exn with
        | :? ProxyRequestException as ex -> 
            match ex.StatusCode with
            | 401 ->    //Unauthorized
                console.log("Unauthorized");
                model, Cmd.ofMsg Logout
            | _ -> 
                model, Cmd.ofMsg (ErrorMsg exn)
        | _ ->
            model, Cmd.ofMsg (ErrorMsg exn)

    // Homeページ
    | HomeMsg msg, HomeModel m ->
        let (model', cmd) = Home.State.update msg m
        { model with PageModel = HomeModel model' }, Cmd.map HomeMsg cmd
    | HomeMsg _, _ ->
        model, Cmd.none
    
    // Counterページ
    | CounterMsg msg, CounterModel m ->
        let (model', cmd) = Counter.State.update msg m
        { model with PageModel = CounterModel model' }, Cmd.map CounterMsg cmd
    | CounterMsg _, _ ->
        model, Cmd.none

    // Jankenページ
    | JankenMsg msg, JankenModel m ->
        let (model', cmd) = Janken.State.update msg m
        { model with PageModel = JankenModel model' }, Cmd.map JankenMsg cmd
    | JankenMsg _, _ ->
        model, Cmd.none

    // Taxonomiesページ
    | TaxonomiesMsg msg, TaxonomiesModel m ->
        match msg with
        | Taxonomies.Types.Msg.ApiError exn -> 
            model, Cmd.ofMsg (ApiError exn)
        | Taxonomies.Types.Msg.Notify notifyMsg -> 
            model, Cmd.ofMsg (NotificationMsg notifyMsg)
        | _ ->
            let (model', cmd) = Taxonomies.State.update msg m
            { model with PageModel = TaxonomiesModel model' }, Cmd.map TaxonomiesMsg cmd
    | TaxonomiesMsg _, _ ->
        model, Cmd.none
    