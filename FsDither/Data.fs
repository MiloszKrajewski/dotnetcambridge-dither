namespace FsDither

open System
open System.Threading.Tasks

module Triplet =
    let inline map f (a, b, c) = (f a, f b, f c)
    let inline reduce f (a, b, c) = a |> f b |> f c
    let inline pmap f (a, b, c) =
        match [|a; b; c|] |> Array.Parallel.map f with 
        | [|a; b; c|] -> (a, b, c) 
        | _ -> failwith "Not going to happen"

module Seq =
    let inline piter func (s: 'a seq) = 
        Parallel.ForEach(s, Action<'a>(func)) |> ignore

module Range =
    type Range = (int * int)

    let inline iter func (lo, hi) = 
        // { lo..hi } |> Seq.iter func
        for i = lo to hi do func i

    let inline piter func (lo, hi) =
        // { lo..hi } |> Seq.piter func
        Parallel.For(lo, hi + 1, Action<int>(func)) |> ignore

    let inline fold func state (lo, hi) =
        // { lo..hi } |> Seq.fold func state
        let mutable s = state
        for i = lo to hi do s <- func i s
        s

module Matrix = // Array@d
    let inline zeroCreate height width = Array2D.zeroCreate height width
    let inline init height width func = Array2D.init height width func
    let inline map func matrix = Array2D.map func matrix
    let inline mapi func matrix = Array2D.mapi func matrix

    let inline widthOf matrix = Array2D.length2 matrix
    let inline heightOf matrix = Array2D.length1 matrix
    let inline sizeOf matrix = heightOf matrix, widthOf matrix

    let pinit height width func =
        let result = zeroCreate height width
        let inline initPixel y x = result.[y, x] <- func y x
        let inline initRow row = for x = 0 to width - 1 do initPixel row x
        (0, height - 1) |> Range.piter initRow
        result

    let pmap func matrix =
        let height, width = matrix |> sizeOf
        let inline processPixel y x = func matrix.[y, x]
        pinit height width processPixel

    let pmapi func matrix =
        let height, width = matrix |> sizeOf
        let inline processPixel y x = func y x matrix.[y, x]
        pinit height width processPixel

    let inline blitRowToVector (matrix: 'a[,]) row column (array: 'a[]) index length =
        let inline clone i = array.[index + i] <- matrix.[row, column + i] 
        for i = 0 to length - 1 do clone i

    let inline blitVectorToRow (vector: 'a[]) index (matrix: 'a[,]) row column length =
        let inline clone i = matrix.[row, column + i] <- vector.[index + i]
        for i = 0 to length - 1 do clone i

    let applyRow matrix row vector =
        let matrixWidth = matrix |> widthOf
        let vectorLength = vector |> Array.length
        let length = min matrixWidth vectorLength
        blitVectorToRow vector 0 matrix row 0 length

    let extractRow matrix row =
        let length = matrix |> widthOf
        let vector = Array.zeroCreate length
        blitRowToVector matrix row 0 vector 0 length
        vector
