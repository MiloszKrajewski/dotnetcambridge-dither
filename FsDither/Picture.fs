namespace FsDither

open System.Drawing
open System.Drawing.Imaging

module Picture = 
    open Value
    open Pixel

    let fromBitmap (bitmap: Bitmap) =
        let width, height = bitmap.Width, bitmap.Height
        let matrix = Array2D.zeroCreate height width

        let inline cloneRow data row = 
            Bitmap.getPhysicalPixels data row
            |> Array.map Pixel.toLogicalPixel
            |> Matrix.setRow matrix row

        bitmap |> Bitmap.lockBits ImageLockMode.ReadOnly (fun data ->
            ISeq.piter 0 (height - 1) (cloneRow data))
        matrix

    let toBitmap matrix =
        let height, width = matrix |> Matrix.sizeOf
        let bitmap = new Bitmap(width, height, Bitmap.bitmapFormat)

        let inline cloneRow data row = 
            Matrix.getRow matrix row
            |> Array.map Pixel.toPhysicalPixel
            |> Bitmap.setPhysicalPixels data row

        bitmap |> Bitmap.lockBits ImageLockMode.WriteOnly (fun data ->
            ISeq.piter 0 (height - 1) (cloneRow data))
        bitmap

    let split (picture: Pixel[,]) =
        (Pixel.getR, Pixel.getG, Pixel.getB) 
        |> Triplet.map (fun f -> Matrix.pmap f picture)

    let join ((red: Value[,], green: Value[,], blue: Value[,]) as layers) =
        let width = layers |> Triplet.map Matrix.widthOf |> Triplet.reduce min
        let height = layers |> Triplet.map Matrix.heightOf |> Triplet.reduce min
        let inline combine y x = Pixel(red.[y, x], green.[y, x], blue.[y, x])
        Matrix.pinit height width combine

    let load fileName = 
        fileName |> Bitmap.load |> fromBitmap

    let show title picture = 
        picture |> toBitmap |> UI.show title

    let slideShow title pictures =
        pictures |> Seq.map toBitmap |> UI.slideShow title