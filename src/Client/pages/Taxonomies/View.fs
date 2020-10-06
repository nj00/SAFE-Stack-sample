module Taxonomies.View

open Fable.React
open Fable.React.Props
open Fable.FontAwesome

open Fulma

open ViewComponent
open Types
open Shared
open Shared.BlogModels

let optionToString = function
| Some x -> string x
| None -> ""

let typeEnumToString = function
  | TaxonomyTypeEnum.Category -> "Category"
  | TaxonomyTypeEnum.Tag -> "Tag"
  | TaxonomyTypeEnum.Series -> "Series"
  | _ -> ""
let stringToTypeEnum = function
  | "Category" -> TaxonomyTypeEnum.Category
  | "Tag" -> TaxonomyTypeEnum.Tag
  | "Series" -> TaxonomyTypeEnum.Series
  | _ -> TaxonomyTypeEnum.Category

open Feliz
open Feliz.Bulma

/// <summary>
/// 一覧部分
/// </summary>
let listView (model:Model) dispatch =
  let criteria (param:ListCriteria) =
    let valueOfTaxonomyType = function
      | All -> "All"
      | Category -> "Category"
      | Tag -> "Tag"
      | Series -> "Series"

    let taxonomyTypeOfValue = function
      | "All" -> All
      | "Category" -> Category
      | "Tag" -> Tag
      | "Series" -> Series
      | _ -> All

    Bulma.level [
      Bulma.levelLeft [
        Bulma.levelItem [ Bulma.label [prop.text "Type"] ]
        Bulma.levelItem [ 
          Bulma.select [
            prop.value (valueOfTaxonomyType param.TaxonomyType)
            prop.onChange ((fun value -> CriteriaChanged { param with TaxonomyType = taxonomyTypeOfValue value }) >> dispatch)
            prop.children [
              Html.option [prop.text (valueOfTaxonomyType All)]
              Html.option [prop.text (valueOfTaxonomyType Category)]
              Html.option [prop.text (valueOfTaxonomyType Tag)]
              Html.option [prop.text (valueOfTaxonomyType Series)]
            ]
          ]
        ]
        Bulma.levelItem [
          Bulma.button.button [
            color.isInfo
            prop.onClick (fun _ -> Reload |> dispatch)
            prop.text "再読み込み"
          ]
        ]
      ]
      Bulma.levelRight [
        Bulma.levelItem [
          Bulma.button.button [
            color.isInfo
            prop.onClick (fun _ -> AddNew |> dispatch)
            prop.text "新規作成"
          ]
        ]
      ]
    ]

  let listRow taxonomy = 
    let trClass = match model.CurrentRec, taxonomy with
                      | Some x, y -> if x.Id = y.Id then "is-selected" else ""
                      | _ -> ""
    let id = taxonomy.Id.ToString()
    Html.tr [ 
      prop.key id
      prop.className trClass
      prop.children [
        Html.td [
          Fa.i [ Fa.Solid.Pen
                 Fa.Props [ OnClick (fun _ -> Select taxonomy |> dispatch)] ] []
        ]
        Html.td [ prop.text id]
        Html.td [ prop.text (typeEnumToString taxonomy.Type)]
        Html.td [ prop.text taxonomy.Name]
        Html.td [ prop.text taxonomy.UrlSlug]
        Html.td [ (optionToString >> prop.text) taxonomy.Description]
      ]
    ]

  let listRows = 
    match model.DataList with
    | None -> Html.none
    | Some s -> s |> List.ofSeq |> List.map listRow |> ofList 

  let page = model.ListCriteria.Page

  Bulma.box [
    criteria model.ListCriteria

    Bulma.table [ 
      table.isBordered; table.isNarrow; table.isStriped;
      prop.children [
        Html.thead [
          Html.tr [
            Html.th [ ]
            Html.th [ prop.text "Id"]
            Html.th [ prop.text "Type"]
            Html.th [ prop.text "Name"]
            Html.th [ prop.text "UrlSlug"]
            Html.th [ prop.text "Description"]
          ]
        ]
        Html.tbody [
          listRows
        ]
      ]
    ]

    ListPager.pager page PageChanged dispatch
  ]

/// <summary>
/// 詳細部
/// </summary>
let detail (record:BlogModels.Taxonomy option) dispatch =
  match record with
  | None -> Html.none
  | Some record ->
      Bulma.box [
        Bulma.field.div [
          Bulma.label [prop.text "Id"]
          Bulma.control.div [
            Bulma.input.text [
              prop.disabled true
              prop.value ( if record.Id < 0L then "" else record.Id.ToString())
            ]
          ]
        ]
        Bulma.field.div [
          Bulma.label [prop.text "Type"]
          Bulma.control.div [
            Bulma.select [
              prop.value (typeEnumToString record.Type)
              prop.onChange ((fun value -> RecordChanged { record with Type = stringToTypeEnum value }) >> dispatch)
              prop.children [
                Html.option [prop.text (typeEnumToString TaxonomyTypeEnum.Category)]
                Html.option [prop.text (typeEnumToString TaxonomyTypeEnum.Tag)]
                Html.option [prop.text (typeEnumToString TaxonomyTypeEnum.Series)]
              ]
            ]
          ]
        ]
        Bulma.field.div [
          Bulma.label [prop.text "Name"]
          Bulma.control.div [
            Bulma.input.text [
              prop.value record.Name
              prop.onChange ((fun value -> RecordChanged { record with Name = value }) >> dispatch)
            ]
          ]
        ]
        Bulma.field.div [
          Bulma.label [prop.text "UrlSlug"]
          Bulma.control.div [
            Bulma.input.text [
              prop.value record.UrlSlug
              prop.onChange ((fun value -> RecordChanged { record with UrlSlug = value }) >> dispatch)
            ]
          ]
        ]
        Bulma.field.div [
          Bulma.label [prop.text "Description"]
          Bulma.control.div [
            Bulma.input.text [
              (optionToString >> prop.value) record.Description
              prop.onChange ((fun value -> RecordChanged { record with Description = Some value }) >> dispatch)
            ]
          ]
        ]

        // 編集ボタン
        Bulma.level [
          Bulma.levelLeft [
            Bulma.levelItem [
              Bulma.button.button [
                color.isInfo
                prop.onClick (fun _ -> UnSelect |> dispatch)
                prop.text "キャンセル"
              ]
            ]
          ]
          Bulma.levelRight [
            Bulma.levelItem [
              Bulma.button.button [
                color.isPrimary
                prop.onClick (fun _ -> Save record |> dispatch) 
                prop.text "保存"
              ]
            ]
            Bulma.levelItem [
              Bulma.button.button [
                color.isDanger
                prop.onClick (fun _ -> Remove record |> dispatch) 
                prop.text "削除"
              ]
            ]
          ]
        ]
      ]

/// <summary>
/// 全体
/// </summary>
let render (model:Model) dispatch =
  Html.div [
    Bulma.label [prop.text "一覧"]
    listView model dispatch
    Html.div [
      detail model.CurrentRec dispatch
    ]
  ]

  