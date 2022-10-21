namespace ElmishWPFx.Models

module LeftCalc =

    open System

    open Elmish
    open Elmish.WPF

    open MainLogicLeft
    open Elmish.Support

    open FSharp.Control
    open System.Windows.Media    

    type ProgressIndicator = Idle | InProgress of percent: int

    type Model =
        {
            MainTextBoxText: string           
            ProgressIndeterminateLeft: bool
            ProgressBackgroundLeft: Brush
            ProgressIndicatorLeft: ProgressIndicator
        }

    let initialModel = 
        {
            MainTextBoxText = String.Empty
            ProgressIndeterminateLeft = false
            ProgressBackgroundLeft = Brushes.LightSkyBlue
            ProgressIndicatorLeft = Idle
        }

    let init(): Model * Cmd<'a> = initialModel, Cmd.none
        
    type Msg =
        | NumberOfImagesButtonEvent  
        | GoogleButtonEvent of Aos<unit, Result<string, string>, exn>
        | UpdateStatusLeft of progress: int
        | WorkIsCompleteLeft
        | TestButtonLeftEvent   

    //**********************************************************************************************************************************
    // FOR TESTING PURPOSES ONLY 
    let private workToDoLeft dispatch = 
       async
           {
               for i in 1..100 do 
                   do! Async.Sleep 20  //simulating long running operation
                   dispatch (UpdateStatusLeft i) 
               dispatch WorkIsCompleteLeft 
            } 
    
    //**********************************************************************************************************************************

    //************************* Google ******************************
    let private msgGoogleButtonEvent = GoogleButtonEvent(StartAsync()) 

    let private asyncOperationGoogle(): Async<Result<string,string>> = 
        async 
           {
               let! result = async { return textBoxString2() } 
               return Result.Ok result
           }

    let private googleButtonEvent event (m: Model) =
        match event with
        | StartAsync () ->
            { m with MainTextBoxText = "Údaje se posílají do Google Sheets tabulky ..." 
                     ProgressIndeterminateLeft = true
                     ProgressBackgroundLeft = Brushes.White
            }, Cmd.OfAsync.either asyncOperationGoogle () (FinishAsync >> GoogleButtonEvent) (FailAsync >> GoogleButtonEvent) //vsimni si parametru asyncOperation a te mezery u () 
        | FinishAsync (Result.Ok value) ->
            { m with MainTextBoxText = value
                     ProgressIndeterminateLeft = false
                     ProgressBackgroundLeft = Brushes.LightSkyBlue
            }, Cmd.none
        | FinishAsync (Result.Error error) ->
            { m with MainTextBoxText = $"Done, but with error {error}" 
                     ProgressIndeterminateLeft = false
                     ProgressBackgroundLeft = Brushes.LightSkyBlue
            }, Cmd.none
        | FailAsync (ex: exn) ->
            { m with MainTextBoxText = $"Done, but with exception message {ex.Message}" 
                     ProgressIndeterminateLeft = false
                     ProgressBackgroundLeft = Brushes.LightSkyBlue
            }, Cmd.none

    let update (msg: Msg) (m: Model) : Model * Cmd<Msg> = 
        match msg with  
        | NumberOfImagesButtonEvent  -> { m with MainTextBoxText = textBoxString1() }, Cmd.none  
        | GoogleButtonEvent event    -> googleButtonEvent event m //Only for ProgressBar Indeterminate (left)        
        | UpdateStatusLeft progress  -> { m with MainTextBoxText = "ProgressBar test running ..."; ProgressIndicatorLeft = InProgress progress; ProgressBackgroundLeft = Brushes.White }, Cmd.none                      
        | WorkIsCompleteLeft         -> { m with ProgressIndicatorLeft = Idle; ProgressBackgroundLeft = Brushes.LightSkyBlue; MainTextBoxText = "End of ProgressBar test"; }, Cmd.none            
        | TestButtonLeftEvent        -> let tryWith =
                                            try
                                                try  
                                                   //failwith "Simulated exception1" 
                                                   let delayedCmd (dispatch: Msg -> unit) : unit =                                                        
                                                       let delayedDispatch = workToDoLeft dispatch //test case only, no need for a determinate ProgressBar (left) in this app 
                                                       Async.StartImmediate delayedDispatch 
                                                   { m with ProgressIndicatorLeft = InProgress 0 }, Cmd.ofSub delayedCmd     
                                                finally
                                                   ()
                                            with
                                            | ex -> { m with ProgressIndicatorLeft = Idle; MainTextBoxText = ex.Message; }, Cmd.none        
                                        tryWith                            
   
    //cmdIf disables the relevant button, cmd does not
    let bindings(): Binding<Model,Msg> list =
        [
          "MainTextBoxText"      |> Binding.oneWay(fun m -> m.MainTextBoxText)
          "NumberOfImagesButton" |> Binding.cmd NumberOfImagesButtonEvent  
          "GoogleButton"         |> Binding.cmd msgGoogleButtonEvent    //Only for ProgressBar Indeterminate (left)  
          "ProgressLeftIndeter"  |> Binding.oneWay(fun m -> m.ProgressIndeterminateLeft)           
          "ProgressLeftBackg"    |> Binding.oneWay(fun m -> m.ProgressBackgroundLeft) 
          "ProgressLeft"         |> Binding.oneWay(fun m -> match m.ProgressIndicatorLeft with Idle -> 0.0 | InProgress v -> float v)
          "TestButtonLeft"       |> Binding.cmdIf(TestButtonLeftEvent, fun m -> match m.ProgressIndicatorLeft with Idle -> true | _ -> false)    //test case
        ]