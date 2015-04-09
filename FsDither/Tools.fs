namespace FsDither

open System.IO
open System.Diagnostics

[<AutoOpen>]
module Tools =
    #if INTERACTIVE
    let root = __SOURCE_DIRECTORY__
    #else
    let root = "."
    #endif

    let timeit name func arg =
        let timer = Stopwatch.StartNew()
        let result = func arg
        timer.Stop()
        printfn "%s: %f" name timer.Elapsed.TotalMilliseconds
        result

    let resolvePath path = Path.Combine(root, path)
