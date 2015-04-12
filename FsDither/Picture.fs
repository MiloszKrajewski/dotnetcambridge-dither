namespace FsDither

open System.Drawing
open System.Drawing.Imaging

module Picture = 
    open Value
    open Pixel

    type Picture = Pixel[,]

    let fromBitmap (bitmap: Bitmap) =
        let width, height = bitmap.Width, bitmap.Height
        let matrix = Matrix.zeroCreate height width

        let inline cloneRow data row = 
            Bitmap.getPhysicalPixels data row
            |> Array.map Pixel.fromInt32
            |> Matrix.applyRow matrix row

        bitmap |> Bitmap.lockBits ImageLockMode.ReadOnly (fun data ->
            (0, height - 1) |> Range.piter (cloneRow data))
        matrix

    let load fileName = 
        fileName |> Bitmap.load |> fromBitmap

    let toBitmap picture =
        let height, width = picture |> Matrix.sizeOf
        let bitmap = new Bitmap(width, height, Bitmap.bitmapFormat)

        let inline cloneRow data row = 
            Matrix.extractRow picture row
            |> Array.map Pixel.toInt32
            |> Bitmap.setPhysicalPixels data row

        bitmap |> Bitmap.lockBits ImageLockMode.WriteOnly (fun data ->
            (0, height - 1) |> Range.piter (cloneRow data))
        bitmap

    let showOne title picture = 
        picture |> toBitmap |> UI.showOne title

    let showMany pictures =
        pictures |> Seq.map (fun (t, p) -> t, p |> toBitmap) |> UI.showMany

    let toGrayscale picture = picture |> Matrix.pmap Pixel.getL
    let fromGrayscale layer = layer |> Matrix.pmap Pixel.fromL

    let split (picture: Pixel[,]) = //!!!
        (Pixel.getR, Pixel.getG, Pixel.getB) 
        |> Triplet.map (fun f -> Matrix.pmap f picture)

    let join ((red: Value[,], green: Value[,], blue: Value[,]) as layers) = //!!!
        let width = layers |> Triplet.map Matrix.widthOf |> Triplet.reduce min
        let height = layers |> Triplet.map Matrix.heightOf |> Triplet.reduce min
        let inline combine y x = Pixel(red.[y, x], green.[y, x], blue.[y, x])
        Matrix.pinit height width combine

    let quickGrayscaleSlides algorithms picture =
        let greyscale = picture |> toGrayscale
        seq {
            for title, func in algorithms do
                let image = greyscale |> func |> fromGrayscale
                yield title, image
        } |> showMany
