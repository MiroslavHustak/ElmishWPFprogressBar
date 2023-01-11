module Strings

open System

let private myOption (s: string) (fn: string) = 
    s 
    |> Option.ofObj 
    |> function 
        | Some value -> value
        | None       -> fn //some action

type StringNonN (s: string) = member this.StringNonN = myOption s      

let (|StringNonN|) s = myOption s

