module Counter.Types

open Shared

type Model = Counter option

type Msg =
  | Increment
  | Decrement
  | Init of Result<Counter, exn>
