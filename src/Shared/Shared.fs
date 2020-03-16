namespace Shared

type Counter = int

module Route =
    /// Defines how routes are generated on server and mapped from client
    let apiRouteBuilder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName
    let publicRouteBuilder typeName methodName =
        sprintf "/public/%s/%s" typeName methodName

/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html

type ApiError = { errorMsg: string }
type ErrorResponse = {
    error: ApiError
    // ignored: bool
    // handled: bool
}

type ICounterApi =
    { initialCounter : unit -> Async<Counter> }

/// ListPagerのModel
type PagerModel = {
  rowsPerPage : int64;
  currentPage : int64;
  allRowsCount : int64;
} with 
      member m.LastPage = 
        (m.allRowsCount / m.rowsPerPage) + (if m.allRowsCount % m.rowsPerPage > 0L then 1L else 0L)  

open BlogModels

[<AutoOpen>]
module Taxonomies =
    type GetTaxonomiesParam = {
        taxonomyType: TaxonomyTypeEnum option;
        pagenation: PagerModel;
    }
    type GetTaxonomiesResult = {
        data: seq<Taxonomy>;
        pagenation: PagerModel;
    }

    type ITaxonomyApi = {
        getTaxonomies : GetTaxonomiesParam -> Async<GetTaxonomiesResult>
        getTaxonomy : int64 -> Async<Taxonomy option>
        addNewTaxonomy : Taxonomy -> Async<int>
        updateTaxonomy : Taxonomy -> Async<int>
        removeTaxonomy : Taxonomy -> Async<int>
    }

[<AutoOpen>]
module Auth =
    // Json web token type.
    type JWT = string
    type UserData = { 
        UserName : string
        Token    : JWT 
    }
    // Login credentials.
    type Login = { 
        UserName   : string
        Password   : string
    }
    type IAuthApi = {
        login : Login -> Async<UserData>
    }
