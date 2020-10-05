module App.View

open Fable.React
open Fable.React.Props

open Feliz
open Feliz.Bulma

open Shared
open Types
open Pages

let navBrand (userName:string) dispatch =
    Bulma.navbar [
        color.isWhite
        prop.children [
            Bulma.container [
                Bulma.navbarBrand.div [
                    Bulma.navbarItem.a [
                        prop.classes ["has-text-weight-bold"; "is-size-4"]
                        prop.text "猫に.NET"
                    ]
                    Bulma.navbarBurger [ Html.span []; Html.span []; Html.span [] ]
                ]
                Bulma.navbarEnd.div [
                    Bulma.navbarItem.div [
                        prop.text userName
                    ]
                    Bulma.navbarItem.div [
                        Bulma.button.button [
                            color.isSuccess
                            prop.text "Logout"
                            prop.onClick (fun _ -> Logout |> dispatch )
                        ]
                    ]
                ]
            ]
        ]
    ]

let menuItem (label:string) page currentPage =
    Bulma.Bulma.menuItem.a [
        prop.className [if page = currentPage then "is-active"]
        prop.href (toPageUrl page)
        prop.onClick ViewHelper.goToUrl
        prop.text label
    ]

let menu currentPage =
    Bulma.menu [
        Bulma.menuLabel [prop.text "Page Sample"]
        Bulma.menuList [
            menuItem "Home" Page.Home currentPage
            menuItem "Counter" Page.Counter currentPage
            menuItem "Janken" Page.Janken currentPage
        ]

        Bulma.menuLabel [prop.text "Maintenance"]
        Bulma.menuList [
            menuItem "Taxonomies" Page.Taxonomies currentPage
        ]
    ]

let view (model : Model) (dispatch : Msg -> unit) =
    let pageHtml =
        function
        | HomeModel m -> Home.View.render m (HomeMsg >> dispatch)
        | CounterModel m -> Counter.View.root m (CounterMsg >> dispatch)
        | JankenModel m -> Janken.View.render m (JankenMsg >> dispatch)
        | TaxonomiesModel m -> Taxonomies.View.render m (TaxonomiesMsg >> dispatch)        

    let userName = 
        match model.LoginModel.State with
        | LoginForm.LoggedIn user -> sprintf "UserName: %s" user.UserName
        | LoginForm.LoggedOut -> ""

    Html.div [
        navBrand userName dispatch
        Bulma.container [
            Bulma.columns [
                Bulma.column [
                    column.is3
                    prop.children [menu model.CurrentPage]
                ]
                Bulma.column [
                    column.is9
                    prop.children [pageHtml model.PageModel]
                ]
            ]
        ]
        LoginForm.view model.LoginModel (LoginMsg >> dispatch)
    ]

