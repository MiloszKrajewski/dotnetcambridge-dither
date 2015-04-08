#load "load-project.fsx"

open FsDither

let image = "flowers-large.jpg" |> Picture.load

image |> Picture.showOne "original"

image
|> Matrix.pmap Pixel.getL
|> FloydSteinberg.processLayer (Value.quantize 2)
|> Matrix.pmap Pixel.fromL
|> Picture.showOne "greyscale"

image
|> Picture.split
|> Triplet.map (FloydSteinberg.processLayer (Value.quantize 2))
|> Picture.join
|> Picture.showOne "color"