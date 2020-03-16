module Janken.Types

type Model = {
  Win: int
  Lost: int
  Result: string
}

type Msg =
  | Guu
  | Choki
  | Paa
