namespace ElmishWPFx.Models

open System
open System.IO
open System.Text.RegularExpressions
open FSharp.Quotations.Evaluator.QuotationEvaluationExtensions

open Errors
open Helpers
open Settings
open GoogleAPI
open ROP_Functions
open PatternBuilders
open DiscriminatedUnions
open Helpers.Deserialisation

module MainLogicRight = 

    //Pokud je to vhodne, jsou pouzity plne funkce, aby to vzdy pocitalo znovu a byl vzdy aktualni stav
    //Tasks v rowStart a Array.Parallel 2x zrychlily program oproti predchozimu stavu (tasks pouze u myNumbers a myArray)

    //************************************************************************************************
    // FOR TESTING PURPOSES ONLY
    let workToDoRight reportProgress = 
       async 
           {    
               [ 1..100 ]    
               |> List.mapi(fun i item ->  //simulating long running operation
                                        [ 1..1_000_000 ] |> List.reduce (*) |> ignore 
                                        reportProgress i 
                           ) |> ignore    
               return 42
           }

    //***************************** auxiliary function definitions ***********************************
    let private stringChoice x y = MyString.getString((x - String.length (string y)), "0")

    let private (&&&&) a b c d e  = a && b && c && d && e 

    let private myTasks task1 task2 =           
        [ 
            task1 
            task2 
        ] 
        |> Async.Parallel //viz strana 433 Isaac Abraham
        |> Async.Catch
        |> Async.RunSynchronously
        |> function
            | Choice1Of2 result    -> result |> List.ofArray
            | Choice2Of2 (ex: exn) -> error0 ex.Message       
                                                               
    let private whatIs(x: obj) =
        match x with
        | :? TaskResults as du -> du  //aby nedoslo k nerizene chybe behem runtime
        | _                    -> 
                                  error4 "error 4 - x :?> TaskResults"                              
                                  x :?> TaskResults

    //*******************************************************************************
    //***************************** main functions **********************************  

    let textBoxString3 low high path reportProgress = 

        let deserialize =              
            let perform x = deserialize "json.xml" 
            tryWith perform (fun x -> ()) (fun ex -> ()) |> deconstructor3 Common_Settings.Default         

        let firstRowIsHeaders = deserialize.firstRowIsHeaders  
        let numOfScansLength = deserialize.numOfScansLength
        let jsonFileName1 = deserialize.jsonFileName1 //u async workflows nebo tasks nesmi v jednom okamziku odkaz na stejny json 
        let jsonFileName2 = deserialize.jsonFileName2 //u async workflows nebo tasks nesmi v jednom okamziku odkaz na stejny json 
        let columnStart1 = deserialize.columnStart1
        let columnStart2 = deserialize.columnStart2
        let prefix = deserialize.prefix
        let id = deserialize.id
        let sheetName6 = deserialize.sheetName6
        
        let processStart = 
            let str = "HH:mm:ss.fff"   
            String.Empty //$"  Začátek procesu: {DateTime.Now.ToString(str)}" //String.Empty //

        // first submain function
        let myNumbers() = 
            let rowStart = Parsing.parseMe low
            let rowEnd = Parsing.parseMe high

            let numOfScans() =        
                let columnStart = columnStart2
                let columnEnd = columnStart2 //mam pouze 1 sloupec
                let result = readingFromGoogleSheets jsonFileName1 columnStart rowStart columnEnd rowEnd firstRowIsHeaders id sheetName6//pozor na jsonFileName, nesmi se otevirat ten samy soubor
                optionToGenerics 
                <| "dat z Google Sheets"
                <| "readingFromGoogleSheets"
                <| result          
        
            let folderNum() = 
                let columnStart = columnStart1
                let columnEnd = columnStart1 //mam pouze 1 sloupec 
                let result = readingFromGoogleSheets jsonFileName2 columnStart rowStart columnEnd rowEnd firstRowIsHeaders id sheetName6 //pozor na jsonFileName, nesmi se otevirat ten samy soubor
                optionToGenerics 
                <| "dat z Google Sheets"
                <| "readingFromGoogleSheets"
                <| result     

            match rowStart with
            | 0 -> [], [] //neni treba specialni error, zachyti se to u sprintf "Chyba:\nPočet složek v Google %s...... - viz nize   
            | _ -> let result = myTasks  
                                <| async { return numOfScans() |> List.ofArray } 
                                <| async { return folderNum () |> List.ofArray }                   
                   result |> List.head, result |> List.last 
    
        // second submain function
        
        let myList1() =  //redundant function - just testing an option without a mutable counter
            let myArray = 
                Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly)
                |> Option.ofObj
                |> optionToArraySort "adresářů" "Directory.EnumerateDirectories()" //sort je quli sitove pripojenemu zarizeni 
            [| 0 .. myArray.Length - 1 |]
            |> Array.map
                    (fun i -> myArray 
                              |> Array.collect
                                      (fun item ->                                              
                                                 reportProgress (i * 5)  
                                                 let arr = 
                                                     let p = [ prefix; "*" ] |> List.fold (+) String.Empty 
                                                     Directory.EnumerateDirectories(item, p) 
                                                     |> Option.ofObj
                                                     |> optionToArraySort "adresářů" "Directory.EnumerateDirectories()"
                                                     |> Array.Parallel.map (fun item -> 
                                                                                      let arr = Directory.EnumerateFiles(item, "*.jpg", SearchOption.TopDirectoryOnly)
                                                                                                |> Option.ofObj   
                                                                                                |> optionToArraySort "souborů" "Directory.EnumerateFiles()"   
                                                                                      arr.Length
                                                                            ) 
                                                 arr                    
                                      )  //tohle da list listu zaplneny stejnymi daty
                    ) |> Array.head |> List.ofArray 

        let myList2() =  
            let myArray = 
                Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly)
                |> Option.ofObj
                |> optionToArraySort "adresářů" "Directory.EnumerateDirectories()" //sort je quli sitove pripojenemu zarizeni 
            (myArray, [| 0 .. myArray.Length - 1 |])
            ||> Array.map2
                     (fun item1 item2 ->                            
                                        reportProgress (item2 * 5)  
                                        let arr = 
                                            let p = [ prefix; "*" ] |> List.fold (+) String.Empty 
                                            Directory.EnumerateDirectories(item1, p) 
                                            |> Option.ofObj
                                            |> optionToArraySort "adresářů" "Directory.EnumerateDirectories()"
                                            |> Array.Parallel.map (fun item -> 
                                                                             let arr =
                                                                                       Directory.EnumerateFiles(item, "*.jpg", SearchOption.TopDirectoryOnly)
                                                                                       |> Option.ofObj   
                                                                                       |> optionToArraySort "souborů" "Directory.EnumerateFiles()"   
                                                                             arr.Length
                                                                  ) 
                                        arr       
                     ) |> Array.concat |> List.ofArray        
                           
        //third submain function comprising myNumbers() and myArray()
        let textBoxString3 x =         
               
            let du: TaskResults list = myTasks 
                                       <| async { return TupleStringString (myNumbers()) } 
                                       <| async { return MyListInt (myList2()) }
       
            let (numOfScans, folderNum) = 
                //primy dynamic cast :?> TaskResults muze vest k chybe behem runtime
                du |> List.head |> whatIs             
                |> function 
                    | TupleStringString(numOfScans, folderNum) -> numOfScans, folderNum                                                    
                    | _                                        -> 
                                                                  error4 "error4 - TupleStringString"
                                                                  [], []        
   
            let myList = 
                du |> List.last |> whatIs  
                |> function 
                    | MyListInt value -> value                                           
                    | _               -> 
                                         error4 "error4 - MyListInt"
                                         [] 

            let processEnd = String.Empty //$"  Konec procesu: {DateTime.Now.ToString(str)}" //String.Empty //
        
            let condition = (&&&&) 
                            <| (=)  myList.Length folderNum.Length 
                            <| (=)  myList.Length numOfScans.Length 
                            <| (=)  folderNum.Length numOfScans.Length 
                            <| (<>) numOfScans [] 
                            <| (<>) folderNum []
                                      

            match condition with
            | true  -> 
                     let len = String.length deserialize.exampleString
                 
                     let msg =         
                         (folderNum, numOfScans, myList)
                         |||> List.map3
                                  (fun item1 item2 item3 ->
                                                          let cond = (&&) (String.length item1 = len) (item1.Contains prefix)   
                                                          let str = "Chybná složka"
                                                          let item1 = 
                                                              match cond with
                                                              | true  -> 
                                                                         sprintf "%s%s"
                                                                         <| item1
                                                                         <| MyString.getString((String.length str) - len - 1, "  ")
                                                              | false -> str 

                                                          let result =    
                                                              match Parsing.parseMeOption item2 with
                                                              | Some value -> 
                                                                            match value - item3 with
                                                                            | 0 -> 
                                                                                   let s = "OK" //%15s
                                                                                   let expr = <@ (sprintf "%s%13s%16s%15s%5s%5s") @> //TODO je mozna dalsi generalizace ? ponechat quotations : pouze sprintf    
                                                                                   s, expr                                           
                                                                            | _ -> 
                                                                                   let s = "Počet skenů je rozdílný" //%36s
                                                                                   let expr = <@ (sprintf "%s%13s%16s%36s%5s%5s") @>  
                                                                                   s, expr 
                                                              | None       -> 
                                                                            let s = " Chybně zvolený sloupec Google" //%36s
                                                                            let expr = <@ (sprintf "%s%13s%16s%36s%5s%5s") @>  
                                                                            s, expr                                                            
                                                     
                                                          let mySprintf = (snd result).Compile()

                                                          mySprintf                                                    
                                                          <| item1 
                                                          <| (stringChoice numOfScansLength item2) + item2 
                                                          <| (stringChoice numOfScansLength (string item3)) + (string item3) 
                                                          <| fst result 
                                                          <| processStart 
                                                          <| processEnd + "\n"                                                 
                                  ) 
                     let str = sprintf "%s%25s%15s" 
                               <| "Složka" 
                               <| "Google"
                               <| "Synology"
                 
                     sprintf "%s%s%s"
                     <| str
                     <| "\n"
                     <| (String.concat <| String.Empty <| msg)                             
            | false -> 
                     sprintf "Chyba:\nPočet složek v Google %s, počet složek na Synology buď %s nebo neustanoven, počet řádků s uvedeným počtem skenů %s (pravděpodobně chybí sloupec se zapsanými počty skenů)" 
                     <| string folderNum.Length 
                     <| string myList.Length 
                     <| string numOfScans.Length               
         
        //prozeneme third submain pres try-with block...
        let result() =
            let result = tryWith textBoxString3 (fun x -> ()) (fun ex -> ())  //fun x je pro finally - viz IrfanView Opener
            //pro nazornost jeste jednou (at tam jde videt partial application):
            let result = (textBoxString3, (fun x -> ()), (fun ex -> ())) |||> tryWith 
            result |> deconstructor2     
    
        //.. a zajistime nejakou tu validaci na uvod, at neni treba odchytavat moc vyjimek
        MyPatternBuilder    
            {   
              let background = sprintf "Chyba - neexistující cesta k adresáři anebo chybné znaky v cestě" 
              let! _ =             
                let left = 
                    System.IO.Path.GetInvalidPathChars() //The array returned from this method is not guaranteed to contain the complete set of characters that are invalid in file and directory names. 
                    |> Option.ofObj 
                    |> optionToGenerics "nepovolených znaků" "Path.GetInvalidPathChars()"     
                    |> Array.map(fun item -> path.Contains(string item)) //dostaneme pole hodnot bool
                    |> Array.contains true //tj. obsahuje nepovoleny znak 
                    //podminka nutna ....  (obsahuje ACSII znaky, ktere muj regex nezachyti) 
                    //...nikoliv vsak dostacujici (GetInvalidPathChars() nezachyti +, <> a vetsinu dalsich znaku z regexu)          
                let right = Regex.IsMatch(path, """^[a-z]:\\(?:[^\\/:*?"<>|\r\n]+\\)*[^\\/:*?"<>|\r\n]*$""")  //tento regex pattern nezachyti +
                let result = (not left) && right && (not (path.Contains "+"))
                result, background
          
              let dInfodatResult =
                  //Priste tady pouzij System.IO.Directory.Exists, tady je zbytecne vytvarena instance tridy (objekt), bo nema dalsi vyuziti, viz poznamka nize
                  let dInfodat: DirectoryInfo = new DirectoryInfo(path) 
                  let dInfodatOption x = dInfodat |> Option.ofObj      
                  let ropResults = tryWith dInfodatOption (fun x -> ()) (fun ex -> ())  
                  ropResults
                  |> deconstructor3 None 
                  |> optionToDirectoryInfo "dInfodat: DirectoryInfo"  

              let! _ = dInfodatResult.Exists, background

              let returnFunction = result()

              return returnFunction
            }
    
    let textBoxString4 x y =
    
        let x = Parsing.parseMe x
        let y = Parsing.parseMe y
  
        match x <= 0 || y <= 0 with
        | true  -> 0
        | false -> let z = [ x; -y ] |> List.fold (+) 0 //jen tak pro vyzkouseni si monoidu
                   z
       

    (*
    Directory

    Directory is a static class.
    This should be used when we want to perform one operation in the folder.
    As there is not any requirement to create object for Directory class, so not any overhead for using this.

    Directory Info Class

    DirectoryInfo is not a static class.
    If user is required to perform lot of operations on one directory like creation, deletion, file listing etc, then DirectoryInfo class should be used.
    A separate object is created for performing all directory related operations.
    It's effective if you are going to perform many operations on the folder because, once the object is created, 
    it has all the necessary information about the folder such as its creation time, last access time and attributes.
    All the members of the DirectoryInfo class are instance members.
    *)