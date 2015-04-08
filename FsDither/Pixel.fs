namespace FsDither

module Value =
    type Value = float

    let quantize n = 
        let q = 1.0 / float (n - 1) 
        fun v -> round (v / q) * q |> max 0.0 |> min 1.0
    let inline toByte v = (v |> min 1.0 |> max 0.0) * 255.0 |> int
    let inline fromByte v = (v &&& 0xFF |> float) / 255.0

    let toAsciiHalftone =
        let shades = [| '$'; 'B'; 'Q'; 'Y'; 'v'; '~'; '.'; ' ' |]
        fun v -> shades.[(v |> max 0.0 |> min 1.0) * 7.0 |> round |> int]

module Pixel =
    open Value

    [<Struct>]
    type Pixel = 
        val r: Value
        val g: Value
        val b: Value
        new(r, g, b) = { r = r; g = g; b = b }
        new((r, g, b)) = { r = r; g = g; b = b }
        new(l) = { r = l; g = l; b = l }

    let inline toLogicalPixel physical =
        let inline toValue o v = v >>> o |> Value.fromByte
        Pixel(toValue 16 physical, toValue 8 physical, toValue 0 physical)

    let inline toPhysicalPixel (logical: Pixel) =
        let inline toByte o v = v |> Value.toByte <<< o
        (0xFF <<< 24) ||| (toByte 16 logical.r) ||| (toByte 8 logical.g) ||| (toByte 0 logical.b)

    let inline fromL (l: Value) = Pixel(l)

    let inline getR (pixel: Pixel) = pixel.r
    let inline getG (pixel: Pixel) = pixel.g
    let inline getB (pixel: Pixel) = pixel.b
    let inline getL (pixel: Pixel) = 0.2126*pixel.r + 0.7152*pixel.g + 0.0722*pixel.b


