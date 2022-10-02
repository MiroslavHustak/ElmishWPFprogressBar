module Errors

open Other_Records

open System
open System.Windows
open System.Diagnostics

//************************** auxiliary function *******************

let private restartApp message f = 
    
    let title = "Závažná chyba"

    MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning)    
    |> function
       | MessageBoxResult.Yes -> let currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName
                                 Process.Start(currentExecutablePath) |> ignore
                                 Environment.Exit(1)  
                                 f
       | _                    -> f

//************************** main error functions ******************

let error0 ex =

    let message = sprintf "Vyskytla se chyba v nastavení. Klikni na \"Ano\" pro restart programu nebo na \"Ne\" pro pokračování s dosazenými prázdnými hodnotami (nedoporučeno). A oprav hodnoty v nastavení. K tomu dopomáhej ti následující chybové hlášení: \n\n%s"
                   <| ex
    restartApp message Array.empty

let error1() = 
    
    let myErrMsgRecord = { Err.myErrMsgDefaultRecord with ErrMsgRecord.errMsg1 = "Nastal problém při parsování, pravděpodobně je příslušný adresář prázdný, případně může být chybná hodnota v nastavení. \n" }
    myErrMsgRecord

let error2 ex =

    let myErrMsgRecord = 
        //let str = ex |> sprintf "Error2 - popis problému: %s Mohou se vyskytovat chybné hodnoty v nastavení.\n"  
        let str = sprintf "Error2. Mohou se vyskytovat chybné hodnoty v nastavení.\n"  
        { Err.myErrMsgDefaultRecord with ErrMsgRecord.errMsg2 = str }
    myErrMsgRecord

let error3 str1 str2 = 

    let message = sprintf "Vyskytla se chyba při čtení %s metodou %s, která není způsobena tímto programem. Oprav I/O problém v PC a klikni na \"Ano\" pro restart programu nebo na \"Ne\" pro pokračování s dosazenými prázdnými hodnotami (nedoporučeno)."
                  <| str1 <| str2
    restartApp message Array.empty                        

let error4 str  = 

    let message = sprintf "Vyskytla se chyba (%s). Klikni na \"OK\" pro ukončení této aplikace a sežeň programátora, anebo oprav problém sám."
                  <| str
    let title = "Závažná chyba"

    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning)   
    |> function  
       | MessageBoxResult.OK -> Environment.Exit(1)                     
       | _                   -> ()

let error5 ex  = 

    let message = sprintf "Vyskytla se chyba při načítání nastavených hodnot. Klikni na \"Ano\" pro restart programu nebo na \"Ne\" pro pokračování s defaultními hodnotami (nedoporučeno). K tomu dopomáhej ti následující chybové hlášení: \n\n%s"
                  <| ex               
    restartApp message ()

let error6 ex  = 

    let message = sprintf "Vyskytla se chyba při deserializaci. Klikni na \"Ano\" pro restart programu nebo na \"Ne\" pro pokračování s defaultními hodnotami (nedoporučeno). K tomu dopomáhej ti následující chybové hlášení: \n\n%s"
                  <| ex               
    restartApp message ()