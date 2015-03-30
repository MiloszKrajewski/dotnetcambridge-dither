﻿namespace FsDither

open System.IO
open System.Collections.Generic

module Program = 
    [<EntryPoint>]
    let main argv = 
        let image = "flowers-large.jpg" |> Bitmap.load

        let greyscale = image |> Picture.fromBitmap |> Matrix.pmap Pixel.getL

//        using (new StreamWriter("d:\\temp.txt")) (fun writer -> 
//            greyscale
//            |> Bayer.processLayer Bayer.bayer8x8 Value.toAsciiHalftone
//            |> Array2D.iteri (fun y x v -> 
//                if x = 0 && y > 0 then writer.WriteLine()
//                writer.Write(v)
//            )
//        )

//        greyscale
//        |> timeit "sfloyd" (FloydSteinberg.processLayer (Value.quantize 2))
//        |> ignore

//        greyscale
//        |> timeit "pfloyd" (PFloydSteinberg.processLayer (Value.quantize 2))
//        |> ignore

        greyscale
        |> Matrix.pmap PintFloydSteinberg.fromFloat
        |> timeit "pintfloyd" (PintFloydSteinberg.processLayer 2)
        |> Matrix.pmap (PintFloydSteinberg.toFloat >> Pixel.fromL)
        |> Picture.toBitmap
        |> Bitmap.show "PintFloyded!"
        |> ignore

        // |> Matrix.pmap Pixel.fromL
        // |> Picture.toBitmap
        // |> Bitmap.show "pfloyd"

//        greyscale
//        |> timeit "pfloyd" (PxFloydSteinberg.processLayer (Value.quantize 2))
//        |> Matrix.pmap Pixel.fromL
//        |> Picture.toBitmap
//        |> Bitmap.show "pfloyd"

//        let debug (w: TextWriter) y x v = w.WriteLine(sprintf "(%d,%d)=%A" y x v)

//        let sfloyd = using (new StreamWriter("d:\\serial.dump")) (fun w -> 
//            let map = Dictionary()
//            let add y x v = map.Add((y, x), v)
//            let res = greyscale |> timeit "serial" (FloydSteinberg.processLayer add (Value.quantize 2))
//            map |> Seq.map (fun kv -> (kv.Key, kv.Value)) |> Seq.sortBy fst |> Seq.iter (fun ((y, x), v) -> debug w y x v)
//            res
//        )

//        image 
//        |> Picture.fromBitmap
//        // |> Image.split
//        // |> Triplet.map (Bayer.processLayer Bayer.bayer8x8 (Value.quantize 2))
//        // |> Image.join
//        |> Matrix.pmap Pixel.getL
//        // |> Bayer.processLayer Bayer.bayer3x3 (Value.quantize 2)
//        |> FloydSteinberg.processLayer (Value.quantize 2)
//        // |> FloydSteinbergParallel.processLayer (Value.quantize 2)
//        // |> Matrix.mapi (Random.processPixel 0 128 (Value.quantize 2))
//        |> Matrix.save (new StreamWriter("d:\\serial.dump"))
//        |> Matrix.pmap Pixel.fromL
//        |> Picture.toBitmap
//        |> Bitmap.show "title"

        printfn "Press <enter>..."
        System.Console.ReadLine() |> ignore
        0 // return an integer exit code