module DbInit

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open DataAccess
open Repository
open Shared.BlogModels

let private createTaxonomyTable conn =
    let sql = 
        """
        create table if not exists [Taxonomy] (
            [Id] integer primary key autoincrement,
            [Type] int not null,
            [Name] text not null,
            [UrlSlug] text not null,
            [Description] text null
        )
        """
    conn
    |> execute sql null
    |> ignore

let private clearTaxonomyTable conn =
    let sql = 
        """
        delete from [Taxonomy];
        vacuum
        """
    conn
    |> execute sql null
    |> ignore

let private existsTaxonomies conn =
    let sql =
        """
        select count(1) from [Taxonomy]
        """
    conn
    |> query<int> sql
    |> Seq.head

let private addTaxonomies (records:List<Taxonomy>) conn =
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
        );
        select [Id] from [Taxonomy] where ROWID = last_insert_rowid()
        """
    conn 
    |> execute sql records
    |> ignore

let Initialize (app:IApplicationBuilder) =
    let config = app.ApplicationServices.GetService<IConfiguration>()
    let connectionString = config.GetConnectionString("BlogDb")
    let conn = getConnection connectionString

    // テーブル作成
    conn |> createTaxonomyTable

    // データ追加
    match (conn |> existsTaxonomies) with 
    | 0 -> 
        // データ追加
        let records = [
            {Id=0L; Type=TaxonomyTypeEnum.Category; Name=".NET"; UrlSlug="dotnet"; Description=Some ".NET Framework, .NET Core に関する話題が中心です。"}
            {Id=0L; Type=TaxonomyTypeEnum.Category; Name="猫"; UrlSlug="cats"; Description=Some "飼っている２匹の猫の話題が中心です。"}
            {Id=0L; Type=TaxonomyTypeEnum.Tag; Name="ASP.NET Core"; UrlSlug="asp-net-core"; Description=Some "ASP.NET Coreに関する話題です。"}
            {Id=0L; Type=TaxonomyTypeEnum.Tag; Name="nekoni.net"; UrlSlug="create-nekoni-net"; Description=Some "本サイトの開発に関する話題です。"}
            {Id=0L; Type=TaxonomyTypeEnum.Tag; Name="マロ"; UrlSlug="maro"; Description=Some "うちの営業部長。先住猫のマロに関しての話題です。"}
            {Id=0L; Type=TaxonomyTypeEnum.Tag; Name="フク"; UrlSlug="fuku"; Description=Some "しんねりさん。２匹目の猫、フクちゃんに関しての話題です。"}
        ]
        conn |> addTaxonomies records
        app    
    | _ -> app
