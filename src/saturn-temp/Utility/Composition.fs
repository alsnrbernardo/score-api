namespace Utility

module Composition =

    type Outcome<'a, 'b> =
    | Success of 'a
    | Failure of 'b
    
    let bind f =
        function
        | Success value -> f value
        | Failure error -> Failure error

    let (>>=) input f =
        bind f input

    let map f = bind (f >> Success)

    module TaskComp =

        open FSharp.Control.Tasks.ContextInsensitive
        open System.Threading.Tasks

        let elevate f outcome =
            task {
              match outcome with
              | Success value -> return! f value
              | Failure f -> return Failure f
            }    