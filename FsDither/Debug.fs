namespace FsDither

module Debug =
    open System.IO
    open System.Diagnostics

    #if INTERACTIVE
    let fixPath path = Path.Combine(__SOURCE_DIRECTORY__, path)
    #else
    let inline fixPath path = path
    #endif

    let timeit name func arg = 
        let timer = Stopwatch.StartNew()
        let result = func arg
        timer.Stop()
        printfn "%s: %gms" name timer.Elapsed.TotalMilliseconds
        result

    let inline timeit0 name func = timeit name func () |> ignore

    let inline log name value = value |> apply (printfn "%s: %A" name)
    // let sumOfAllNumbers = [2; (log "creating" 3); 4; 5; 6] |> Seq.map (log "passing") |> Seq.sum |> log "sum"
