namespace FsDither

module UI =
    open System.Windows.Forms
    open System.Threading
    open System.Drawing
    open System

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
            let screenRect = Screen.FromControl(form).WorkingArea
            let formMargin = Size(form.Width - form.ClientSize.Width, form.Height - form.ClientSize.Height)
            let maximumClientSize = Size(screenRect.Width - formMargin.Width, screenRect.Height - formMargin.Height)

            let ratioX = float maximumClientSize.Width / float clientSize.Width
            let ratioY = float maximumClientSize.Height / float clientSize.Height
            let ratio = min ratioX ratioY
            let clientX = float clientSize.Width * ratio |> int
            let clientY = float clientSize.Height * ratio |> int
            let originX = (screenRect.Left + screenRect.Width - clientX - formMargin.Width) / 2
            let originY = (screenRect.Top + screenRect.Height - clientY - formMargin.Height) / 2

            form.SetBounds(originX, originY, clientX + formMargin.Width, clientY + formMargin.Height)

    let private show (setup: ViewerForm -> unit) =
        let action () = 
            let form = new ViewerForm()
            setup form
            Application.Run(form)
        let thread = Thread(action, IsBackground = true)
        thread.Start()

    let showOne title image =
        show (fun f ->
            f.NextImage |> Event.add (fun _ -> f.Close())
            f.LoadImage(title, image)
        )

    let showMany (images: (string * #Image) seq) =
        let images = images.GetEnumerator()
        show (fun f ->
            let nextImage () = 
                match images.MoveNext() with
                | true -> images.Current |> f.LoadImage
                | _ -> f.Close()
            f.NextImage |> Event.add (fun _ -> nextImage ())
            nextImage ()
        )
