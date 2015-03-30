#load "load-project.fsx"

open FsDither

let image = "flowers-large.jpg" |> Picture.load

image |> Picture.show "original"

image
|> Matrix.pmap Pixel.getL
|> FloydSteinberg.processLayer (Value.quantize 2)
|> Matrix.pmap Pixel.fromL
|> Picture.show "greyscale"

image
|> Picture.split
|> Triplet.map (FloydSteinberg.processLayer (Value.quantize 2))
|> Picture.join
|> Picture.show "color"