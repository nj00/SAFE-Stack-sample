module Services.Taxonomies

open System.Threading.Tasks

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open Microsoft.Extensions.Configuration

let getTaxonomies (connectionString:string) (param: GetTaxonomiesParam) :Task<GetTaxonomiesResult> = task {
    return Repository.getTaxonomies connectionString param.taxonomyType param.pagenation
}

let getTaxonomy (connectionString:string) (id:int64) = task {
    return Repository.getTaxonomy connectionString id
}

let addNewTaxonomy (connectionString:string) (record:BlogModels.Taxonomy) = task {
    return Repository.addNewTaxonomy connectionString record
}

let updateTaxonomy (connectionString:string) (record:BlogModels.Taxonomy) = task {
    return Repository.updateTaxonomy connectionString record
}

let removeTaxonomy (connectionString:string) (record:BlogModels.Taxonomy) = task {
    return Repository.removeTaxonomy connectionString record
}

let apiRoute:(HttpFunc -> HttpContext -> HttpFuncResult) =
    let getApi (ctx:HttpContext) :ITaxonomyApi = 
        // let config = ctx.RequestServices.GetService(typeof<IConfiguration>) :?> IConfiguration
        let config = ctx.RequestServices.GetService<IConfiguration>();
        let connectionString = config.GetConnectionString("BlogDb")
        printfn "ConnectionString:%s" connectionString
        
        { 
            getTaxonomies = getTaxonomies connectionString >> Async.AwaitTask
            getTaxonomy = getTaxonomy connectionString >> Async.AwaitTask
            addNewTaxonomy = addNewTaxonomy connectionString >> Async.AwaitTask
            updateTaxonomy = updateTaxonomy connectionString >> Async.AwaitTask
            removeTaxonomy = removeTaxonomy connectionString >> Async.AwaitTask
        }

    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.apiRouteBuilder
    |> Remoting.fromContext getApi
    |> Remoting.buildHttpHandler
