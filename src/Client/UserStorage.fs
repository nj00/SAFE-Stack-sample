module App.UserStorage

open Browser.WebStorage
open Thoth.Json

open Shared.Auth

[<Literal>]
let StorageKey = "user"

let load () : UserData option =
    // let userDecoder = Decode.Auto.generateDecoder<UserData>()
    // match localStorage.getItem userDecoder StorageKey with
    // | Ok user -> Some user
    // | Error _ -> None
    let json = localStorage.getItem StorageKey
    match Decode.Auto.fromString<UserData> json with
    | Ok user -> Some user
    | Error _ -> None


let save (user:UserData) =
    // BrowserLocalStorage.save StorageKey user
    let json = Encode.Auto.toString(0, user)
    localStorage.setItem(StorageKey, json)

let remove () =
    // BrowserLocalStorage.delete StorageKey
    localStorage.removeItem StorageKey

