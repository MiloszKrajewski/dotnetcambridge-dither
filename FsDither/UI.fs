namespace FsDither

module UI =
    open System.Windows.Forms
    open System.Threading
    open System.Drawing

    let private showForm (factory: unit -> Form) = 
        #if INTERACTIVE
        let form = factory ()
        form.Show()
        #else
        let thread = Thread(fun () -> 
            let form = factory ()
            Application.Run(form)
        )
        thread.IsBackground <- true
        thread.Start()
        #endif

    let private adjustClientSize (form: Form) (clientSize: Size) =
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

    let private createForm title =
        let form = new Form(TopMost = true, Text = title)
        let viewer = 
            new PictureBox(
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom)
        form.Controls.Add(viewer)

        let load (image: Image) =
            adjustClientSize form image.Size
            viewer.Image <- image

        form, load

    let show title (image: Image) =
        showForm (fun () -> 
            let form, load = createForm title
            load image
            form
        )

    let slideShow title (images: #Image seq) =
        showForm (fun () -> 
            let form, load = createForm title
            let images = images.GetEnumerator()

            let nextImage () =
                match images.MoveNext() with
                | true -> images.Current |> load
                | _ -> form.Close()

            form.KeyPress 
            |> Event.filter (fun e -> e.KeyChar = ' ')
            |> Event.add (fun _ -> nextImage ())

            form.KeyPress
            |> Event.filter (fun e -> e.KeyChar = char Keys.Escape)
            |> Event.add (fun _ -> form.Close())

            nextImage ()

            form
        )
