#load "init.fsx"
open FsDither

let image = "flowers-large.jpg" |> Picture.load 
let quant = Value.quantize 4
let floyd = FloydSteinberg.processLayer quant

image
|> Picture.split
|> Debug.timeit "map" (Triplet.map floyd)
|> Picture.join
|> Picture.showOne "map"

image
|> Picture.split
|> Debug.timeit "pmap" (Triplet.pmap floyd)
|> Picture.join
|> Picture.showOne "pmap"
