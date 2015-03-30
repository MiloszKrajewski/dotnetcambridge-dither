#load "load-project.fsx"
open FsDither

let flowers = Bitmap.load "college-huge.jpg"
let grayscale = flowers |> Picture.fromBitmap |> Matrix.map Pixel.getL 
let quantizer = Value.quantize 2

let gc () = System.GC.Collect(2, System.GCCollectionMode.Forced)

grayscale |> FloydSteinberg.processLayer quantizer |> ignore
grayscale |> PFloydSteinberg.processLayer quantizer |> ignore
grayscale |> Bayer.processLayer Bayer.bayer8x8 quantizer |> ignore

gc ()