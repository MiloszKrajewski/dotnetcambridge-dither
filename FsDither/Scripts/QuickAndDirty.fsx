open System

/// retries given action; 
/// returns its result or throws exception
/// after `countLimit` failures or when `timeLimit` expires
let retry countLimit timeLimit func arg =
    let timeLimit = DateTime.Now.Add(TimeSpan.FromSeconds(timeLimit))
    let rec retry count =
        try 
            func arg 
        with 
        | _ when (count <= countLimit && DateTime.Now <= timeLimit) -> 
            retry (count + 1)
        | _ -> 
            failwith "Operation failed"
    retry 0
