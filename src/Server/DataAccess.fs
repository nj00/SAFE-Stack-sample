module DataAccess

open System
open System.Data
open System.Collections.Generic
open System.Dynamic
open Dapper

// https://stackoverflow.com/questions/42797288/dapper-column-to-f-option-property
type OptionHandler<'T> () =
    inherit SqlMapper.TypeHandler<option<'T>> ()

    override __.SetValue (param, value) =
        let valueOrNull =
            match value with
            | Some x -> box x
            | None   -> null

        param.Value <- valueOrNull

    override __.Parse value =
        if isNull value || value = box DBNull.Value
        then None
        else Some (value :?> 'T)

let addOptionHandlers() =
    // 他にも使用する型があるなら追加する
    SqlMapper.AddTypeHandler(OptionHandler<string>())
    SqlMapper.AddTypeHandler(OptionHandler<int>())
    SqlMapper.AddTypeHandler(OptionHandler<DateTime>())
    SqlMapper.AddTypeHandler(OptionHandler<decimal>())


let private logError work = 
    try
        work()
    with 
    | ex ->
        let name = ex.GetType().Name
        printfn "%s:%s" name ex.Message
        reraise()

// 参考　https://gist.github.com/vbfox/1e9f42f6dcdd9efd6660
let query<'Result> (sql:string) (connection:IDbConnection) : 'Result seq =
    let work() = 
        connection.Query<'Result>(sql)
    work |> logError

let parametrizedQuery<'Result> (sql:string) (param:obj) (connection:IDbConnection) : 'Result seq =
    let work() = 
        connection.Query<'Result>(sql, param)
    work |> logError
   
let mapParametrizedQuery<'Result> (sql:string) (param : Map<string,_>) (connection:IDbConnection) : 'Result seq =
    let expando = ExpandoObject()
    let expandoDictionary = expando :> IDictionary<string,obj>
    for paramValue in param do
        expandoDictionary.Add(paramValue.Key, paramValue.Value :> obj)

    connection |> parametrizedQuery sql expando

let execute (sql:string) (param:_) (connection:IDbConnection) =
    let work() = 
        let response = connection.Execute(sql, param)
        printfn "response:%d" response
        response
    work |> logError
