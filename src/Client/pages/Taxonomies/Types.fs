module Taxonomies.Types

open App.Notification
open Shared

type TaxonomyType =
  | All
  | Category
  | Tag
  | Series

type ListCriteria = {
  TaxonomyType: TaxonomyType;
  Page: PagerModel;
}

type Model = {
  Jwt: string
  ListCriteria: ListCriteria
  DataList: seq<BlogModels.Taxonomy> option
  CurrentRec: BlogModels.Taxonomy option
}

type Msg =
  | Reload
  | CriteriaChanged of ListCriteria
  | PageChanged of PagerModel
  | Loaded of Result<GetTaxonomiesResult, exn>
  | AddNew
  | Select of BlogModels.Taxonomy
  | Selected of Result<BlogModels.Taxonomy option, exn>
  | UnSelect
  | RecordChanged of BlogModels.Taxonomy
  | Save of BlogModels.Taxonomy
  | Saved of Result<int, exn>
  | Remove of BlogModels.Taxonomy
  | Removed of Result<int, exn>
  | ApiError of exn
  | Notify of MsgType
 
