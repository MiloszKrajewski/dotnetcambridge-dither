#load "load-project.fsx"

open FsDither
"lena.jpg" |> Bitmap.load |> UI.showOne "lena"
["lena.jpg"; "david.jpg"] |> List.iter (fun fn -> fn |> Bitmap.load |> UI.showOne fn)
