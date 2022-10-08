module MainLogicLeft

open System
open System.IO;

open Errors
open Helpers
open Settings
open GoogleAPI
open Other_Records
open ROP_Functions
open Helpers.Deserialisation

open Serilog

//leva strana je priklad, jak uz to stylove nerobit s textboxem

//***************************** settings *******************************/
(*
let private jsonFileName = @"e:\C\Downloads\nomadic-charge-314614-fd817ff829fa.json" 
let private id = @"1G15Mn_A9EjIXiS0UODz7WVsmx6BoSJGYA4tA-MtoEC4" // je to soucast URL Google tabulky // zkusebni Google Sheet
*)

//Pokud je to vhodne, jsou pouzity plne funkce, aby to vzdy pocitalo znovu a byl vzdy aktualni stav

//Array.Parallel tady zvysilo rychlost, tasks ne

//**************************************************************************************************
//***************************** auxiliary function definitions *************************************

let private deserializeMe() = 

    let perform x = deserialize "json.xml" 
    tryWith perform (fun x -> ()) (fun ex -> failwith) |> deconstructor3 Common_Settings.Default 
               
let private str = "HH:mm:ss.fff" 

let private (++) a b c = a + b + c
let private (+++) _ = (++) 

let private getDirsAndFiles() =

    let deserialize = deserializeMe()
   
    let path = deserialize.path //napr. $@"o:\Litoměřice 6. část\"   
    
    //TODO porovnej toto....
    let subdirs = 
        let prefix = deserialize.prefix // jo, a uz jsem zjistil odborny termin: parameterless function
        let strLength = deserialize.exampleString.Length //8
        Directory.GetDirectories(path) 
        |> Option.ofObj
        |> optionToGenerics "adresářů" "Directory.GetDirectories()"
        |> Array.filter(fun (item: string) -> item.Contains prefix && item.Replace(path, String.Empty).Length = strLength)    
    
    //TODO .... s timto, a over chovani funkci filter a choose v danem pripade
    let subdirs = 
        let prefix = deserialize.prefix // jo, a uz jsem zjistil odborny termin: parameterless function
        let strLength = deserialize.exampleString.Length //8
        let condition (item: string) = item.Contains prefix && item.Replace(path, String.Empty).Length = strLength
        Directory.GetDirectories(path) 
        |> Option.ofObj
        |> optionToGenerics "adresářů" "Directory.GetDirectories()"
        |> Array.Parallel.choose(fun item ->
                                            match item with
                                            | item when condition item -> Some(item)
                                            | _                        -> None
                                ) 
    
    let dirNoFullPath = subdirs
                        |> Array.Parallel.map(fun item -> item.Replace(path, String.Empty))   

    let numberOfFiles = subdirs 
                        |> Array.Parallel.map(fun item ->
                                                        let arr = Directory.GetFiles(item) 
                                                                  |> Option.ofObj 
                                                                  |> optionToGenerics "souborů" "Directory.GetFiles()"
                                                        string <| arr.Length)        
    
    dirNoFullPath, numberOfFiles  //tady tasks nemely vliv na rychlost

let private parsingValues() =   

    let results =
        let perform x = fst (getDirsAndFiles())
        tryWith perform (fun x -> ()) (fun ex -> failwith) |> deconstructor1

    match results with 
    | [||], ex   -> (0, 0), ex
    | _          -> 
                  let deserialize = deserializeMe()
                  let prefix = deserialize.prefix   
                  let x = (fst results).[0] //.NET  
                  let y = (fst results).[(fst results).Length - 1] //.NET
                  let x = fst results |> Array.item 0 //pro srovnani funkcionalni pristup //ponechano Array (nedavam do listu, aby nebyl indexed access to a list)
                  let y = fst results |> Array.item ((fst results |> Array.length) - 1) //FP
                  let low  = Parsing.parseMe <| x.Replace(prefix, String.Empty)  
                  let high = Parsing.parseMe <| y.Replace(prefix, String.Empty)
                  (low, high), String.Empty  

