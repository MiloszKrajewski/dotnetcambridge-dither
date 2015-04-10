#load "load-project.fsx"
open FsDither

"lena.jpg" |> Bitmap.load |> UI.showOne "lena"
["lena.jpg"; "david.jpg"] |> Seq.iter (fun fn -> fn |> Bitmap.load |> UI.showOne fn)
["lena.jpg"; "david.jpg"] |> Seq.map (fun fn -> (fn, fn |> Bitmap.load)) |> UI.showMany
