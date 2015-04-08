namespace FsDither

module Triplet =
    let inline map f (a, b, c) = (f a, f b, f c)
    let inline reduce f (a, b, c) = a |> f b |> f c

module ISeq =
    open System
    open System.Threading.Tasks

    let inline fold state func (lo, hi) = 
        let mutable s = state
        for i = lo to hi do s <- func i s
        s

    let inline iter func (lo, hi) = 
        for i = lo to hi do func i

    let inline piter func (lo, hi) =
        Parallel.For(lo, hi + 1, Action<int>(func)) |> ignore

module Seq =
    open FSharp.Collections.ParallelSeq

    let inline piter f s = PSeq.iter f s

module Matrix =
    open System.IO

    let inline widthOf matrix = Array2D.length2 matrix
    let inline heightOf matrix = Array2D.length1 matrix
    let inline sizeOf matrix = heightOf matrix, widthOf matrix

    let inline zeroCreate height width = Array2D.zeroCreate height width

    let inline blitRowToArray (matrix: 'a[,]) row column (array: 'a[]) index length =
        let inline clone i = array.[index + i] <- matrix.[row, column + i] 
        for i = 0 to length - 1 do clone i

    let inline blitArrayToRow (array: 'a[]) index (matrix: 'a[,]) row column length =
        let inline clone i = matrix.[row, column + i] <- array.[index + i]
        for i = 0 to length - 1 do clone i

    let setRow matrix row vector =
        let matrixWidth = matrix |> widthOf
        let vectorLength = vector |> Array.length
        let length = min matrixWidth vectorLength
        let inline clone i = matrix.[row, i] <- vector.[i]
        for i = 0 to length - 1 do clone i

    let getRow matrix row =
        let length = matrix |> widthOf
        let inline clone i = matrix.[row, i]
        Array.init length clone

    let inline init height width func = Array2D.init height width func
    let inline map func matrix = Array2D.map func matrix
    let inline mapi func matrix = Array2D.mapi func matrix

    let pinit height width func =
        let result = zeroCreate height width
        let inline initPixel y x = result.[y, x] <- func y x
        let inline initRow row = for x = 0 to width - 1 do initPixel row x
        (0, height - 1) |> ISeq.piter initRow
        result

    let pmap func matrix =
        let height, width = matrix |> sizeOf
        let inline processPixel y x = func matrix.[y, x]
        pinit height width processPixel

    let pmapi func matrix =
        let height, width = matrix |> sizeOf
        let inline processPixel y x = func y x matrix.[y, x]
        pinit height width processPixel

