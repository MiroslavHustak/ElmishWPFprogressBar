﻿namespace ElmishWPFx.Models

open System

module Other_Records =

    [<Struct>]
    type ErrMsgRecord  = 
        {
            errMsg0: string
            errMsg1: string 
            errMsg2: string 
        }

        module Err = 
            let myErrMsgDefaultRecord = 
                { 
                    ErrMsgRecord.errMsg0 = String.Empty
                    ErrMsgRecord.errMsg1 = String.Empty
                    ErrMsgRecord.errMsg2 = String.Empty              
                }