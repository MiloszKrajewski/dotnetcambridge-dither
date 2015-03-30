type Cell = 
    { Left: int; Top: int; Width: int; Height: int }
    member x.Right = x.Left + x.Width - 1
    member x.Bottom = x.Top + x.Height - 1

let diffusionCells offset cellSize (height, width) =
    let inline rect x0 y0 x1 y1 = 
        { Left = x0; Top = y0; Width = x1 - x0; Height = y1 - y0 }
    let rec emitRow y0 y1 row col =
        let origin = col * cellSize - row * offset
        let x0 = origin |> max 0
        let x1 = origin + cellSize |> min width
        seq {
            if x0 < width then
                if x1 > 0 then yield row + col, rect x0 y0 x1 y1
                yield! emitRow y0 y1 row (col + 1)
        }
    let rowCount = (height + cellSize - 1) / cellSize - 1
    let cells = 
        seq { 
            for row = 0 to rowCount do 
                let y0 = row * cellSize
                let y1 = y0 + cellSize |> min height
                yield! emitRow y0 y1 row 0 
        } 
    cells |> Seq.groupBy fst |> Seq.sortBy fst |> Seq.map (snd >> Seq.map snd)

let w, h = 2000, 2000
let s = 64
let m = Array2D.zeroCreate h w

printfn "populating..."
diffusionCells 1 s (h, w) |> Seq.iter (fun row ->
    printfn "**********"
    row |> Seq.iter (fun cell ->
        printfn "%d,%d -> %d,%d" cell.Left cell.Top cell.Right cell.Bottom
        for y = cell.Top to cell.Bottom do
            for x = cell.Left to cell.Right do
                if m.[y, x] then failwithf "duplicated @ %A" (y, x)
                m.[y, x] <- true
    )
)

printfn "testing..."
for y = 0 to h - 1 do
    for x = 0 to w - 1 do
        if not m.[y, x] then failwithf "missing @ %A" (y, x)
