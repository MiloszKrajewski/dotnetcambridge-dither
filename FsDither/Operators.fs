namespace FsDither

[<AutoOpen>]
module Operators =
    let inline apply func arg = func arg |> ignore; arg