let private getFullInterval() = 

    //srovnej toto....
    (*
    let fullInt =
        [|
          for i in low .. high -> sprintf "%s%s%s"   
                                  <| prefix
                                  <| MyString.GetString((suffixLength - String.length (i |> string)), "0") 
                                  <| string i 
        |]                         
    *)

    //... s timto
    let fullInt = 
        let deserialize = deserializeMe()
        let (low, high), _ = parsingValues()
        let prefix = deserialize.prefix 
        let strLength = deserialize.exampleString.Length //8
        let suffixLength = [ strLength; - prefix.Length ] |> List.fold (+) 0 //5

        [| low .. high |]   //kupodivu Array.mapi stejne rychle jako for, s Array.Parallel.mapi je to rychlejsi, nez for
        |> Array.Parallel.mapi
              (fun i item -> sprintf "%s%s%s"                            
                             <| prefix
                             <| MyString.GetString((suffixLength - String.length ((i + low) |> string)), "0") 
                             <| string (i + low)                                                                                                       
              ) 
    fullInt

let private getFullNumberOfFiles() = 

    let fullNumberOfFiles = //jadro leve casti programu
        let myFullInterval = getFullInterval() 
        let myPartialInterval = fst (getDirsAndFiles())    //stejny index
        let partialNumberOfFiles = snd (getDirsAndFiles()) //stejny index

        myFullInterval 
        |> Array.Parallel.mapi
              (fun i item ->                                           
                            let condition = myPartialInterval |> Array.contains item
                            let index = myPartialInterval 
                                        |> Array.tryFindIndex (fun item -> condition = true && item = (myFullInterval |> Array.item i)) // myFullInterval.[i]                        
                            match index with   
                            | Some value -> partialNumberOfFiles |> Array.item value // partialNumberOfFiles.[value]                                                                           
                            | None       -> String.Empty //neni treba error, oznameni sprintf "Složka %s momentálně nemá ..." plne postacuje
              )   
    fullNumberOfFiles

let private listOfLines x = 
                
    (getFullInterval(), getFullNumberOfFiles()) //funkce, aby to vzdy pocitalo znovu a byl vzdy aktualni stav
    ||> Array.map2
           (fun item1 item2 -> 
                              match item2 with   
                              | "" -> (item1 |> sprintf "Složka %s momentálně nemá stanovený počet skenů") + "\n"                                                            
                              | _  -> (item1 |> sprintf "Složka %s má počet skenů   %s" <| item2) + "\n"
           ) 

let private textBoxString x = 

    let (low, high), ex = parsingValues()
    let condition = (||) (high = 0) (low = 0)
    
    match condition with   
    | true  -> 
             let x = [| (ex |> error2).errMsg2; error1().errMsg1 |] 
             (String.concat <| String.Empty <| x) 
    | false ->
             x

//*****************************************************************************
//***************************** main functions ********************************
let textBoxString1() = 

    let x = 
        let processStart = String.Empty //$"Začátek procesu: {DateTime.Now.ToString(str)}\n" //String.Empty //
        Log.Information($"Začátek procesu: {DateTime.Now.ToString(str)}\n")

        let results = tryWith listOfLines (fun x -> ()) (fun ex -> failwith) |> deconstructor1         
        let processEnd = String.Empty //$"Konec procesu: {DateTime.Now.ToString(str)}\n" //String.Empty //
        
        match snd results with
        | ""    -> (+++) 
                   <| processStart
                   <| processEnd 
                   <| "Co jsem zjistil....\n" 
                   <| (String.concat <| String.Empty <| fst results) //parametry : rozdelovac, array obsahujici lines pro spojeni
        | value -> let x = (value |> error2).errMsg2
                   x       
    textBoxString x         

let textBoxString2() = 
   
    let perform x =  
        let processStart = String.Empty //$"Začátek procesu: {DateTime.Now.ToString(str)}\n" //String.Empty // 
        let processEnd = String.Empty //$"Konec procesu: {DateTime.Now.ToString(str)} \n" //String.Empty //
        
        let partialFn1 = writeIntoGoogleSheet <| getFullInterval <| getFullNumberOfFiles  //tady tasks zpusobilo zpomaleni          
        let partialFn2() = 
            let deserialize = deserializeMe()
            partialFn1 <| deserialize.jsonFileName1 <| deserialize.id <| deserialize.sheetName <| deserialize.columnStart <| deserialize.rowStart 
        partialFn2()                     
        
        (++)        
        <| processStart
        <| processEnd
        <| "Údaje byly úspěšně zapsány. Ještě před kontrolou skenů je zkopíruj do příslušného sloupce v barevné tabulce."
       
    let result = tryWith perform (fun x -> ()) (fun ex -> failwith) |> deconstructor2  
       
    textBoxString result  