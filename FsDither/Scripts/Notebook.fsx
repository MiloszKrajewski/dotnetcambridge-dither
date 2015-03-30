open System
    
/// retries given action
/// returns its result or throws exception 
/// after `countLimit` failures or when `timeLimit` expires
let retry countLimit timeLimit action =
    let timeLimit = DateTime.Now.Add(TimeSpan.FromSeconds(timeLimit))
    let rec retry count =
        match count, DateTime.Now with
        | c, t when (c > 0) && (c > countLimit || t > timeLimit) -> 
            failwith "Operation failed"
        | c, _ -> 
            try action () with | _ -> retry (c + 1)
    retry 0