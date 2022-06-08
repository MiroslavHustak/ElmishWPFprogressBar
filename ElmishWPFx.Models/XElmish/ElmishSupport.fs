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

module Elmish.Support

module Async =

    let map f computation = async.Bind(computation, f >> async.Return)

// The Deferred type and a modified AsyncOperationStatus type from
// https://zaid-ajaj.github.io/the-elmish-book
// AsyncOperationStatus is modified to support a start parameter,
// for cases when input data is not found in the model. It also
// has a FinishAsync case that often handles the Result type,
// and a FailAsync case that usually handles the exception from
// Cmd.Async.either

type Aos<'argument,'result,'error> = // short for AsyncOperationStatus
    | StartAsync of 'argument
    | FinishAsync of 'result
    | FailAsync of 'error

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't
