module Taxonomies.Types

open App.Notification
open Shared

type TaxonomyType =
  | All
  | Category
  | Tag
  | Series

type ListCriteria = {
  taxonomyType: TaxonomyType;
  page: PagerModel;
}

type Model = {
  jwt: string
  listCriteria: ListCriteria
  dataList: seq<BlogModels.Taxonomy> option
  currentRec: BlogModels.Taxonomy option
}

type Msg =
  | Reload
  | CriteriaChanged of ListCriteria
  | PageChanged of PagerModel
  | Loaded of Result<GetTaxonomiesResult, exn>
  | AddNew
  | Select of BlogModels.Taxonomy
  | Selected of Result<BlogModels.Taxonomy option, exn>
  | RecordChanged of BlogModels.Taxonomy
  | Save of BlogModels.Taxonomy
  | Saved of Result<int, exn>
  | Remove of BlogModels.Taxonomy
  | Removed of Result<int, exn>
  | ApiError of exn
  | Notify of MsgType
 
