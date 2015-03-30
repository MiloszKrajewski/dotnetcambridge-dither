type PersonDTO(name, age) =
    member x.Name = name
    member x.Age = age
    override x.ToString() = sprintf "Person(Name: \"%s\", Age: %d)" name age

let p = PersonDTO("Milosz", 18)
printfn "%A" p