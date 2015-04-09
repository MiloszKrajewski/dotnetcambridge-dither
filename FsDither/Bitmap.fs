namespace FsDither

open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices

module Bitmap =
    let bitmapFormat = PixelFormat.Format32bppRgb

    let private enforceFormat (image: Image) =
        match image with
        | :? Bitmap as bitmap when bitmap.PixelFormat = bitmapFormat ->
            bitmap
        | _ ->
            let width, height = image.Width, image.Height
            let bitmap = new Bitmap(width, height, bitmapFormat)
            use graphics = Graphics.FromImage(bitmap)
            graphics.DrawImage(image, 0, 0, width, height)
            bitmap

    let load (fileName: string) = 
        let fileName = Tools.resolvePath fileName
        fileName |> Image.FromFile |> enforceFormat

    let lockBits (lockMode: ImageLockMode) (func: BitmapData -> 'a) (bitmap: Bitmap) =
        let width, height = bitmap.Width, bitmap.Height
        let rect = Rectangle(0, 0, width, height)
        let data = bitmap.LockBits(rect, lockMode, bitmapFormat)
        try
            func data
        finally
            bitmap.UnlockBits(data)

    let inline private physicalRowAddress line (data: BitmapData) = 
        data.Scan0 + nativeint (data.Stride * line)

    let getPhysicalPixels (data: BitmapData) row =
        assert (data.PixelFormat = bitmapFormat)
        let width = data.Width
        let buffer = Array.zeroCreate<int32> width
        let pointer = data |> physicalRowAddress row
        Marshal.Copy(pointer, buffer, 0, buffer.Length)
        buffer

    let setPhysicalPixels (data: BitmapData) row (vector: int32[]) =
        assert (data.PixelFormat = bitmapFormat)
        assert (data.Width = vector.Length)
        let pointer = data |> physicalRowAddress row
        Marshal.Copy(vector, 0, pointer, vector.Length)
