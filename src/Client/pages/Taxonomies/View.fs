module Taxonomies.View

open Fable.Core
open Fable.React
open Fable.React.Props
open Fulma
open Fable.FontAwesome
open ViewComponent
open Types
open Shared
open Shared.BlogModels

let optionToString = function
| Some x -> string x
| None -> ""

let typeEnumToString typeEnum =
  match typeEnum with
  | TaxonomyTypeEnum.Category -> "Category"
  | TaxonomyTypeEnum.Tag -> "Tag"
  | TaxonomyTypeEnum.Series -> "Series"
  | _ -> ""
let typeEnumToStr = typeEnumToString >> str

let typeEnumOfSelectValue value =
  match value with
  | "0" -> TaxonomyTypeEnum.Category
  | "1" -> TaxonomyTypeEnum.Tag
  | "2" -> TaxonomyTypeEnum.Series
  | _ -> TaxonomyTypeEnum.Category

/// <summary>
/// 一覧部分
/// </summary>
let listView (model:Model) dispatch =
    let criteria (param:ListCriteria) =
      let valueOfTaxonomyType = function
        | All -> "All"
        | Category -> typeEnumToString TaxonomyTypeEnum.Category
        | Tag -> typeEnumToString TaxonomyTypeEnum.Tag
        | Series -> typeEnumToString TaxonomyTypeEnum.Series
      let strOfTaxonomyType = valueOfTaxonomyType >> str

      let taxonomyTypeOfValue = function
        | "All" -> All
        | "Category" -> Category
        | "Tag" -> Tag
        | "Series" -> Series
        | _ -> All

      Level.level [] [
          Level.left [] [
            Level.item [] [ Label.label [ ] [ str "Type" ] ]
            Level.item [] [ 
              Select.select [ ]
                  [ select [ Value (valueOfTaxonomyType param.taxonomyType)
                             OnChange (fun ev -> CriteriaChanged { param with taxonomyType = taxonomyTypeOfValue ev.Value } |> dispatch) ]
                      [ 
                        option [ Value (valueOfTaxonomyType All) ] [ strOfTaxonomyType All ]
                        option [ Value (valueOfTaxonomyType Category) ] [ strOfTaxonomyType Category ]
                        option [ Value (valueOfTaxonomyType Tag) ] [ strOfTaxonomyType Tag ]
                        option [ Value (valueOfTaxonomyType Series) ] [ strOfTaxonomyType Series ] 
                      ] 
                  ]
            ]
            Level.item [] [
              Button.button [ 
                Button.Color IsInfo
                Button.Props [OnClick (fun _ -> Reload |> dispatch)] ] [ str "再読み込み"]
            ]
          ]
          Level.right [] [
            Level.item [] [
              Button.button [ 
                Button.Color IsInfo
                Button.Props [OnClick (fun _ -> AddNew |> dispatch)] ] [ str "新規追加"]
            ]
          ] 
      ]

    let listRows = 
      match model.dataList with
      | None -> ofOption None
      | Some s -> s |> List.ofSeq |> List.map (fun i -> 
          let trClass = match model.currentRec, i with
                            | Some x, y -> if x.Id = y.Id then "is-selected" else ""
                            | _ -> ""
          let id = i.Id.ToString()

          tr [Key id; ClassName trClass;] 
            [ td [ ] [ Fa.i [ Fa.Solid.Pen
                              Fa.Props [ OnClick (fun _ -> Select i |> dispatch)] ]
                            [] ] 
              td [ ] [ str id ]
              td [ ] [ typeEnumToStr i.Type ]
              td [ ] [ str i.Name ] 
              td [ ] [ str i.UrlSlug ]
              td [ ] [ (optionToString >> str) i.Description ] ] ) |> ofList 

    let page = model.listCriteria.page

    Box.box' [] [
      criteria model.listCriteria

      Table.table [ Table.IsBordered
                    Table.IsNarrow
                    Table.IsStriped ]
            [ thead [ ]
                [ tr [ ]
                     [ th [ ] [ ]
                       th [ ] [ str "Id" ]
                       th [ ] [ str "Type" ]
                       th [ ] [ str "Name" ]
                       th [ ] [ str "UrlSlug" ] 
                       th [ ] [ str "Description" ] ] ]
              tbody [ ]
                [ listRows ] 
            ]
      ListPager.pager page PageChanged dispatch
    ]

/// <summary>
/// 詳細部
/// </summary>
let detail (record:BlogModels.Taxonomy option) dispatch =
  match record with
  | None -> ofOption None
  | Some record ->
      Box.box' [] [
        div [ ] [ 
          Field.div []
              [ Label.label [ ] [ str "Id" ]
                Control.div [ ]
                  [ Input.text [ Input.IsReadOnly true; Input.Value ( if record.Id < 0L then "" else record.Id.ToString()) ] ] ]
          Field.div [ ]
              [ Label.label [ ] [ str "Type" ]
                Control.div [ ]
                  [ Select.select [ ]
                      [ select [ Value (record.Type.ToString())
                                 OnChange (fun ev -> RecordChanged { record with Type = typeEnumOfSelectValue ev.Value } |> dispatch) ]
                          [ option [ Value (TaxonomyTypeEnum.Category.ToString()) ] [ typeEnumToStr TaxonomyTypeEnum.Category ]
                            option [ Value (TaxonomyTypeEnum.Tag.ToString()) ] [ typeEnumToStr TaxonomyTypeEnum.Tag ]
                            option [ Value (TaxonomyTypeEnum.Series.ToString()) ] [ typeEnumToStr TaxonomyTypeEnum.Series ] ] ] ] ]
          Field.div [ ]
              [ Label.label [ ] [ str "Name" ]
                Control.div [ ]
                  [ Input.text [ 
                      Input.Value record.Name
                      Input.Props[ OnChange (fun ev -> RecordChanged { record with Name = ev.Value } |> dispatch) ]
                    ] ] ]
          Field.div [ ]
              [ Label.label [ ] [ str "UrlSlug" ]
                Control.div [ ]
                  [ Input.text [ 
                      Input.Value record.UrlSlug 
                      Input.OnChange (fun ev -> RecordChanged { record with UrlSlug = ev.Value } |> dispatch)
                    ] ] ]
          Field.div [ ]
              [ Label.label [ ] [ str "Description" ]
                Control.div [ ]
                  [ Input.text [ 
                      (optionToString >> Input.Value) record.Description 
                      Input.OnChange (fun ev -> RecordChanged { record with Description = Some ev.Value } |> dispatch)
                    ] ] ]

          Level.level [] [
            Level.left [] []
            Level.right [] [
              Level.item [] [
                Button.button [ 
                  Button.Color IsPrimary
                  Button.OnClick (fun _ -> Save record |> dispatch) ]
                  [ str "保存"]
              ]
              Level.item [] [
                Button.button [ 
                  Button.Color IsDanger 
                  Button.OnClick (fun _ -> Remove record |> dispatch) ] 
                  [ str "削除"]
              ]
            ]
          ]          
        ]    
      ]


let root (model:Model) dispatch =

    div [] [
      Label.label [] [str "一覧"]
      listView model dispatch
      div [] [ detail model.currentRec dispatch ]
    ]

  