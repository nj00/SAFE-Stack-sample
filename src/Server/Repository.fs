module Repository

open System.Data.SQLite
open DataAccess
open Shared
open Shared.BlogModels
open Dapper

module SqliteTypeHandler =
    open Dapper
    type TaxonomyTypeEnumHandler () =
        inherit SqlMapper.TypeHandler<TaxonomyTypeEnum> ()

        override __.SetValue (param, value) =
            param.Value <- value

        override __.Parse value =
            enum<TaxonomyTypeEnum> (value :?> int)

    let addTypeHandlers () =
        SqlMapper.AddTypeHandler(TaxonomyTypeEnumHandler())

let getConnection (connectionString:string) = 
    new SQLiteConnection(connectionString)


let getTaxonomies (connectionString:string) (taxonomyType:TaxonomyTypeEnum option) (page:PagerModel)  =
    let connection = getConnection connectionString

    let sqlWhere = 
        match taxonomyType with
        | None -> ""
        | Some x -> sprintf "where [Type] = %d " (int x)

    let getCount criteria =
        let sql = 
            """
            select count(1) as [cnt]
            from [Taxonomy]
            """
        connection 
        |> query<int64> (sql + criteria) |> Seq.head
    let newPager = {page with allRowsCount = getCount sqlWhere }
    let newCurrent = min newPager.currentPage newPager.LastPage

    let getList criteria =    
        let sql = 
            """
            select *
            from [Taxonomy]
            """
        let sqlOrder = "order by [Id] "
        let sqlLimitAndOffset = sprintf "limit %d offset %d" page.rowsPerPage ((newCurrent - 1L) * page.rowsPerPage)
        connection 
        |> query<Taxonomy> (sql + criteria + sqlOrder + sqlLimitAndOffset)

    { data = getList sqlWhere
      pagenation = {newPager with currentPage = newCurrent} }


type IdParam = { 
    Id : int64;
}
let getTaxonomy (connectionString:string) (id:int64) =
    let connection = getConnection connectionString

    let sql =
        """
        select * from [Taxonomy]
        where [Id] = @Id
        """
    let param = {Id = id}

    connection 
    |> parametrizedQuery<Taxonomy> sql param
    |> Seq.tryHead


let addNewTaxonomy (connectionString:string) (record:Taxonomy) =
    let connection = getConnection connectionString

    let sql = 
        """
        insert into [Taxonomy] (
          [Type] 
         ,[Name]
         ,[UrlSlug]
         ,[Description]
        )
        values (
          @Type 
         ,@Name 
         ,@UrlSlug 
         ,@Description
        )
        """
    connection 
    |> execute sql record

let updateTaxonomy (connectionString:string) (record:Taxonomy) =
    let connection = getConnection connectionString

    let sql = 
        """
        update [Taxonomy] 
        set 
          [Type] = @Type
         ,[Name] = @Name
         ,[UrlSlug] = @UrlSlug
         ,[Description] = @Description
        where [Id] = @Id
        """
    connection 
    |> execute sql record

let removeTaxonomy (connectionString:string) (record:Taxonomy) =
    let connection = getConnection connectionString

    let sql =
        """
        delete from [Taxonomy]
        where [Id] = @Id
        """
    connection 
    |> execute sql record        