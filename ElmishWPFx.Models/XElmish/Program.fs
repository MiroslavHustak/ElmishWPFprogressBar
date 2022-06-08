(*
MIT License

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

module Elmish.Program

open Serilog
open Serilog.Extensions.Logging

open Elmish.WPF
open ElmishWPFx.Models

open Helpers.CopyingFiles

let main window = 

    copyFiles 
    <| "json.xml"
    <| "jsonBackUp.xml" 
   
    let logger =
        LoggerConfiguration()
            .MinimumLevel.Override("Elmish.WPF.Update", Events.LogEventLevel.Verbose)
            .MinimumLevel.Override("Elmish.WPF.Bindings", Events.LogEventLevel.Verbose)
            .MinimumLevel.Override("Elmish.WPF.Performance", Events.LogEventLevel.Verbose)
            .WriteTo.Console()
            .CreateLogger()
    
    //non-optional submodels              
    //WpfProgram.mkProgram MainWindowNonOpt.init MainWindowNonOpt.update MainWindowNonOpt.bindings
    //|> WpfProgram.withLogger (new SerilogLoggerFactory(logger))
    //|> WpfProgram.startElmishLoop window   
    
    //optional submodels
    WpfProgram.mkProgram MainWindowOpt.init MainWindowOpt.update MainWindowOpt.bindings
    |> WpfProgram.withLogger (new SerilogLoggerFactory(logger))
    |> WpfProgram.startElmishLoop window
   