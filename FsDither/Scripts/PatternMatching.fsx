let func1 p =
    let x, y = p
    printfn "as tuple: %A, as x and y: %A, %A" p x y

let func2 ((x, y) as p) =
    printfn "as tuple: %A, as x and y: %A, %A" p x y

let func3 ((x, _) as p) =
    let (_, y) = p
    printfn "as tuple: %A, as x and y: %A, %A" p x y

 // as tuple: (1234, 3455), as x and y: 1234, 3455
func1 (1234, 3455)
func2 (1234, 3455)
func3 (1234, 3455)

let func4 array =
    match array with
    | [| x; 0; 0 |] -> printfn "Yup, %d and two zeroes" x
    | [| _; 2; _ |] -> printfn "2 in the middle"
    | [| _; _; _ |] -> printfn "3 random elements"
    | _ -> printfn "C'mon I wanted 3 elements, not %d" array.Length

func4 [| 3; 0; 0 |] // Yup, 3 and two zeroes
func4 [| 4; 2; 2 |] // 2 in the middle
func4 [| 1; 1; 1 |] // 3 random elements
func4 [| 1 |] // C'mon I wanted 3 elements, not 1

open System.Text

let func5 (subject: obj) =
    match subject with
    | :? StringBuilder as sb -> sb.Append(" was a StringBuilder").ToString()
    | :? string as s -> s + " is a string"
    | _ -> invalidArg "subject" "No idea what to do"

// StringBuilder was a StringBuilder
func5 (StringBuilder().Append("StringBuilder")) |> printfn "%s"

// string is a string
func5 ("string") |> printfn "%s"


let sum = 
    [1; 2; 3; 4; 5] 
    |> Seq.reduce (fun a b -> a + b)

let concat = 
    [1; 2; 3; 4; 5] 
    |> Seq.fold (fun s n -> s + string n) ""
        
printfn "Sum: %A, Concat: %A" sum concat
