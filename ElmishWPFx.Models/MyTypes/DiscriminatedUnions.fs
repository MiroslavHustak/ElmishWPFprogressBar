namespace ElmishWPFx.Models

module DiscriminatedUnions =

    type Result<'TSuccess,'TFailure> =

       | Success of 'TSuccess
       | Failure of 'TFailure

    [<Struct>]
    type TaskResults =    

       | TupleStringString of outputValues: string list * string list
       | MyListInt of myList: int list



