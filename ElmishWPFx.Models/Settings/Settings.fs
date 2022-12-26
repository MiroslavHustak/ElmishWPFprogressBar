namespace ElmishWPFx.Models

module Settings =
    
    [<Struct>]  //vhodne pro 16 bytes => 4096 characters
    type Common_Settings = 
        {        
            //priste rozdelit na zpusob jaky mam v IrfanViewOpener - dle jednotlivych casti programu plus commonSettings pro spolecne hodnoty
            //***************************** settings
            prefix: string
            exampleString: string
            path: string
            numOfScansLength: int   
            firstRowIsHeaders: bool
            jsonFileName1: string
            jsonFileName2: string
            id: string
            sheetName: string 
            sheetName6: string 
            columnStart1: int //pracovni znaceni      
            columnStart2: int //numOfScans      
            columnStart: int //levy vypocet
            rowStart: int //levy vypocet 
        }
        static member Default = 
            {
                prefix = "LT-" 
                exampleString = "LT-01402"
                path = @"c:\Users\Martina\Kontroly skenu Litomerice\slozky pro kontrolu\"
                numOfScansLength = 4          
                firstRowIsHeaders = false
                jsonFileName1 = @"c:\Users\Martina\source\repos\ElmishWPFx\JSON\json1\ampacolitomerice-bca0f962c6f9.json" //u tasks nesmi byt stejny json pro excel a csv
                jsonFileName2 = @"c:\Users\Martina\source\repos\ElmishWPFx\JSON\json2\ampacolitomerice-bca0f962c6f9.json" //u tasks nesmi byt stejny json pro excel a csv
                id = @"1n429ukClsoDxiCzRrWcX9PeWegKARby0PeMSe9cR51o" // je to soucast URL Google tabulky // Ampaco Google Sheet
                sheetName = "Zaloha" 
                sheetName6 = "Kontrolni list" 
                columnStart1 = 1         
                columnStart2 = 14          
                columnStart = 1
                rowStart = 1 
            }

