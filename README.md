If you are new to Elmish.WPF, please check out information about Elmish.WPF first (https://github.com/elmish/Elmish.WPF).

Background:

There is an example with Progress Bar Determinate on GitHub (https://github.com/BentTranberg/ElmishXmas) - DemoAsync and DemoAsync.Core projects with a single window. But a real-life application will probably look more like this: https://github.com/BentTranberg/ExploreElmishWpf, where a window is used with tabs. I ran into difficulties while implementing progress bars into my ExploreElmishWpf-based app. The problems were dealt with here: https://stackoverflow.com/questions/70208381/updating-progressbar-value-in-fsxaml-and-elmishwpf, and I greatly appreciate the help of Bent Tranberg's.

Progress Bars:

Check out RightCalc.fs, MainWindowOpt/NonOpt.fs in the XElmish folder, MainLogicRight.fs in the MainLogic folder and the relevant xaml files. TestButtonRightEvent in the "update" function of the RightCalc.fs file fires both Progress Bar Determinate and Progress Bar Indeterminate. Names/notes/comments important for you are in English. Long time operations in my app worth bothering with progress bars relate to dealing with up to thousands of directories, tens of thousands of files as well as writing to and reading from GoogleSheets tables. But you are not going to create a new table or thousands of dummy files just to test the progress bars, of course, so the code already refers to a dummy function simulating a long running operation. Just un-comment this line: //let! hardWork = Async.StartChild (workToDoRight reportProgress) and comment out this line: let! hardWork = Async.StartChild (async { return textBoxString3 m.LowLimit m.HighLimit m.PathToSynology reportProgress }). However, check out the function "myArray" in MainLogicRight.fs to see how Progress Bar Determinate is used in reality.

TestButtonLeftEvent in LeftCalc.fs will give you no more significant information on Progress Bar Determinate than ElmishXmas, so no special need to go there.

I am neither a GUI nor front-end expert, so if you are looking for an inspiration in Elmish.WPF related to some awesome GUI, you have to look elsewhere.


Disclaimer:

This F# app was successfully used in production. However, it serves as a learning app for the author of the code as well. Therefore, the code may contain some ostensibly "redundant" or "strange-looking" code. 
The operation of this desktop app was undertaken under my supervision, therefore exceptions and validations were coded accordingly. 
Legacy C# code in the file GoogleSheetsHelper.cs is not mine.

Nowadays, I would code the entire app in a different way, of course. For example, I would transition to idiomatic Result-type error-handling while departing from my previous unsuccessful attempts at railway-oriented-style error handling. Moreover, I would adopt a more judicious approach to Option-type usage, minimizing unnecessary caution.
