namespace FsDither

open System.Diagnostics
open System.IO

[<AutoOpen>]
module Tools =
    #if INTERACTIVE
    let root = __SOURCE_DIRECTORY__
    #else
    let root = "."
    #endif

    let timeit name func value =
        let timer = Stopwatch.StartNew()
        let result = func value
        timer.Stop()
        printfn "%s: %f" name timer.Elapsed.TotalMilliseconds
        result

    let inline timeit0 name func = timeit name func ()

    let resolvePath path = Path.Combine(root, path)
