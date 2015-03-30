open System

/// No body
let func1 a b f = a, b

/// f takes a as argument
let func2 a b f = f a, b

/// f takes a and b as argument
let func3 a b f = f a, f b

/// apparently a and b are string
let func4 a b f = f (a + " ?"), f (b + " !")

/// it seems that f returns int
let func5 a b f = f (a + " ?") + 1, f (b + " !")

/// retries given action maximum `countLimit` times 
/// or when given `timeLimit` expires
let retry countLimit timeLimit action =
    let timeLimit = DateTime.Now.Add(timeLimit)
    let rec retry count =
        match count, DateTime.Now with
        | c, t when c <> 0 && (c > countLimit || t > timeLimit) -> 
            failwith "Operation failed"
        | c, _ -> 
            try action () with | _ -> retry (c + 1)
    retry 0
