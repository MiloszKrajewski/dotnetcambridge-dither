namespace FsDither

module UI =
    open System.Drawing
    open System.Windows.Forms
    open System.Threading

    type ViewerForm() as form =
        inherit Form(TopMost = true)

        let viewer = 
            new PictureBox(
                Dock = DockStyle.Fill, 
                SizeMode = PictureBoxSizeMode.Zoom)

        do form.Controls.Add(viewer)

        member form.LoadImage (title, image) = 
            form.Text <- title
            viewer.Image <- image
            form.AdjustSize image.Size

        member form.NextImage =
            form.KeyPress
            |> Event.filter (fun e -> e.KeyChar = ' ')
            |> Event.map (fun _ -> form)

        member private form.AdjustSize clientSize =
            let inline sizeDiff (a: Size) (b: Size) = 
                Size(a.Width - b.Width, a.Height - b.Height)
            let inline sizeRatios (a: Size) (b: Size) =
                float a.Width / float b.Width, float a.Height / float b.Height

            let screenRect = Screen.FromControl(form).WorkingArea
            let formMargin = sizeDiff form.Size form.ClientSize
            let maximumClientSize = sizeDiff screenRect.Size formMargin
            let ratioX, ratioY = sizeRatios maximumClientSize clientSize
            let ratio = min ratioX ratioY
            let clientX = float clientSize.Width * ratio |> round |> int
            let clientY = float clientSize.Height * ratio |> round |> int
            let originX = (screenRect.Right - clientX - formMargin.Width) / 2
            let originY = (screenRect.Bottom - clientY - formMargin.Height) / 2

            form.SetBounds(
                originX, originY, 
                clientX + formMargin.Width, clientY + formMargin.Height)

    #if CONSOLE
    let private showForm factory = 
        let action () = Application.Run(factory () :> Form)
        Thread(action, IsBackground = true).Start()
    #else
    let private showForm factory = (factory () :> Form).Show()
    #endif

    let private showViewer setup =
        let factory () = new ViewerForm() |> apply setup
        showForm factory

    let showOne title image =
        showViewer (fun form ->
            form.NextImage |> Event.add (fun _ -> form.Close())
            form.LoadImage(title, image)
        )

    let showMany (images: (string * #Image) seq) =
        let images = images.GetEnumerator()
        showViewer (fun form ->
            let nextImage () = 
                match images.MoveNext() with
                | true -> images.Current |> form.LoadImage
                | _ -> form.Close()
            form.NextImage |> Event.add (fun _ -> nextImage ())
            nextImage ()
        )
