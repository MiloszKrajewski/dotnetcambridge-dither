open System
open System.Threading
open System.IO

let rec listFiles root = 
    seq {
        yield! Directory.GetFiles(root)
        yield! Directory.GetDirectories(root) |> Seq.map listFiles |> Seq.collect id
    }

let defaultOf<'a> () = 
    Unchecked.defaultof<'a>
let deleteFile fn = 
    File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.None).Dispose()
let oblivious func arg = 
    try func arg with | _ -> defaultOf ()
let logging label func arg =
    try
        printfn "%s: %A" label arg
        func arg
    with 
    | e -> 
        e.ToString() |> printfn "%s"
        reraise ()

"C:\\Windows" |> listFiles |> Seq.iter (deleteFile |> logging "Deleting" |> oblivious)

type Result<'a> = 
    | Result of 'a
    | Error of Exception

let retry1 interval countLimit timeLimit action arg = 
    let timeLimit = 
        match timeLimit with
        | None -> DateTime.MaxValue
        | Some secs -> DateTime.UtcNow.Add(TimeSpan.FromSeconds(secs))
    let exceptions = ResizeArray()
    let rec loop index = 
        let expired = 
            index <> 0 && (index > countLimit || DateTime.UtcNow > timeLimit)
        if expired then
            AggregateException(exceptions) |> raise
        else
            if index > 0 then
                Thread.Sleep(TimeSpan.FromSeconds(interval))
            let result = try action arg |> Result with | e -> e |> Error
            match result with | Result r -> r | Error e -> exceptions.Add(e); loop (index + 1)
    loop 0

let retry2 interval retry action arg = 
    let exceptions = ResizeArray()
    let start = DateTime.UtcNow
    let rec loop index = 
        let expired = (index <> 0) && not (retry index (DateTime.UtcNow.Subtract(start).TotalSeconds))
        if expired then 
            AggregateException(exceptions) |> raise
        else
            if index > 0 then Thread.Sleep(TimeSpan.FromSeconds(interval))
            let result = try action arg |> Result with | e -> e |> Error
            match result with | Result r -> r | Error e -> exceptions.Add(e); loop (index + 1)
    loop 0

let upToSecs secs c t = t < secs
let call fn = fn ()
let action () = printfn "exec"; failwith "error"

retry2 0.5 (upToSecs 2.0) call action |> ignore
