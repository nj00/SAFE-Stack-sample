module ViewComponent.ListPager

open Fable.React
open Fable.React.Props
open Fulma

open Shared


let pager (model:PagerModel) pageAction dispatch =
  let itemCount = 7L
  let firstPage = 1L
  let lastPage = model.LastPage
  let pageLink page =
    Pagination.link 
      [ Pagination.Link.Current (model.currentPage = page) 
        Pagination.Link.Props [OnClick (fun _ -> pageAction { model with currentPage = page} |> dispatch) ] ] 
      [ str (string page) ]
  let pageEllipsis left right x =
    let subs = right - left - 1L
    match sign subs with
      | 0 -> pageLink x
      | 1 -> Pagination.ellipsis []
      | _ -> ofOption None

  let pageList = 
    if lastPage <= itemCount then
      [1L..lastPage]
        |> List.map pageLink
    else
      let leftLinks = 
        if model.currentPage = firstPage then []
        elif model.currentPage = firstPage + 1L then [pageLink firstPage]
        elif model.currentPage  = firstPage + 2L then [pageLink firstPage; pageLink (model.currentPage - 1L)]
        else [pageLink firstPage; pageEllipsis firstPage (model.currentPage - 2L) (firstPage + 1L); pageLink (model.currentPage - 1L)]
      let rightLinks = 
        if model.currentPage = lastPage then []
        elif model.currentPage = lastPage - 1L then [pageLink lastPage]
        elif model.currentPage = lastPage - 2L then [pageLink (model.currentPage + 1L); pageLink lastPage]
        else [pageLink (model.currentPage + 1L); pageEllipsis (model.currentPage + 2L) lastPage (lastPage - 1L); pageLink lastPage]
      let currentPageLink = [pageLink model.currentPage]
      List.concat [leftLinks; currentPageLink; rightLinks]

  if lastPage = 1L then
    ofOption None
  else
    Pagination.pagination [ Pagination.Size IsSmall ]
      [ Pagination.previous 
          [ Props [
              Disabled (model.currentPage = 1L);
              OnClick (fun _ -> pageAction { model with currentPage = model.currentPage - 1L} |> dispatch) ] ]
          [ str "前ページ" ]
        Pagination.next 
          [ Props [ 
              Disabled (model.currentPage = lastPage);
              OnClick (fun _ -> pageAction { model with currentPage = model.currentPage + 1L} |> dispatch) ] ]
          [ str "次ページ" ]
        Pagination.list [ ] pageList 
      ]

