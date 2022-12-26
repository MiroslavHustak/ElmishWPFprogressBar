namespace ElmishWPFx.Models

open System
open System.IO

open Errors
open DiscriminatedUnions

module ROP_Functions = 


    let tryWith f1 f2 x = //to x tady k nicemu neni, ale z duvodu jednotnosti ponechano (vede to mj. k pouziti partial application)
        try
            try          
               f1 x |> Success
            finally
               f2 x
        with
        | ex -> Failure ex.Message  

    let deconstructor1 =  
        function
        | Success x  -> x, String.Empty                                                   
        | Failure ex -> List.empty, ex                   
                    
    let deconstructor2 =  
        function
        | Success x  -> x                                                   
        | Failure ex -> (ex |> error2).errMsg2  

    let deconstructor3 y =  
        function
        | Success x  -> x                                                   
        | Failure ex -> error6 <| ex
                        y
                    
    let optionToArraySort str1 str2 x = 
        match x with
        | Some value -> Array.sort (Array.ofSeq (value)) 
        | None       -> error3 str1 str2      

    let optionToGenerics str1 str2 x = 
        match x with 
        | Some value -> value 
        | None       -> error3 str1 str2 |> Array.head

    let optionToDirectoryInfo str (x: DirectoryInfo option) = 
        match x with 
        | Some value -> value
        | None       -> error4 str //ukonci program
                        new DirectoryInfo(String.Empty) //whatever of DirectoryInfo type
                                      
                    



