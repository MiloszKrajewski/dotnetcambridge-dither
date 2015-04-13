namespace FsDither

module Adhoc =
    let quickGrayscaleSlides algorithms picture =
        let greyscale = picture |> Picture.toGrayscale
        seq {
            for title, func in algorithms do
                let image = greyscale |> func |> Picture.fromGrayscale
                yield title, image
        } |> Picture.showMany
