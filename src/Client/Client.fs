module Client

open Elmish
open Elmish.Navigation
open Elmish.React
open App.View
open App.State
open Thoth.Elmish.Toast

#if DEBUG
open Elmish.Debug
#endif

// Elmish.Program.runをシャドーイングするため、最後に付ける。デフォルトで DEBUGシンボル時のみらしいので#ifディレクティブは不要
open Elmish.HMR 

Program.mkProgram init update view
|> Program.toNavigable Pages.urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withToast ViewHelper.renderToastWithFulma
// |> Program.withReact "elmish-app"    // 日本語入力が出来ない。以下を使用する[https://github.com/elmish/react/issues/12]
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger // Chromeに Redux DevTools extension[https://github.com/zalmoxisus/redux-devtools-extension]を導入すること https://github.com/elmish/templates/issues/36 
#endif
|> Program.run
