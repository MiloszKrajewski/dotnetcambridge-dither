#load "init.fsx"
open FsDither

let quantize = Value.quantize 2

"flowers-large.jpg"
|> Picture.load
|> Adhoc.quickGrayscaleSlides [
    "none", id
    "treshold", Treshold.processLayer quantize
    "random", Random.processLayer 1.0 quantize
    "bayer8", Bayer.processLayer Bayer.bayer8x8 quantize
    "floyd", FloydSteinberg.processLayer quantize
]