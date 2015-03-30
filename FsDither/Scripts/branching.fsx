let length = 1000000
let random = 
    let r = System.Random(0)
    fun n -> r.Next() % n
let a = 
    { 0..length } 
    |> Seq.map (fun _ -> random length) 
    |> Seq.toArray
;;

// 720ms (100%)
let mutable x = 0;
let mutable y = 0;
for r = 0 to 100 do
    for i = 0 to length do 
        if a.[i] > 500000 then x <- x + 1 else y <- y + 1
;;
        
Array.sortInPlace a
;;

// 380ms (53%)
let mutable x = 0;
let mutable y = 0;
for r = 0 to 100 do
    for i = 0 to length do 
        if a.[i] > 500000 then x <- x + 1 else y <- y + 1
;;

// 280ms (39%)
let mutable z = 0;
for r = 0 to 100 do
    for i = 0 to length do 
        z <- z + 1
;;

