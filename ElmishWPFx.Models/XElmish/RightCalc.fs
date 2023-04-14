namespace ElmishWPFx.Models

module RightCalc =

    open System

    open Elmish
    open Elmish.WPF

    open MainLogicRight
    open Elmish.Support

    open FSharp.Control
    open System.Windows.Media
    
    type ProgressIndicator = Idle | InProgress of percent: int
    
    type Model =
        {
            MainTextBoxText: string
            LowLimit: string
            HighLimit: string
            PathToSynology: string
            ProgressIndeterminateRight: bool
            ProgressBackgroundRight: Brush
            ProgressIndicatorRight: ProgressIndicator    
        }
    
    let initialModel = 
        {
            MainTextBoxText = String.Empty
            LowLimit = String.Empty
            HighLimit = String.Empty
            PathToSynology = String.Empty
            ProgressIndeterminateRight = false
            ProgressBackgroundRight = Brushes.LightSkyBlue
            ProgressIndicatorRight = Idle
        }
    
    let init(): Model * Cmd<'a>  = initialModel, Cmd.none
            
    type Msg =
        | SynologyButtonEvent of Aos<unit, Result<string, string>, exn>
        | LowLimitChanged of string
        | HighLimitChanged of string
        | PathToSynologyChanged of string
        | UpdateStatusRight of progress:int
        | WorkIsCompleteRight of string
        | TestButtonRightEvent     
    
    //************************ Synology *****************************
    let private msgSynologyButtonEvent = SynologyButtonEvent(StartAsync()) 
    
    let private mainLogicRight low high path =     
        let result4 = textBoxString4 low high 
        let result3 =
            let reportProgress progressValue = () //dummy partial app fn
            textBoxString3 low high path reportProgress
        match result4 >= 0 with
        | false -> result3
        | true  -> "Chybný rozdíl limitních hodnot anebo limity nebyly vůbec zadány" 
     
    let private asyncOperationSynology(m: Model): Async<Result<string,string>> = 
        async 
           {
               let! result = async { return mainLogicRight m.LowLimit m.HighLimit m.PathToSynology } 
               return Result.Ok result
           }
      
    let private synologyButtonEvent event (m: Model) =
        match event with
        | StartAsync () ->
            { m with MainTextBoxText = "Příslušné hodnoty se načítají a zpracovávají ..." 
                     ProgressIndeterminateRight = true
                     ProgressBackgroundRight = Brushes.White
            }, Cmd.OfAsync.either asyncOperationSynology (m: Model) (FinishAsync >> SynologyButtonEvent) (FailAsync >> SynologyButtonEvent) //vsimni si parametru asyncOperation a te mezery u () 
        | FinishAsync (Result.Ok value) ->
            { m with MainTextBoxText = value
                     ProgressIndeterminateRight = false
                     ProgressBackgroundRight = Brushes.LightSkyBlue
            }, Cmd.none
        | FinishAsync (Result.Error error) ->
            { m with MainTextBoxText = $"Done, but with error {error}" 
                     ProgressIndeterminateRight = false
                     ProgressBackgroundRight = Brushes.LightSkyBlue
            }, Cmd.none
        | FailAsync (ex: exn) ->
            { m with MainTextBoxText = $"Done, but with exception message {ex.Message}" 
                     ProgressIndeterminateRight = false
                     ProgressBackgroundRight = Brushes.LightSkyBlue
            }, Cmd.none
    
    let update (msg: Msg) (m: Model) : Model * Cmd<Msg> = 
        match msg with  
        | SynologyButtonEvent event  -> synologyButtonEvent event m //Only for ProgressBar Indeterminate (right)                             
        | LowLimitChanged low        -> { m with LowLimit = low }, Cmd.none
        | HighLimitChanged high      -> { m with HighLimit = high }, Cmd.none
        | PathToSynologyChanged path -> { m with PathToSynology = path }, Cmd.none// -> { m with PathToSynology =  @"o:\Litoměřice 8. část\" }, Cmd.none
        | UpdateStatusRight progress -> { m with MainTextBoxText = "Příslušné hodnoty se načítají a zpracovávají ..."; ProgressIndicatorRight = InProgress progress; ProgressBackgroundRight = Brushes.White }, Cmd.none 
        | WorkIsCompleteRight result -> { m with ProgressIndicatorRight = Idle; ProgressBackgroundRight = Brushes.LightSkyBlue; MainTextBoxText = result; ProgressIndeterminateRight = false }, Cmd.none 
        | TestButtonRightEvent       -> let tryWith =
                                            try
                                                try  
                                                   //failwith "Simulated exception2" 
                                                   let delayedCmd (dispatch: Msg -> unit): unit =       
                                                       let delayedDispatch: Async<unit> = 
                                                           async
                                                              {
                                                                  let reportProgress progressValue = dispatch (UpdateStatusRight progressValue)
                                                                  let result4 = textBoxString4 m.LowLimit m.HighLimit  
                                                                  //let! hardWork = Async.StartChild (workToDoRight reportProgress) //test case
                                                                  let! hardWork = Async.StartChild (async { return textBoxString3 m.LowLimit m.HighLimit m.PathToSynology reportProgress }) //real-life case
                                                                  do! Async.Sleep 1000 // Can do some async work here too while waiting for hardWork to finish.
                                                                  let! result3 = hardWork 
                                                                  let result = 
                                                                      match result4 >= 0 with
                                                                      | false -> result3
                                                                      | true  -> "Chybný rozdíl limitních hodnot anebo limity nebyly vůbec zadány"
                                                                  dispatch (WorkIsCompleteRight result)
                                                              }                                       
                                                       Async.StartImmediate delayedDispatch
                                                   { m with ProgressIndicatorRight = InProgress 0; ProgressIndeterminateRight = true }, Cmd.ofSub delayedCmd    
                                                finally
                                                   ()
                                            with
                                            | ex -> { m with ProgressIndicatorRight = Idle; MainTextBoxText = ex.Message; }, Cmd.none        
                                        tryWith     
                                       
    //cmdIf disables the relevant button, cmd does not
    let bindings(): Binding<Model,Msg> list =
        [
            "MainTextBoxText"      |> Binding.oneWay(fun m -> m.MainTextBoxText)
            "SynologyButton"       |> Binding.cmd msgSynologyButtonEvent  //Only for ProgressBar Indeterminate (right) 
            "LowLimit"             |> Binding.twoWay((fun m -> m.LowLimit), (fun newVal -> newVal |> LowLimitChanged))
            "HighLimit"            |> Binding.twoWay((fun m -> m.HighLimit), (fun newVal -> newVal |> HighLimitChanged))
            "PathToSynology"       |> Binding.twoWay((fun m -> m.PathToSynology), (fun newVal -> newVal |> PathToSynologyChanged))
            "ProgressRightIndeter" |> Binding.oneWay(fun m -> m.ProgressIndeterminateRight)  
            "ProgressRightBackg"   |> Binding.oneWay(fun m -> m.ProgressBackgroundRight) 
            "ProgressRight"        |> Binding.oneWay(fun m -> match m.ProgressIndicatorRight with Idle -> 0.0 | InProgress v -> float v)
            "TestButtonRight"      |> Binding.cmdIf(TestButtonRightEvent, fun m -> match m.ProgressIndicatorRight with Idle -> true | _ -> false)  //test case + real-life case
        ]