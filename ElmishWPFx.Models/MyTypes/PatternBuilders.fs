namespace ElmishWPFx.Models

module PatternBuilders =
    (*
    let private (>>=) condition nextFunc =
        match condition with
        | false -> sprintf "Chyba - neexistující cesta k adresáři anebo chybné znaky v cestě"
        | true  -> nextFunc() 

    [<Struct>]
    type MyPatternBuilder = MyPatternBuilder with            
        member _.Bind(condition, nextFunc) = (>>=) <| condition <| nextFunc 
        member _.Return x = x
    *)
    let private (>>=) condition nextFunc =
        match fst condition with
        | false -> snd condition
        | true  -> nextFunc()  
    
    [<Struct>]
    type MyPatternBuilder = MyPatternBuilder with    
        member _.Bind(condition, nextFunc) = (>>=) <| condition <| nextFunc
        member _.Return x = x