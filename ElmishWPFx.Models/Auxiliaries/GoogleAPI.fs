namespace ElmishWPFx.Models

open GoogleSheets
open CheckingNetConn

module GoogleAPI = 

    //****************** auxiliary function definitions **********************

    let private checkForNetConn() = 

        Seq.initInfinite (fun _ -> NetConn.CheckForNetConn()) //DLL C# 
        |> Seq.takeWhile ((=) false) 
        |> Seq.iter      (fun _ -> ())  


    //****************** main function definitions **********************

    let writeIntoGoogleSheet getFullInterval getFullNumberOfFiles jsonFileName1 id sheetName columnStart rowStart = 
    
        let header = 
            [|
                "Pracovní značení"
                "Počet skenů"
            |]
    
        let rows =   
            [| 
                getFullInterval() 
                getFullNumberOfFiles() 
            |]

        do checkForNetConn() 

        do WritingToGoogleSheets.WriteToGoogleSheets(
            header,
            rows, 
            jsonFileName1, 
            id, 
            sheetName,
            columnStart, rowStart, true //true -> vysvetleni viz DLL C# 
        ) //DLL C#  

    let readingFromGoogleSheets jsonFileName columnStart rowStart columnEnd rowEnd firstRowIsHeaders id sheetName6 =
   
        do checkForNetConn()     

        ReadingFromGoogleSheets.ReadFromGoogleSheets(
            jsonFileName, 
            id, 
            sheetName6,
            columnStart, rowStart, columnEnd, rowEnd, firstRowIsHeaders
        ) |> Option.ofObj //DLL C# 
       
  