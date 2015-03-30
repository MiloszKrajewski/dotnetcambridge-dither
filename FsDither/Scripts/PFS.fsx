let enumeratePatches ph pw th tw =
    let inline patch y0 xc = 
        let y1 = (y0 + ph |> min th) - 1
        let ph' = y1 - y0 + 1
        let x1 = (xc + pw |> min tw) - 1
        let x0 = xc - ph' + 1
        (y0, x0, y1, x1)
    let inline overlaps (y0, x0, y1, x1) = y1 >= 0 && x1 >= 0 && y0 < th && x0 < tw
    let inline moveE (y0, xc) = (y0, xc + pw)
    let inline moveSW (y0, xc) = (y0 + ph, xc - pw - ph - 1)

    let rec walk ((y0, xc) as p) =
        let inline finished (y0, _, _, _) = y0 >= th
        let patch = patch y0 xc
        seq {
            match finished patch, overlaps patch with
            | true, _ -> ()
            | _, true -> yield (y0, xc); yield! walk (moveE p)
            | _ -> yield! walk (moveSW p)
        }

    let rec fork ((y0, xc) as p) =
        let inline finished (y0, _, _, x1) = y0 >= th || x1 < 0
        let patch = patch y0 xc
        seq {
            if not (finished patch) then 
                yield (y0, xc)
                yield! fork (moveSW p)
        }

    (0, 0) |> walk |> Seq.map fork
