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

//Non-optional variant
module MainWindowNonOpt =

    open System
    open Serilog
    open System.Windows.Media

    open Elmish
    open Elmish.WPF
    open ElmishWPFx.Models

    open PatternBuilders

    let private header1 = "Calculations" 
    let private header2 = "Settings" 

    let newGuid () = Guid.NewGuid()

    type Toolbutton =
        {
            Id: Guid
            Text: string
            IsMarkable: bool
        }

    type Tab =
        {
            Id: Guid
            Header: string
            Toolbuttons: Toolbutton list
        }

    type Msg =
        | ButtonClick of id: Guid
        | ShowLeftCalc
        | ShowRightCalc
        | ShowSettings
        | ShowLicences
        | LeftCalcMsg of LeftCalc.Msg
        | RightCalcMsg of RightCalc.Msg
        | SettingsMsg of XElmishSettings.Msg
        | LicencesMsg of Licences.Msg
        | SetSelectedTabHeader of tabHeader:string

    type Model =
        {
            Tabs: Tab list
            MarkedButton: Guid
            LeftCalcPage: LeftCalc.Model 
            RightCalcPage: RightCalc.Model 
            SettingsPage: XElmishSettings.Model 
            LicencesPage: Licences.Model 
            SelectedTabHeader: string
        }
           
    let tbNone = newGuid ()
    let tbLeftCalc = newGuid ()
    let tbRightCalc = newGuid ()
    let tbSettings = newGuid ()
    let tbLicences = newGuid ()  

    let tabs =
        let tab header toolButtons =
            { Id = newGuid (); Header = header; Toolbuttons = toolButtons }
        let toolbutton id text isMarkable =
            { Id = id; Text = text; IsMarkable = isMarkable }
        let firstTab =
            [
              toolbutton tbLeftCalc "Left Calculation" true
              toolbutton tbRightCalc "Right Calculation" true         
            ]
        let secondTab = 
            [      
              toolbutton tbSettings "Settings Page" true
              toolbutton tbLicences "Licences" true      
            ]
        [ tab header1 firstTab; tab header2 secondTab ]
    
    let leftCalcPage, (leftCalcPageCmd: Cmd<LeftCalc.Msg>) = LeftCalc.init () 
    let rightCalcPage, (rightCalcPageCmd: Cmd<RightCalc.Msg>) = RightCalc.init ()
    let settingsPage, (settingsPageCmd: Cmd<XElmishSettings.Msg>) = XElmishSettings.init ()
    let licencesPage, (licencesPageCmd: Cmd<Licences.Msg>) = Licences.init () 
    
    let startModel =
        {
            Tabs = tabs
            MarkedButton = tbLeftCalc           
            LeftCalcPage = leftCalcPage
            RightCalcPage = rightCalcPage               
            SettingsPage = settingsPage
            LicencesPage = licencesPage
            SelectedTabHeader = (tabs |> List.item 0).Header
        }

    let init () : Model * Cmd<Msg> = startModel, Cmd.map LeftCalcMsg leftCalcPageCmd

    let findButton (id: Guid) (m: Model) =
        m.Tabs |> List.tryPick (fun tab -> tab.Toolbuttons |> List.tryFind (fun tb -> tb.Id = id))

    let update (msg: Msg) (m: Model) = 
        match msg with
        | ButtonClick id ->
            match findButton id m with
            | None -> m, Cmd.none
            | Some clickedButton ->                                              
                                  let m = 
                                      match clickedButton.IsMarkable with
                                      | true  -> { m with MarkedButton = id }
                                      | false -> m                            

                                  MyPatternBuilder    
                                      {    
                                          let! _ = not (clickedButton.Id = tbLeftCalc), (m, Cmd.ofMsg ShowLeftCalc) 
                                          let! _ = not (clickedButton.Id = tbRightCalc), (m, Cmd.ofMsg ShowRightCalc)
                                          let! _ = not (clickedButton.Id = tbSettings), (m, Cmd.ofMsg ShowSettings)  
                                          let! _ = not (clickedButton.Id = tbLicences), (m, Cmd.ofMsg ShowLicences)  
                                          
                                          return m, Cmd.none 
                                      }       

        | ShowLeftCalc  -> { m with LeftCalcPage = fst (LeftCalc.init());  }, Cmd.none          
        | ShowRightCalc -> { m with RightCalcPage = fst (RightCalc.init()) }, Cmd.none          
        | ShowSettings  -> { m with SettingsPage = fst (XElmishSettings.init()) }, Cmd.none 
        | ShowLicences  -> { m with LicencesPage = fst (Licences.init ()) }, Cmd.none 
         
        | LeftCalcMsg msg' ->
                             let m', cmd' = LeftCalc.update msg' m.LeftCalcPage
                             { m with LeftCalcPage = m' }, Cmd.map LeftCalcMsg cmd'
        | RightCalcMsg msg' ->
                             let m', cmd' = RightCalc.update msg' m.RightCalcPage
                             { m with RightCalcPage = m' }, Cmd.map RightCalcMsg cmd'        
        | SettingsMsg msg' ->
                             let m', cmd' = XElmishSettings.update msg' m.SettingsPage
                             { m with SettingsPage = m' }, Cmd.map SettingsMsg cmd'
        | LicencesMsg msg' ->
                             let m', cmd' = Licences.update msg' m.LicencesPage
                             { m with LicencesPage = m' }, Cmd.map LicencesMsg cmd'

        | SetSelectedTabHeader header ->           
            match header with
            | value when header1 = header -> { m with MarkedButton = tbLeftCalc; SelectedTabHeader = value }, Cmd.ofMsg ShowLeftCalc 
            | value when header2 = header -> { m with MarkedButton = tbSettings; SelectedTabHeader = value }, Cmd.ofMsg ShowSettings 
            | _                           -> { m with SelectedTabHeader = header }, Cmd.none
        
    let bindings(): Binding<Model,Msg> list =
        [
            "Tabs" |> Binding.subModelSeq((fun m -> m.Tabs), (fun t -> t), fun () ->
                [
                    "Id" |> Binding.oneWay (fun (_, t) -> t.Id)
                    "Header" |> Binding.oneWay (fun (_, t) -> t.Header)
                    "Toolbuttons" |> Binding.subModelSeq((fun (_, t) -> t.Toolbuttons), (fun t -> t), fun () ->
                        [
                            "Id" |> Binding.oneWay (fun (_, t) -> t.Id)
                            "Text" |> Binding.oneWay (fun (_, t) -> t.Text)
                            // "ImageSource" |> Binding.oneWay (fun (_, t) -> t.ImageSource)
                            "Foreground" |> Binding.oneWay (fun (_, t) -> Brushes.Green)
                            "ButtonClick" |> Binding.cmd (fun (_, (t: Toolbutton)) -> ButtonClick t.Id)
                            "MarkerVisible" |> Binding.oneWay (fun ((m, tab), tb) -> tb.Id = m.MarkedButton)
                        ])
                ])            
             
            "LeftCalcPage"
            |> Binding.SubModel.required LeftCalc.bindings 
            |> Binding.mapModel (fun m -> m.LeftCalcPage)
            |> Binding.mapMsg LeftCalcMsg
            "RightCalcPage"
            |> Binding.SubModel.required RightCalc.bindings
            |> Binding.mapModel (fun m -> m.RightCalcPage)
            |> Binding.mapMsg RightCalcMsg
            "SettingsPage"
            |> Binding.SubModel.required XElmishSettings.bindings 
            |> Binding.mapModel (fun m -> m.SettingsPage)
            |> Binding.mapMsg SettingsMsg
            "LicencesPage"
            |> Binding.SubModel.required Licences.bindings 
            |> Binding.mapModel (fun m -> m.LicencesPage)
            |> Binding.mapMsg LicencesMsg
   
            "LeftCalcPageVisible"  |> Binding.oneWay (fun m -> m.MarkedButton = tbLeftCalc)
            "RightCalcPageVisible" |> Binding.oneWay (fun m -> m.MarkedButton = tbRightCalc)
            "SettingsPageVisible"  |> Binding.oneWay (fun m -> m.MarkedButton = tbSettings)
            "LicencesPageVisible"  |> Binding.oneWay (fun m -> m.MarkedButton = tbLicences)   
            "SelectedTabHeader"    |> Binding.twoWay ((fun m -> m.SelectedTabHeader), SetSelectedTabHeader)
        ]    
    
    let designVm = ViewModel.designInstance startModel (bindings())
    

    //Equivalent code in FsXaml
       (*
       
       namespace FsXaml
       
       open System
       open System.Windows.Input
       
       open MainLogicLeft
       open MainLogicRight
       
       type MainWindowXaml = FsXaml.XAML<"XAMLAndCodeBehind/MainWindow.xaml">
       
       type MainWindow() as this =
       
           inherit MainWindowXaml()
       
           let whenLoaded _ =
               ()
       
           let whenClosing _ =
               ()
       
           let whenClosed _ =
               ()
       
           let Button1Click _ = 
              
               this.TextBox1.Clear()
               this.TextBox1.Text <- textBoxString1()
          
           let Button2Click _ =   
       
               let textBoxString2() = 
                   async
                       {
                           let result = textBoxString2()
                           return result
                       }  
       
               let asyncOperation() =   
                   async
                       {
                           let context = System.Threading.SynchronizationContext.Current
       
                           this.ProgressBar1.Background <- System.Windows.Media.Brushes.White
                           this.ProgressBar1.Foreground <- System.Windows.Media.Brushes.Cyan
                           this.ProgressBar1.IsIndeterminate <- true
                         
                           this.TextBox1.Text <- "Údaje se posílají do Google Sheets tabulky ..."
                         
                           do! Async.SwitchToThreadPool() 
       
                           let! result2 = textBoxString2() //let! v async -> asynchronously waiting for another async computation to complete.
                 
                           do! Async.SwitchToContext context
       
                           this.TextBox1.Clear()
                           this.TextBox1.Text <- result2
       
                           this.ProgressBar1.Background <- System.Windows.Media.Brushes.LightSkyBlue
                           this.ProgressBar1.IsIndeterminate <- false
                       } 
               Async.StartImmediate(asyncOperation()) 
       
           let Button3Click _ = 
       
               this.TextBox1.Clear()
       
               let low = string (this.TextBox2.Text)
               let high = string (this.TextBox3.Text)
               let path = string (this.TextBox4.Text) 
       
               //let low = "3" //for testing purposes
               //let high = "251" //for testing purposes
               //let path = @"o:\Litoměřice 8. část\"  //for testing purposes    
               
               let textBoxString4() = 
                   async
                       {
                           let result = textBoxString4 low high
                           return result
                       }                  
                                      
               let textBoxString3() =        
                   async
                       {
                           let result = textBoxString3 low high path 
                           return result
                       }  
                      
               let asyncOperation34() = 
                   async
                       {
                           let context = System.Threading.SynchronizationContext.Current
       
                           this.ProgressBar2.Background <- System.Windows.Media.Brushes.White
                           this.ProgressBar2.Foreground <- System.Windows.Media.Brushes.Cyan
                           this.ProgressBar2.IsIndeterminate <- true
       
                           this.TextBox1.Text <- "Příslušné hodnoty se načítají a zpracovávají ..."
       
                           do! Async.SwitchToThreadPool() //zacatek
       
                           //nize uvedene nesmi odkazovat na UI thread, proto match az po hopping back on the UI thread
                           let! result4 = textBoxString4()  
                           let! result3 = textBoxString3()                               
                           
                           do! Async.SwitchToContext context //konec
                          
                           this.TextBox1.Clear()
                           
                           match result4 >= 0 with
                           | false -> this.TextBox1.Text <- result3
                           | true  -> this.TextBox1.Text <- "Chybný rozdíl limitních hodnot anebo limity nebyly vůbec zadány" 
                         
                           this.ProgressBar2.Background <- System.Windows.Media.Brushes.LightSkyBlue
                           this.ProgressBar2.IsIndeterminate <- false
                       } 
               Async.StartImmediate(asyncOperation34()) 
       
           let Button4Click _ = //ponechano pro vyukove a testovaci ucely
           
               this.TextBox1.Clear()
       
               let asyncOperationTest() = 
                   async
                       {
                           // SynchronizationContext -> provides a way to queue a unit of work to a context. Note that this unit of work is queued to a context rather than a specific thread. This distinction is important, because many implementations of SynchronizationContext aren’t based on a single, specific thread.
                           // Predchudce vyse uvedeneho: ISynchronizeInvoke -> a “source” thread can queue a delegate to a “target” thread, optionally waiting for that delegate to complete.
                           let context = System.Threading.SynchronizationContext.Current
           
                           this.ProgressBar2.Background <- System.Windows.Media.Brushes.White
                           this.ProgressBar2.Foreground <- System.Windows.Media.Brushes.Cyan    
                           this.ProgressBar2.Value <- 0.0    
          
                           this.TextBox1.Text <- "Testing (ProgressBar, MailboxProcessor<Message>, PostAndReply) ..."
           
                           do! Async.SwitchToThreadPool() 
           
                           // We capture the current context and do some work on thread pool. When we finished, we need to continue executing 
                           // the rest of our program. But we want to do it in the correct context. That is the working environment of the UI thread 
                           // and not the thread pool thread. So we pass what’s needed to be done to Post method of our SynchronizationContext and that code runs on UI thread. 
                           // By doing that we solve the problem of object’s thread ownership.
                           // Async.StartImmediate will start an async on the current thread. A typical use is with a GUI, you have some GUI app that wants to e.g. update 
                           // the UI (e.g. to say "loading..." somewhere), and then do some background work (load something off disk or whatever), and then 
                           // return to the foreground UI thread to update the UI when completed ("done!"). StartImmediate enables an async to update the UI 
                           // at the start of the operation and to capture the SynchronizationContext so that at the end of the operation is can return to the GUI 
                           // to do a final update of the UI.
                           do! Async.SwitchToContext context
                                 
                           this.TextBox1.Clear()      
                           this.ProgressBar2.Value <- 0.0
                           this.ProgressBar2.Background <- System.Windows.Media.Brushes.LightSkyBlue
                       } 
               Async.StartImmediate(asyncOperationTest()) 
           
           let Button5Click _ =  
       
               let settingsWindow = new SettingsWindow() 
               do settingsWindow.Show()
       
           do
               this.Loaded.Add whenLoaded
               this.Closing.Add whenClosing
               this.Closed.Add whenClosed
               this.Button1.Click.Add Button1Click
               this.Button2.Click.Add Button2Click
               this.Button3.Click.Add Button3Click
               this.Button5.Click.Add Button5Click
           
           override this.OnKeyDownHandler(_: obj, e: System.Windows.Input.KeyEventArgs) = 
               match (e.Key = Key.Escape) with 
               | true  -> this.WindowState <- Windows.WindowState.Minimized
               | false -> this.WindowState <- Windows.WindowState.Normal         
       *)
