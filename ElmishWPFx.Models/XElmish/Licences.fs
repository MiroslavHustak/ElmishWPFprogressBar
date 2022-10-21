namespace ElmishWPFx.Models

module Licences =

    open System
    open Elmish
    open Elmish.WPF

    type Model = 
        {
            LicencesTextBox0: string
            LicencesTextBox1: string
            LicencesTextBox2: string
        }

    type Msg =
        | FixedText    

    let initialModel = 
        {
            LicencesTextBox0 = sprintf //(the &quot;Software&quot;)
                "\rMIT License\r\nCopyright (c) 2021 Bent Tranberg\r\r\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files \"Software\", to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\r\r\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software." 
            LicencesTextBox1 = sprintf
                "\rCode in this file was taken from this publicly available source (except where noted in the code): https://www.hardworkingnerd.com/how-to-read-and-write-to-google-sheets-with-c.\r\r\nThe website is owned by Ian Preston. No licence conditions specified - check the website for possible updates." 
            LicencesTextBox2 = sprintf
                "\rThe rest of the code was created by Miroslav Husťák.\r\r\nhttp://hustak.somee.com" 
        }

    let init(): Model * Cmd<'a> = initialModel, Cmd.none 

    let update (msg: Msg) (m: Model) : Model * Cmd<Msg> =
                   
               match msg with          
               | FixedText -> { m with LicencesTextBox0 = initialModel.LicencesTextBox0
                                       LicencesTextBox1 = initialModel.LicencesTextBox1
                                       LicencesTextBox2 = initialModel.LicencesTextBox2
                              }, Cmd.none    

    let bindings(): Binding<Model,Msg> list =
        [
          "licencesTextBox0"  |> Binding.oneWay(fun m -> m.LicencesTextBox0)
          "licencesTextBox1"  |> Binding.oneWay(fun m -> m.LicencesTextBox1)
          "licencesTextBox2"  |> Binding.oneWay(fun m -> m.LicencesTextBox2)         
        ]
  