module Pages

open Elmish.UrlParser

[<RequireQualifiedAccess>]
type Page =
  | Home
  | Counter
  | Janken
  | Taxonomies

let toPageUrl page =
  match page with
  | Page.Home -> "#home"
  | Page.Counter -> "#counter"
  | Page.Janken -> "#janken"
  | Page.Taxonomies -> "#taxonomies"

let pageParser: Parser<Page->Page,_> =
    oneOf [
        map Page.Home (s "home")
        map Page.Counter (s "counter")
        map Page.Janken (s "janken")
        map Page.Taxonomies (s "taxonomies")
    ]
// let urlParser location = parsePath pageParser location
let urlParser location = (parseHash pageParser) location
