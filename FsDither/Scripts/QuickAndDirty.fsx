let countDown = 
    10 
    |> Seq.unfold (fun s -> if s < 0 then None else Some (string s, s - 1)) 
    |> Seq.toList

// unfold: ["10"; "9"; "8"; "7"; "6"; "5"; "4"; "3"; "2"; "1"; "0"]
printfn "unfold: %A" countDown