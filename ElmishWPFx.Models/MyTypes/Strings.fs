module Strings

open System

let (|StringNonN|) s = 
    s 
    |> Option.ofObj 
    |> function 
        | Some value -> string value
        | None       -> String.Empty

//let private myString (StringNonN s) = s //to be implemented in the relevant module