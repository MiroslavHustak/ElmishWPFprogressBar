module DiscriminatedUnions

type Result<'TSuccess,'TFailure> =

   | Success of 'TSuccess
   | Failure of 'TFailure

[<Struct>]
type TaskResults =    

   | TupleStringString of outputValues: string[] * string[]
   | MyArrayInt of myArray: int[]



