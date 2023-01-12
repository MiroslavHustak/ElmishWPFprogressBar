module Strings

open System

let private myOption s = 
    s 
    |> Option.ofObj 
    |> function 
        | Some value -> string value
        | None       -> String.Empty

let (|StringNonN|) s = myOption s 

//let private myString (StringNonN s) = s //to be implemented in the relevant module