(*
MIT License for software design in this source file

Copyright (c) 2021 Bent Tranberg

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*)

namespace ElmishWPFx.Models

open Microsoft.FSharp.Core
open System.Windows
open System.Windows.Controls.Primitives

module Settings =
    
    open Elmish
    open Elmish.WPF

    open Settings
    
    open Helpers
    open Helpers.Serialisation
    open Helpers.Deserialisation

    open System
    open System.Windows.Media
    open System.Windows.Input
    open System.Windows.Controls

    let inline xor a b = (a || b) && not (a && b) //zatim nevyuzito
    
    type Model =
        {
            PrefixTextBoxText: string   
            ExampleStringTextBoxText: string                 
            PathTextBoxText: string  
            NumOfScansLengthTextBoxText: string  
                      
            FirstRowIsHeadersCheckBoxIsChecked: bool  
            JsonFileName1TextBoxText: string  
            JsonFileName2TextBoxText: string  
            IdTextBoxText: string  
            
            SheetNameTextBoxText: string  
            SheetName6TextBoxText: string  
            ColumnStart1TextBoxText: string   
            ColumnStart2TextBoxText: string   
            ColumnStartTextBoxText: string  
            RowStartTextBoxText: string  

            InfoTextBoxText: string
            InfoTextBoxForeground: SolidColorBrush 
            
            SheetNameLabel: string     
            SheetName6Label: string   
            JsonFileName1Label: string   
            JsonFileName2Label: string   
            IdLabel: string   
            PrefixLabel: string   
            ExampleStringLabel: string   
            NumOfScansLengthLabel: string   
            ColumnStart1Label: string   
            ColumnStart2Label: string   
            PathLabel: string   
            ColumnStartLabel: string   
            RowStartLabel: string   
            FirstRowIsHeadersCheckBox: string  
        }

    let defaultValues message = 
        {
            PrefixTextBoxText = Common_Settings.Default.prefix
            ExampleStringTextBoxText = Common_Settings.Default.exampleString               
            PathTextBoxText = Common_Settings.Default.path
            NumOfScansLengthTextBoxText = string Common_Settings.Default.numOfScansLength
                          
            FirstRowIsHeadersCheckBoxIsChecked = Common_Settings.Default.firstRowIsHeaders
            JsonFileName1TextBoxText = Common_Settings.Default.jsonFileName1
            JsonFileName2TextBoxText = Common_Settings.Default.jsonFileName2 
            IdTextBoxText = Common_Settings.Default.id 
                
            SheetNameTextBoxText = Common_Settings.Default.sheetName
            SheetName6TextBoxText = Common_Settings.Default.sheetName6 
            ColumnStart1TextBoxText = string Common_Settings.Default.columnStart1 
            ColumnStart2TextBoxText = string Common_Settings.Default.columnStart2 
            ColumnStartTextBoxText = string Common_Settings.Default.columnStart 
            RowStartTextBoxText = string Common_Settings.Default.rowStart 

            InfoTextBoxText = message
            InfoTextBoxForeground = Brushes.Green 

            SheetNameLabel = "Název listu v Google Sheets - levý výpočet" 
            SheetName6Label = "Název listu v Google Sheets - pravý výpočet" 
            JsonFileName1Label = "Cesta k Json1" 
            JsonFileName2Label = "Cesta k Json2"
            IdLabel = "Id" 
            PrefixLabel = "Předpona (například LT-)" 
            ExampleStringLabel = "Příklad pracovního značení složky (například LT-01442)" 
            NumOfScansLengthLabel = "Maximální počet skenů ve složce (počet číslic)" 
            ColumnStart1Label = "Číslo sloupce s pracovním značením" 
            ColumnStart2Label = "Číslo sloupce s počtem skenů" 
            PathLabel = "Cesta ke složkám pro kontrolu" 
            ColumnStartLabel = "Začátek umístění sloupce - levý výpočet" 
            RowStartLabel = "Začátek umístění řádku - levý výpočet" 
            FirstRowIsHeadersCheckBox = "Header"             
        }       

    let initialModel xmlFile message = 
        
        let deserializeWhenLoaded message = 
            
            let deserialize = deserialize xmlFile

            let myInitialModel = 
                {  
                    PrefixTextBoxText = deserialize.prefix
                    ExampleStringTextBoxText = deserialize.exampleString               
                    PathTextBoxText = deserialize.path
                    NumOfScansLengthTextBoxText = string deserialize.numOfScansLength
                          
                    FirstRowIsHeadersCheckBoxIsChecked = deserialize.firstRowIsHeaders
                    JsonFileName1TextBoxText = deserialize.jsonFileName1
                    JsonFileName2TextBoxText = deserialize.jsonFileName2 
                    IdTextBoxText = deserialize.id 
                
                    SheetNameTextBoxText = deserialize.sheetName
                    SheetName6TextBoxText = deserialize.sheetName6 
                    ColumnStart1TextBoxText = string deserialize.columnStart1 
                    ColumnStart2TextBoxText = string deserialize.columnStart2 
                    ColumnStartTextBoxText = string deserialize.columnStart 
                    RowStartTextBoxText = string deserialize.rowStart 

                    InfoTextBoxText = message
                    InfoTextBoxForeground = Brushes.Blue

                    SheetNameLabel = "Název listu v Google Sheets - levý výpočet" 
                    SheetName6Label = "Název listu v Google Sheets - pravý výpočet" 
                    JsonFileName1Label = "Cesta k Json1" 
                    JsonFileName2Label = "Cesta k Json2"
                    IdLabel = "Id" 
                    PrefixLabel = "Předpona (například LT-)" 
                    ExampleStringLabel = "Příklad pracovního značení složky (například LT-01442)" 
                    NumOfScansLengthLabel = "Maximální počet skenů ve složce (počet číslic)" 
                    ColumnStart1Label = "Číslo sloupce s pracovním značením" 
                    ColumnStart2Label = "Číslo sloupce s počtem skenů" 
                    PathLabel = "Cesta ke složkám pro kontrolu" 
                    ColumnStartLabel = "Začátek umístění sloupce - levý výpočet" 
                    RowStartLabel = "Začátek umístění řádku - levý výpočet" 
                    FirstRowIsHeadersCheckBox = "Header" 
                }             
            myInitialModel

        try
            try       
                deserializeWhenLoaded message 
            finally
                () 
        with
        | _ as ex ->                           
                    try
                        try
                            serialize Common_Settings.Default
                            defaultValues ((+) "Byly načteny defaultní hodnoty, jelikož se objevila následující chyba: " (string ex.Message))  
                        finally
                        () 
                    with
                    | _ as  ex -> defaultValues ((+) "Defaultní hodnoty neuloženy, jelikož se objevila následující chyba: " (string ex.Message))  
        
    let updateSettings m = 
           
           //pouze pro malo pravdepodobny pripad, kdyby nekdo v kodu nebo primo do json.xml/jsonBackup.xml zadal prazdne hodnoty
           let cond x y =             
               match String.IsNullOrWhiteSpace(string x) with
               | true  -> y     
               | false -> x
           
           //pouze pro malo pravdepodobny pripad, kdyby nekdo v kodu nebo primo do json.xml/jsonBackup.xml zadal hodnoty jineho typu, nez integer
           let condInt x y = 
               Parsing.parseMeOption (string x)
               |> function
                   | Some value -> value   
                   | None       -> y
            
           let myCopyOfSettings() =  //to je, co se ulozi
               {
                   prefix = cond m.PrefixTextBoxText Common_Settings.Default.prefix 
                   exampleString = cond m.ExampleStringTextBoxText Common_Settings.Default.exampleString 
                   path = cond m.PathTextBoxText Common_Settings.Default.path 
                   numOfScansLength = condInt m.NumOfScansLengthTextBoxText Common_Settings.Default.numOfScansLength 
       
                   firstRowIsHeaders = unbox m.FirstRowIsHeadersCheckBoxIsChecked
                   jsonFileName1 = cond m.JsonFileName1TextBoxText Common_Settings.Default.jsonFileName1 
                   jsonFileName2 = cond m.JsonFileName2TextBoxText Common_Settings.Default.jsonFileName2
                   id = cond m.IdTextBoxText Common_Settings.Default.id
       
                   sheetName = cond m.SheetNameTextBoxText Common_Settings.Default.sheetName
                   sheetName6 = cond m.SheetName6TextBoxText Common_Settings.Default.sheetName6
                   columnStart1 = condInt m.ColumnStart1TextBoxText Common_Settings.Default.columnStart1
                   columnStart2 = condInt m.ColumnStart2TextBoxText Common_Settings.Default.columnStart2
                   columnStart = condInt m.ColumnStartTextBoxText Common_Settings.Default.columnStart
                   rowStart = condInt m.RowStartTextBoxText Common_Settings.Default.rowStart
               }
           
           let myCopyOfModel() =  //to je, co se hned zobrazi
               {
                   PrefixTextBoxText = cond m.PrefixTextBoxText Common_Settings.Default.prefix 
                   ExampleStringTextBoxText = cond m.ExampleStringTextBoxText Common_Settings.Default.exampleString 
                   PathTextBoxText = cond m.PathTextBoxText Common_Settings.Default.path 
                   NumOfScansLengthTextBoxText = string (condInt m.NumOfScansLengthTextBoxText Common_Settings.Default.numOfScansLength)
                   
                   FirstRowIsHeadersCheckBoxIsChecked = unbox m.FirstRowIsHeadersCheckBoxIsChecked
                   JsonFileName1TextBoxText = cond m.JsonFileName1TextBoxText Common_Settings.Default.jsonFileName1 
                   JsonFileName2TextBoxText = cond m.JsonFileName2TextBoxText Common_Settings.Default.jsonFileName2
                   IdTextBoxText = cond m.IdTextBoxText Common_Settings.Default.id
       
                   SheetNameTextBoxText = cond m.SheetNameTextBoxText Common_Settings.Default.sheetName
                   SheetName6TextBoxText = cond m.SheetName6TextBoxText Common_Settings.Default.sheetName6
                   ColumnStart1TextBoxText = string (condInt m.ColumnStart1TextBoxText Common_Settings.Default.columnStart1)
                   ColumnStart2TextBoxText = string (condInt m.ColumnStart2TextBoxText Common_Settings.Default.columnStart2)
                   ColumnStartTextBoxText = string (condInt m.ColumnStartTextBoxText Common_Settings.Default.columnStart)
                   RowStartTextBoxText = string (condInt m.RowStartTextBoxText Common_Settings.Default.rowStart)
                   //RowStartTextBoxText = m.RowStartTextBoxText

                   InfoTextBoxText =  m.InfoTextBoxText
                   InfoTextBoxForeground = m.InfoTextBoxForeground

                   SheetNameLabel = m.SheetNameLabel 
                   SheetName6Label = m.SheetName6Label 
                   JsonFileName1Label = m.JsonFileName1Label 
                   JsonFileName2Label = m.JsonFileName2Label 
                   IdLabel = m.IdLabel 
                   PrefixLabel = m.PrefixLabel 
                   ExampleStringLabel = m.ExampleStringLabel 
                   NumOfScansLengthLabel = m.NumOfScansLengthLabel 
                   ColumnStart1Label = m.ColumnStart1Label 
                   ColumnStart2Label = m.ColumnStart2Label 
                   PathLabel = m.PathLabel 
                   ColumnStartLabel = m.ColumnStartLabel 
                   RowStartLabel = m.RowStartLabel 
                   FirstRowIsHeadersCheckBox = m.FirstRowIsHeadersCheckBox 
               }           
           
           let strEx (ex: exn) = (+) "Hodnoty neuloženy, neboť se objevila následující chyba: " (string ex.Message)
           try
              try 
                 serialize <| myCopyOfSettings()
                 initialModel "json.xml" |> ignore
                 {myCopyOfModel() with InfoTextBoxForeground = m.InfoTextBoxForeground; InfoTextBoxText = m.InfoTextBoxText}                
                finally
                 () 
           with
           | _ as ex -> {myCopyOfModel() with InfoTextBoxForeground = Brushes.Red; InfoTextBoxText = strEx ex} 
       
    let init(): Model * Cmd<'a> = initialModel "json.xml" "Hodnoty načteny. V případě prázdné kolonky je vždy dosazena defaultní hodnota.", Cmd.none 
        
    type Msg =
        | CancelButton2Event 
        //| SaveValuesEvent 
        | TextBoxClickedEvent
        | DefaultButton3Event 
        
        | PrefixTextBox of string
        | ExampleStringTextBox of string             
        | PathTextBox of string
        | NumOfScansLengthTextBox of string
                   
        | FirstRowIsHeadersCheckBox of bool
        | JsonFileName1TextBox of string
        | JsonFileName2TextBox of string
        | IdTextBox of string
         
        | SheetNameTextBox of string
        | SheetName6TextBox of string
        | ColumnStart1TextBox of string
        | ColumnStart2TextBox of string
        | ColumnStartTextBox of string
        | RowStartTextBox of string
        | InfoTextBoxForeground //of SolidColorBrush

    //*********************** cmdParam ********************************** v teto app nepouzivana cast
    //pouze navic parametry, pokud bychom potrebovali (viz GitHub), nic jineho navic to neprinasi oproti "TextBoxChanged" |> Binding.cmd SaveValuesEvent 
    //redundant code, for learning purposes only TextBoxClickedEvent
    let paramToTextBoxChanged (p: obj) =     
               
        let args = p :?> Button   
        //args.IsMouseOver baj tohle ziskame...
        //args.Width ... nebo tohle
        let e = args.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent))
        e    
        //SaveValuesEvent    

        let args = p :?> TextBox   
        //args.Text baj tohle ziskame...
        //args.FontSize ... nebo tohle
        let e = args.RaiseEvent(new RoutedEventArgs(TextBoxBase.TextChangedEvent))
        e    
        //SaveValuesEvent   // v tomto pripade paramToTextBoxChanged by pak robilo to same, jako "TextBoxChanged" |> Binding.cmd SaveValuesEvent   
    
    //*********************** cmdParam ********************************** v teto app pouzivana cast
    let castAs<'T when 'T : null> (o:obj) = 
        match o with
        | :? 'T as res -> res
        | _            -> null       
    
    let paramTextBoxClickedEvent (p: obj) =    
        
        let e = castAs<MouseButtonEventArgs>(p) 
        let sender = e.Source
        let textBox = castAs<TextBox>(sender) 
         
        let e =             
            match textBox.IsKeyboardFocusWithin with 
            | true  -> ()
            | false -> 
                      textBox.SelectAll()     
                      e.Handled <- true
                      textBox.Focus() |> ignore
        e  
        TextBoxClickedEvent
      
    // A command in Elmish is a function that can trigger events into the dispatch loop. // A command is essentially a function that takes a dispatch function as input and returns unit:
    let update (msg: Msg) (m: Model) : Model * Cmd<Msg> = 
                       
        let str = sprintf "Hodnota \"%s\" byla změněna (pokud nebyla zadaná prázdná hodnota nebo chybná číselná hodnota)."
        let strCbx = sprintf "Hodnota \"%s\" byla změněna."

        match msg with          
        | CancelButton2Event   -> initialModel "jsonBackUp.xml" "Načteny hodnoty ze záložního souboru (hodnoty uložené před spuštěním programu). \n" |> updateSettings, Cmd.none                                   
        // | SaveValuesEvent   -> { updateSettings m with InfoTextBoxForeground = m.InfoTextBoxForeground }, Cmd.none   
        | TextBoxClickedEvent  -> m, Cmd.none 
        | DefaultButton3Event  -> defaultValues "Načteny defaultní hodnoty." |> updateSettings, Cmd.none                                        
        | PrefixTextBox prefix -> { m with PrefixTextBoxText = prefix; InfoTextBoxText = str m.PrefixLabel; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none
        | ExampleStringTextBox exampleString       -> { m with ExampleStringTextBoxText = exampleString; InfoTextBoxText = str m.ExampleStringLabel; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none           
        | PathTextBox path     -> { m with PathTextBoxText = path; InfoTextBoxText = str m.PathLabel; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | NumOfScansLengthTextBox numOfScansLength -> { m with NumOfScansLengthTextBoxText = string numOfScansLength; InfoTextBoxText = str m.NumOfScansLengthLabel; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
                              
        | FirstRowIsHeadersCheckBox firstRowIsHeaders -> { m with FirstRowIsHeadersCheckBoxIsChecked = firstRowIsHeaders; InfoTextBoxText = strCbx m.FirstRowIsHeadersCheckBox; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | JsonFileName1TextBox jsonFileName1 -> { m with JsonFileName1TextBoxText = jsonFileName1; InfoTextBoxText = str m.JsonFileName1Label; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | JsonFileName2TextBox jsonFileName2 -> { m with JsonFileName2TextBoxText = jsonFileName2;InfoTextBoxText = str m.JsonFileName2Label; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | IdTextBox id -> { m with IdTextBoxText = id; InfoTextBoxText = str m.IdLabel; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
                    
        | SheetNameTextBox sheetName       -> { m with SheetNameTextBoxText = sheetName; InfoTextBoxText = str m.SheetNameLabel; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | SheetName6TextBox sheetName6     -> { m with SheetName6TextBoxText = sheetName6; InfoTextBoxText =  str m.SheetName6Label; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | ColumnStart1TextBox columnStart1 -> { m with ColumnStart1TextBoxText = string columnStart1; InfoTextBoxText = str m.ColumnStart1Label; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | ColumnStart2TextBox columnStart2 -> { m with ColumnStart2TextBoxText = string columnStart2; InfoTextBoxText = str m.ColumnStart2Label; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | ColumnStartTextBox columnStart   -> { m with ColumnStartTextBoxText = string columnStart; InfoTextBoxText = str m.ColumnStartLabel; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none    
        | RowStartTextBox rowStart         -> { m with RowStartTextBoxText = string rowStart; InfoTextBoxText = str m.RowStartLabel; InfoTextBoxForeground = Brushes.Red } |> updateSettings, Cmd.none   
        | InfoTextBoxForeground            -> { m with InfoTextBoxForeground = Brushes.Black }, Cmd.none //tohle je barva, na kterou se to po pohybu mysi nebo po zvednuti klavesy zmeni
             
    let cond x y =             
        match String.IsNullOrWhiteSpace(string x) with
        | true  -> y     
        | false -> x
    
    let condInt y x =   //musim x a y prehodit, nebot hodnota pres piping je dosazena az nakonec vpravo
        match Parsing.parseMeOption (string x) with
        | Some value -> string value   
        | None       -> string y       

    let condition x y = (cond x y) |> condInt y 

    let bindings(): Binding<Model,Msg> list =
        [ 
          "CancelButton2"           |> Binding.cmd CancelButton2Event  
          // "CheckBoxChanged"      |> Binding.cmd SaveValuesEvent   
          // "TextBoxChanged"       |> Binding.cmd SaveValuesEvent
          "TextBoxClicked"          |> Binding.cmdParam paramTextBoxClickedEvent  
          "DefaultButton3"          |> Binding.cmd DefaultButton3Event          
          "PrefixTextBox"           |> Binding.twoWay((fun m -> m.PrefixTextBoxText), (fun newVal m -> cond (string newVal) m.PrefixTextBoxText |> PrefixTextBox))
          "ExampleStringTextBox"    |> Binding.twoWay((fun m -> m.ExampleStringTextBoxText), (fun newVal m -> cond (string newVal) m.ExampleStringTextBoxText |> ExampleStringTextBox))              
          "PathTextBox"             |> Binding.twoWay((fun m -> m.PathTextBoxText), (fun newVal m -> cond (string newVal) m.PathTextBoxText |> PathTextBox))
          "NumOfScansLengthTextBox" |> Binding.twoWay((fun m -> m.NumOfScansLengthTextBoxText), (fun newVal m -> condition newVal m.NumOfScansLengthTextBoxText |> NumOfScansLengthTextBox))
                     
          "FirstRowIsHeadersCheckBox" |> Binding.twoWay((fun m -> m.FirstRowIsHeadersCheckBoxIsChecked), (fun newVal -> newVal |> FirstRowIsHeadersCheckBox))
          
          "JsonFileName1TextBox"   |> Binding.twoWay((fun m -> m.JsonFileName1TextBoxText), (fun newVal m -> cond (string newVal) m.JsonFileName1TextBoxText |> JsonFileName1TextBox))
          "JsonFileName2TextBox"   |> Binding.twoWay((fun m -> m.JsonFileName2TextBoxText), (fun newVal m -> cond (string newVal) m.JsonFileName2TextBoxText |> JsonFileName2TextBox))
          "IdTextBox"              |> Binding.twoWay((fun m -> m.IdTextBoxText), (fun newVal m -> cond (string newVal) m.IdTextBoxText |> IdTextBox))
           
          "SheetNameTextBox"       |> Binding.twoWay((fun m -> m.SheetNameTextBoxText), (fun newVal m -> cond (string newVal) m.SheetNameTextBoxText |> SheetNameTextBox))
          "SheetName6TextBox"      |> Binding.twoWay((fun m -> m.SheetName6TextBoxText), (fun newVal m -> cond (string newVal) m.SheetName6TextBoxText |> SheetName6TextBox))
          "ColumnStart1TextBox"    |> Binding.twoWay((fun m -> m.ColumnStart1TextBoxText), (fun newVal m -> condition newVal m.ColumnStart1TextBoxText |> ColumnStart1TextBox))
          "ColumnStart2TextBox"    |> Binding.twoWay((fun m -> m.ColumnStart2TextBoxText), (fun newVal m -> condition newVal m.ColumnStart2TextBoxText |> ColumnStart2TextBox))
          "ColumnStartTextBox"     |> Binding.twoWay((fun m -> m.ColumnStartTextBoxText), (fun newVal m -> condition newVal m.ColumnStartTextBoxText |> ColumnStartTextBox))
          "RowStartTextBox"        |> Binding.twoWay((fun m -> m.RowStartTextBoxText), (fun newVal m -> condition newVal m.RowStartTextBoxText |> RowStartTextBox))

          "TriggerEvent"           |> Binding.cmd InfoTextBoxForeground

          "InfoTextBox"            |> Binding.oneWay(fun m -> m.InfoTextBoxText)
          "InfoTextBoxForeground"  |> Binding.oneWay(fun m -> m.InfoTextBoxForeground)
          
          "PrefixLabel"            |> Binding.oneWay(fun m -> m.PrefixLabel) 
          "ExampleStringLabel"     |> Binding.oneWay(fun m -> m.ExampleStringLabel)            
          "PathLabel"              |> Binding.oneWay(fun m -> m.PathLabel)
          "NumOfScansLengthLabel"  |> Binding.oneWay(fun m -> m.NumOfScansLengthLabel)
                              
          "FirstRowIsHeadersCheckBoxContent" |> Binding.oneWay(fun m -> m.FirstRowIsHeadersCheckBox)
                   
          "JsonFileName1Label"     |> Binding.oneWay(fun m -> m.JsonFileName1Label)
          "JsonFileName2Label"     |> Binding.oneWay(fun m -> m.JsonFileName2Label)
          "IdLabel"                |> Binding.oneWay(fun m -> m.IdLabel)                    
          "SheetNameLabel"         |> Binding.oneWay(fun m -> m.SheetNameLabel)
          "SheetName6Label"        |> Binding.oneWay(fun m -> m.SheetName6Label)
          "ColumnStart1Label"      |> Binding.oneWay(fun m -> m.ColumnStart1Label)
          "ColumnStart2Label"      |> Binding.oneWay(fun m -> m.ColumnStart2Label)
          "ColumnStartLabel"       |> Binding.oneWay(fun m -> m.ColumnStartLabel)
          "RowStartLabel"          |> Binding.oneWay(fun m -> m.RowStartLabel)
        ]      

//Equivalent code in FsXaml        

(*
namespace FsXaml

open System
open System.Windows.Forms

open Settings

open Helpers
open Helpers.Serialisation
open Helpers.Deserialisation

type SettingsWindowXaml = FsXaml.XAML<"XAMLAndCodeBehind/SettingsWindow.xaml">

type SettingsWindow() as this =

    inherit SettingsWindowXaml()

    let updateSettings() = 

        let cond x y =             
            match String.IsNullOrWhiteSpace(string x) with
            | true  -> y     
            | false -> x
               
        let condInt x y = 
            let result = Parsing.parseMeOption (string x)
            match result with
            | Some value -> value   
            | None       -> y
        
        let myCopyOfSettings =  
            {
                prefix = cond (string this.prefixTextBox.Text) Common_Settings.Default.prefix 
                exampleString = cond (string this.exampleStringTextBox.Text) Common_Settings.Default.exampleString 
                path = cond (string this.pathTextBox.Text) Common_Settings.Default.path 
                numOfScansLength = condInt (string (this.numOfScansLengthTextBox.Text)) Common_Settings.Default.numOfScansLength 

                firstRowIsHeaders = unbox (this.firstRowIsHeadersCheckBox.IsChecked)
                jsonFileName1 = cond (string this.jsonFileName1TextBox.Text) Common_Settings.Default.jsonFileName1 
                jsonFileName2 = cond (string this.jsonFileName2TextBox.Text) Common_Settings.Default.jsonFileName2
                id = cond (string this.idTextBox.Text) Common_Settings.Default.id

                sheetName = cond (string this.sheetNameTextBox.Text) Common_Settings.Default.sheetName
                sheetName6 = cond (string this.sheetName6TextBox.Text) Common_Settings.Default.sheetName6
                columnStart1 = condInt (string (this.columnStart1TextBox.Text)) Common_Settings.Default.columnStart1
                columnStart2 = condInt (string (this.columnStart2TextBox.Text)) Common_Settings.Default.columnStart2
                columnStart = condInt (string (this.columnStartTextBox.Text)) Common_Settings.Default.columnStart
                rowStart = condInt (string (this.rowStartTextBox.Text)) Common_Settings.Default.rowStart
            }

        try
            try
                do serialize myCopyOfSettings   
                this.infoTextBox.Foreground <- Windows.Media.Brushes.Red                
                this.infoTextBox.Text <- "Hodnota aktualizována. V případě chyby při parsování je vždy dosazena defaultní hodnota. \n"
                //this.infoTextBox.AppendText("Hodnota aktualizována. V případě chyby při parsování je vždy dosazena defaultní hodnota. \n")
                //this.infoTextBox.TextChanged.Add (fun _ ->  ()  )                  
            finally
                () //zatim nepotrebne
        with
        | _ as  ex -> this.infoTextBox.Text <- (+) "Hodnoty neuloženy, neboť se objevila následující chyba: " (string ex.Message)

    let defaultValues() = 

        this.prefixTextBox.Text <- Common_Settings.Default.prefix
        this.exampleStringTextBox.Text <- Common_Settings.Default.exampleString               
        this.pathTextBox.Text <- Common_Settings.Default.path
        this.numOfScansLengthTextBox.Text <- string Common_Settings.Default.numOfScansLength
          
        this.firstRowIsHeadersCheckBox.IsChecked <- Common_Settings.Default.firstRowIsHeaders
        this.jsonFileName1TextBox.Text <- Common_Settings.Default.jsonFileName1
        this.jsonFileName2TextBox.Text <- Common_Settings.Default.jsonFileName2 
        this.idTextBox.Text <- Common_Settings.Default.id 

        this.sheetNameTextBox.Text <- Common_Settings.Default.sheetName
        this.sheetName6TextBox.Text <- Common_Settings.Default.sheetName6 
        this.columnStart1TextBox.Text <- string Common_Settings.Default.columnStart1 
        this.columnStart2TextBox.Text <- string Common_Settings.Default.columnStart2 
        this.columnStartTextBox.Text <- string Common_Settings.Default.columnStart 
        this.rowStartTextBox.Text <- string Common_Settings.Default.rowStart 

    let whenLoaded _ = 

        let deserializeWhenLoaded() = 

            let deserialize = deserialize()
            this.prefixTextBox.Text <- deserialize.prefix
            this.exampleStringTextBox.Text <- deserialize.exampleString               
            this.pathTextBox.Text <- deserialize.path
            this.numOfScansLengthTextBox.Text <- string deserialize.numOfScansLength
         
            this.firstRowIsHeadersCheckBox.IsChecked <- deserialize.firstRowIsHeaders
            this.jsonFileName1TextBox.Text <- deserialize.jsonFileName1
            this.jsonFileName2TextBox.Text <- deserialize.jsonFileName2 
            this.idTextBox.Text <- deserialize.id 

            this.sheetNameTextBox.Text <- deserialize.sheetName
            this.sheetName6TextBox.Text <- deserialize.sheetName6 
            this.columnStart1TextBox.Text <- string deserialize.columnStart1 
            this.columnStart2TextBox.Text <- string deserialize.columnStart2 
            this.columnStartTextBox.Text <- string deserialize.columnStart 
            this.rowStartTextBox.Text <- string deserialize.rowStart 
            
        try
            try
                do deserializeWhenLoaded()
                //this.infoTextBox.FontWeight <- Windows.FontWeight.FromOpenTypeWeight 700
                this.infoTextBox.Foreground <- Windows.Media.Brushes.Black
                this.infoTextBox.Text <- "Hodnoty v pořádku načteny. V případě prázdné kolonky je vždy dosazena defaultní hodnota." 
            finally
               () //zatim nepotrebne
        with
        | _ as ex ->                           
                    try
                        try
                            do serialize (Common_Settings.Default)
                        finally
                        () //zatim nepotrebne
                    with
                    | _ as  ex -> this.infoTextBox.Text <- (+) "Defaultní hodnoty neuloženy, jelikož se objevila následující chyba: " (string ex.Message)                                                  
                          
                    do defaultValues()
                    do updateSettings() 
                    this.infoTextBox.Text <- (+) "Byly načteny defaultní hodnoty, jelikož se objevila následující chyba: " (string ex.Message)
                        
    let whenClosing _ = 

        Application.Restart()
        Environment.Exit(0)
   
    let whenClosed _ = ()   
    
    let whenTextChanged _ = do updateSettings()    

    let whenMouseEnterTextBox _ =

        match this.IsMouseOver with
        | true  -> this.infoTextBox.Foreground <- Windows.Media.Brushes.Black
        | false -> ()

    let Button2Click _ =  this.Close()      

    let Button3Click _ =  
    
        do defaultValues()
        do updateSettings()  
        this.infoTextBox.Foreground <- Windows.Media.Brushes.Black
        this.infoTextBox.Text <- "Defaultní hodnoty v pořádku načteny a uloženy." 
        
    do  
        this.prefixTextBox.TextChanged.Add whenTextChanged
        this.prefixTextBox.MouseEnter.Add whenMouseEnterTextBox
        this.exampleStringTextBox.TextChanged.Add whenTextChanged 
        this.exampleStringTextBox.MouseEnter.Add whenMouseEnterTextBox
        this.pathTextBox.TextChanged.Add whenTextChanged
        this.pathTextBox.MouseEnter.Add whenMouseEnterTextBox
        this.numOfScansLengthTextBox.TextChanged.Add whenTextChanged
        this.numOfScansLengthTextBox.MouseEnter.Add whenMouseEnterTextBox
        this.jsonFileName1TextBox.TextChanged.Add whenTextChanged
        this.jsonFileName1TextBox.MouseEnter.Add whenMouseEnterTextBox
        this.jsonFileName2TextBox.TextChanged.Add whenTextChanged
        this.jsonFileName2TextBox.MouseEnter.Add whenMouseEnterTextBox
        this.idTextBox.TextChanged.Add whenTextChanged
        this.idTextBox.MouseEnter.Add whenMouseEnterTextBox
        this.sheetNameTextBox.TextChanged.Add whenTextChanged
        this.sheetNameTextBox.MouseEnter.Add whenMouseEnterTextBox
        this.sheetName6TextBox.TextChanged.Add whenTextChanged
        this.sheetName6TextBox.MouseEnter.Add whenMouseEnterTextBox
        this.columnStart1TextBox.TextChanged.Add whenTextChanged
        this.columnStart1TextBox.MouseEnter.Add whenMouseEnterTextBox
        this.columnStart2TextBox.TextChanged.Add whenTextChanged
        this.columnStart2TextBox.MouseEnter.Add whenMouseEnterTextBox
        this.columnStartTextBox.TextChanged.Add whenTextChanged
        this.columnStartTextBox.MouseEnter.Add whenMouseEnterTextBox
        this.rowStartTextBox.TextChanged.Add whenTextChanged
        this.rowStartTextBox.MouseEnter.Add whenMouseEnterTextBox

        this.firstRowIsHeadersCheckBox.Click.Add whenTextChanged
        
        this.Loaded.Add whenLoaded
        this.Closing.Add whenClosing
        this.Closed.Add whenClosed
        this.Button2.Click.Add Button2Click
        this.Button3.Click.Add Button3Click   
    
    override this.OnKeyDownHandler(_: obj, e: System.Windows.Input.KeyEventArgs) = 
        match (e.Key = System.Windows.Input.Key.Escape) with 
        | true  -> 
                 this.WindowState <- Windows.WindowState.Minimized
                 Application.Restart()
                 Environment.Exit(0)
        | false -> 
                 this.WindowState <- Windows.WindowState.Normal  
*)    