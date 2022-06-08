module Helpers

open System
open System.IO;
open Newtonsoft.Json

open Errors
open Settings
open ROP_Functions
open System.Runtime.Serialization

    module private TryParserInt =

         let tryParseWith (tryParseFunc: string -> bool * _) = tryParseFunc >> function
             | true, value -> Some value
             | false, _    -> None
         let parseInt    = tryParseWith <| System.Int32.TryParse  
         let (|Int|_|)   = parseInt        

    module Parsing =

         let f x = let isANumber = x                                          
                   isANumber   
                   
         let rec parseMe = 
             function            
             | TryParserInt.Int i -> f i
             | _                  -> 0  

         let rec parseMeOption = 
             function            
             | TryParserInt.Int i -> f Some i
             | _                  -> None   
    
    module MyString = 

        //priklad pouziti: GetString(8, "0")//tuple a nazev velkym kvuli DLL pro C#
        let GetString (numberOfStrings: int, stringToAdd: string): string =   

            let initialString = String.Empty                //initial value of the string
            let listRange = [ 1 .. numberOfStrings ]
            let rec loop list auxString stringToAdd =
                match list with 
                | []        -> auxString
                | _ :: tail -> let finalString = (+) auxString stringToAdd
                               loop <| tail <| finalString <| stringToAdd
            loop <| listRange <| initialString <| stringToAdd  

    module SimulatingLongRunningOperation = 

        let longRunOper minValue maxValue =  

            let listRange = [ minValue .. maxValue ]
            let rec loop list   =
                match list with 
                | []        -> ()
                | _ :: tail ->                                 
                                //nejaky kod                                      
                                loop <| tail 
            loop <| listRange  
        
    module Serialisation = 

         let serialize record = 
            
             let filepath = Path.GetFullPath("json.xml") 
                            |> Option.ofObj 
                            |> optionToGenerics "čtení cesty k souboru json.xml" "Path.GetFullPath()"

             let xmlSerializer = new DataContractSerializer(typedefof<string>)          
                                 |> Option.ofObj 
                                 |> optionToGenerics "při tvorbě nové instance" "DataContractSerializer()"
             use stream = File.Create(filepath)   
             xmlSerializer.WriteObject(stream, JsonConvert.SerializeObject(record))            

    module Deserialisation =       
              
       let deserialize xmlFile = 
           
           let filepath = Path.GetFullPath(xmlFile) 
                          |> Option.ofObj 
                          |> optionToGenerics (sprintf "čtení cesty k souboru souboru %s" xmlFile) "Path.GetFullPath()"
          
           let jsonString() = 

               let xmlSerializer = new DataContractSerializer(typedefof<string>) 
                                   |> Option.ofObj 
                                   |> optionToGenerics "při tvorbě nové instance" "DataContractSerializer()"
               let fileStream = File.ReadAllBytes(filepath)
                                |> Option.ofObj 
                                |> optionToGenerics (sprintf "čtení dat ze souboru %s" xmlFile) "File.ReadAllBytes()"
               use memoryStream = new MemoryStream(fileStream) 
               let resultObj = xmlSerializer.ReadObject(memoryStream)  
                               |> Option.ofObj 
                               |> optionToGenerics (sprintf "čtení dat ze souboru %s" xmlFile) "xmlSerializer.ReadObject()"      
               let resultString = unbox resultObj  
                                  |> Option.ofObj 
                                  |> optionToGenerics "downcasting" "(unbox resultObj)"      
               let jsonString = JsonConvert.DeserializeObject<Common_Settings>(resultString) 
               jsonString
           
           //priste System.IO.File - viz poznamka uplne dole
           let fInfodat: FileInfo = new FileInfo(filepath)  
           match fInfodat.Exists with 
           | true  -> jsonString()              
           | false -> failwith (sprintf "Soubor %s nenalezen" xmlFile) //Common_Settings.Default   

    module CopyingFiles =     
        
        let copyFiles source destination =
            try
                try  
                    let sourceFilepath = Path.GetFullPath(source) 
                                         |> Option.ofObj 
                                         |> optionToGenerics (sprintf "čtení cesty k souboru %s" source)  "Path.GetFullPath()"

                    let destinFilepath = Path.GetFullPath(destination) 
                                         |> Option.ofObj 
                                         |> optionToGenerics (sprintf "čtení cesty k souboru %s" destination)  "Path.GetFullPath()"
                    
                    let fInfodat: FileInfo = new FileInfo(sourceFilepath)  
                    match fInfodat.Exists with 
                    | true  -> File.Copy(sourceFilepath, destinFilepath, true)             
                    | false -> failwith (sprintf "Soubor %s nenalezen" source)
                
                finally
                    () 
            with
            | _ as ex -> error4 ex.Message
                      
    (*       
    System.IO.File provides static members related to working with files, whereas System.IO.FileInfo represents a specific file and contains non-static members for working with that file.          
    Because all File methods are static, it might be more efficient to use a File method rather than a corresponding FileInfo instance method if you want to perform only one action. All File methods 
    require the path to the file that you are manipulating.    
    The static methods of the File class perform security checks on all methods. If you are going to reuse an object several times, consider using the corresponding 
    instance method of FileInfo instead, because the security check will not always be necessary.    
    *) 
        
    
        
       
          

