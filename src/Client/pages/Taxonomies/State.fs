module Taxonomies.State

open Elmish
open Fable.Remoting.Client
open Types
open Shared
open Shared.BlogModels

open App.Notification
let initPagenation = { rowsPerPage = 5L; currentPage = 1L; allRowsCount = -1L;}

let getApi jwt : ITaxonomyApi =
    let header = sprintf "Bearer %s" jwt
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.apiRouteBuilder
    |> Remoting.withAuthorizationHeader header
    |> Remoting.buildProxy<ITaxonomyApi>

/// <summary>
/// 一覧取得
/// </summary>
let getList jwt (criteria:ListCriteria) =
    let param = { taxonomyType =
                    match criteria.taxonomyType with
                    | Category -> Some TaxonomyTypeEnum.Category
                    | Tag -> Some TaxonomyTypeEnum.Tag
                    | Series -> Some TaxonomyTypeEnum.Series
                    | _ -> None
                  pagenation = criteria.page }

    Cmd.OfAsync.either
        (getApi jwt).getTaxonomies
        param
        (Ok >> Loaded)
        ApiError

/// <summary>
/// Init関数
/// <summary>
let init jwt : Model * Cmd<Msg> =
    let model = {
        jwt = jwt
        listCriteria = { taxonomyType = All; page = initPagenation}
        dataList = None
        currentRec = None
    }
    let cmd = getList model.jwt model.listCriteria
    model, cmd

/// <summary>
/// Update関数
/// </summary>
let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    let jwt = model.jwt
    let api = getApi jwt
    let getList = getList jwt
    
    let isNewRec (taxonomy:Taxonomy) =
        taxonomy.Id < 0L

    // idが負の値は追加、それ以外は更新を行う
    let insertOrUpdate (taxonomy:Taxonomy) : Cmd<Msg> =
        let serverApi = 
            if isNewRec taxonomy then
                api.addNewTaxonomy
            else
                api.updateTaxonomy
        Cmd.OfAsync.either serverApi taxonomy (Ok >> Saved) ApiError

    // 保存後のコマンド
    let savedCmd (note:Note) =
        // 通知と再表示
        Cmd.batch [
            Cmd.ofMsg (Notify (MsgType.Success note))
            Cmd.ofMsg Reload]    

    match msg with
    // 一覧再読み込み
    | Reload -> 
        {model with currentRec = None}, getList model.listCriteria

    // 一覧抽出条件変更
    | CriteriaChanged x ->
        let newCriteria = {x with page = initPagenation } 
        {model with listCriteria = newCriteria}, Cmd.ofMsg Reload

    // ページング
    | PageChanged newPage ->
        {model with listCriteria = { model.listCriteria with page = newPage} }, Cmd.ofMsg Reload

    // 一覧読み込み後
    | Loaded (Ok x) -> 
        { model with dataList = Some x.data; listCriteria = {model.listCriteria with page = x.pagenation } }, 
        Cmd.none

    // 新規追加
    | AddNew -> 
        { model with currentRec = Some { Id = -1L; Type = TaxonomyTypeEnum.Category; Name = ""; UrlSlug = ""; Description = None; }}, 
        Cmd.none

    // 一覧からデータ選択
    | Select x -> 
        model, 
        Cmd.OfAsync.either
            api.getTaxonomy 
            x.Id 
            (Ok >> Selected) 
            ApiError
    | Selected (Ok x) ->
        { model with currentRec = x }, Cmd.none
    
    // 値変更
    | RecordChanged changed ->
        { model with currentRec = Some changed }, Cmd.none

    // 保存
    | Save x ->
        { model with currentRec = Some x}, insertOrUpdate x
    | Saved (Ok _)->
        let newCurrent = 
            if isNewRec model.currentRec.Value then
                // 新規の場合、末尾に持っていく。サーバー側でcurrentPageが調整される
                System.Int64.MaxValue
            else
                model.listCriteria.page.currentPage
        let newPager = {model.listCriteria.page with currentPage = newCurrent}
        let newCriteria = {model.listCriteria with page = newPager } 
        { model with listCriteria = newCriteria }, savedCmd { title=""; message="保存しました。" }

    // 削除
    | Remove x ->
        model,
        Cmd.OfAsync.either
            api.removeTaxonomy
            x
            (Ok >> Removed)
            ApiError
    | Removed (Ok _)->
        model, savedCmd { title=""; message="削除しました。" }

    // Apiエラーと通知はそのまま伝搬
    | ApiError _ | Notify _ ->
        model, Cmd.ofMsg msg

    | _ -> model, Cmd.none
