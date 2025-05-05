' Forms/Form1.vb
Imports System.Windows.Forms

Public Class Form1
    ' მენიუს მენეჯერი
    Private menuManager As MenuManager

    ' ავტორიზაციის ღილაკები
    Private btnLogin As Button
    Private btnLogout As Button
    Private lblUserInfo As Label

    ' მიმდინარე მომხმარებლის სტატუსი
    Private isAuthenticated As Boolean = False
    Private currentUserRole As String = ""
    Private currentUserEmail As String = ""
    Private currentUserName As String = ""

    ''' <summary>
    ''' ფორმის ჩატვირთვის ივენთ ჰენდლერი
    ''' </summary>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' ფორმის მახასიათებლების დაყენება
        Me.Text = "Scheduler_v8.6a - კლინიკის მართვის სისტემა"
        Me.Size = New Size(1000, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Icon = New System.Drawing.Icon(System.IO.Path.Combine(Application.StartupPath, "Resources\Images\AppPics\scheduler_logo3.ico"))
        'Me.Resources.Images.AppPics.shedulerlogo3.ico ' თუ გაქვთ აიკონი რესურსებში

        ' კომპონენტების ინიციალიზაცია
        InitializeComponents()
    End Sub

    ''' <summary>
    ''' ფორმის კომპონენტების ინიციალიზაცია
    ''' </summary>
    Private Sub InitializeComponents()
        ' მენიუს მენეჯერის ინიციალიზაცია
        menuManager = New MenuManager(Me)

        ' ავტორიზაციის ღილაკების ინიციალიზაცია
        InitializeLoginButtons()

        ' სტატუს ლეიბლის ინიციალიზაცია
        InitializeStatusLabel()

        ' აპლიკაციის დასახელების ლეიბლი
        Dim lblAppName As New Label()
        lblAppName.Text = "Scheduler v8.6a"
        lblAppName.Font = New Font("Segoe UI", 16, FontStyle.Bold)
        lblAppName.ForeColor = Color.DarkBlue
        lblAppName.AutoSize = True
        lblAppName.Location = New Point(20, 20)
        Me.Controls.Add(lblAppName)

        ' აპლიკაციის აღწერის ლეიბლი
        Dim lblAppDescription As New Label()
        lblAppDescription.Text = "კლინიკის ბენეფიციარების სეანსების აღრიცხვის სისტემა"
        lblAppDescription.Font = New Font("Segoe UI", 12)
        lblAppDescription.AutoSize = True
        lblAppDescription.Location = New Point(20, 50)
        Me.Controls.Add(lblAppDescription)

        ' დამატებითი კონტენტის პანელი (გამოჩნდება მხოლოდ ავტორიზაციის შემდეგ)
        Dim pnlContent As New Panel()
        pnlContent.Name = "pnlContent"
        pnlContent.Dock = DockStyle.Fill
        pnlContent.Padding = New Padding(20)
        pnlContent.Visible = False
        Me.Controls.Add(pnlContent)
    End Sub

    ''' <summary>
    ''' ავტორიზაციის ღილაკების ინიციალიზაცია
    ''' </summary>
    Private Sub InitializeLoginButtons()
        ' ავტორიზაციის ღილაკი
        btnLogin = New Button()
        btnLogin.Text = "Google-ით ავტორიზაცია"
        btnLogin.Size = New Size(200, 35)
        btnLogin.Location = New Point(Me.ClientSize.Width - 220, 30)
        btnLogin.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnLogin.BackColor = Color.FromArgb(66, 133, 244) ' Google-ის ფერი
        btnLogin.ForeColor = Color.White
        btnLogin.FlatStyle = FlatStyle.Flat
        btnLogin.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        btnLogin.Cursor = Cursors.Hand
        AddHandler btnLogin.Click, AddressOf btnLogin_Click
        Me.Controls.Add(btnLogin)

        ' გამოსვლის ღილაკი
        btnLogout = New Button()
        btnLogout.Text = "გამოსვლა"
        btnLogout.Size = New Size(120, 30)
        btnLogout.Location = New Point(Me.ClientSize.Width - 140, 70)
        btnLogout.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnLogout.BackColor = Color.LightGray
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.Cursor = Cursors.Hand
        btnLogout.Visible = False  ' თავდაპირველად დამალული
        AddHandler btnLogout.Click, AddressOf btnLogout_Click
        Me.Controls.Add(btnLogout)
    End Sub

    ''' <summary>
    ''' მომხმარებლის ინფორმაციის ლეიბლის ინიციალიზაცია
    ''' </summary>
    Private Sub InitializeStatusLabel()
        lblUserInfo = New Label()
        lblUserInfo.Text = "გთხოვთ გაიაროთ ავტორიზაცია"
        lblUserInfo.AutoSize = True
        lblUserInfo.Location = New Point(Me.ClientSize.Width - 220, 110)
        lblUserInfo.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        lblUserInfo.Font = New Font("Segoe UI", 9)
        Me.Controls.Add(lblUserInfo)
    End Sub

    ''' <summary>
    ''' ავტორიზაციის ღილაკზე დაჭერის ივენთ ჰენდლერი
    ''' </summary>
    Private Sub btnLogin_Click(sender As Object, e As EventArgs)
        Try
            ' Google-ის ავტორიზაციის ფორმის შექმნა
            Dim authForm As New GoogleAuthForm()

            ' ივენთის დამატება
            AddHandler authForm.AuthenticationComplete, AddressOf GoogleAuth_AuthenticationComplete

            ' ფორმის ჩვენება
            authForm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show("ავტორიზაციის შეცდომა: " & ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' ივენთ ჰენდლერი Google-ის ავტორიზაციის დასრულებისთვის
    ''' </summary>
    Private Sub GoogleAuth_AuthenticationComplete(sender As Object, e As GoogleAuthForm.AuthCompletedEventArgs)
        If e.IsAuthenticated Then
            ' მომხმარებლის ავტორიზებულად მონიშვნა
            isAuthenticated = True
            currentUserEmail = e.UserEmail
            currentUserName = e.UserName
            currentUserRole = e.UserRole

            ' მომხმარებლის ინფორმაციის განახლება
            UpdateUserInfo()

            ' მენიუს განახლება როლის მიხედვით
            menuManager.ShowMenuByRole(currentUserRole)

            ' ღილაკების ხილვადობის განახლება
            btnLogin.Visible = False
            btnLogout.Visible = True

            ' შეტყობინების გამოჩენა
            MessageBox.Show("ავტორიზაცია წარმატებით დასრულდა!", "ავტორიზაცია", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' კონტენტის პანელის გამოჩენა
            Dim contentPanel As Panel = DirectCast(Me.Controls("pnlContent"), Panel)
            contentPanel.Visible = True

            ' საწყისი გვერდის ჩატვირთვა
            LoadHomePage()
        Else
            MessageBox.Show("ავტორიზაცია ვერ მოხერხდა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ''' <summary>
    ''' გამოსვლის ღილაკზე დაჭერის ივენთ ჰენდლერი
    ''' </summary>
    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        Try
            ' ავტორიზაციის სერვისის შექმნა
            Dim authService As New GoogleAuthService()

            ' სისტემიდან გამოსვლა
            If authService.Logout() Then
                ' მომხმარებლის სტატუსის განახლება
                isAuthenticated = False
                currentUserEmail = ""
                currentUserName = ""
                currentUserRole = ""

                ' ინფორმაციის განახლება ეკრანზე
                UpdateUserInfo()

                ' მენიუს განახლება
                menuManager.ShowOnlyHomeMenu()

                ' ღილაკების ხილვადობის განახლება
                btnLogin.Visible = True
                btnLogout.Visible = False

                ' კონტენტის პანელის დამალვა
                Dim contentPanel As Panel = DirectCast(Me.Controls("pnlContent"), Panel)
                contentPanel.Visible = False

                MessageBox.Show("თქვენ წარმატებით გამოხვედით სისტემიდან.", "გამოსვლა", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("სისტემიდან გამოსვლის შეცდომა: " & ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' მომხმარებლის ინფორმაციის განახლება ეკრანზე
    ''' </summary>
    Private Sub UpdateUserInfo()
        If isAuthenticated Then
            lblUserInfo.Text = $"მომხმარებელი: {currentUserName}{Environment.NewLine}როლი: {currentUserRole}"
        Else
            lblUserInfo.Text = "გთხოვთ გაიაროთ ავტორიზაცია"
        End If
    End Sub

    ''' <summary>
    ''' საწყისი გვერდის ჩატვირთვა
    ''' </summary>
    Private Sub LoadHomePage()
        ' აქ შეგიძლიათ ჩატვირთოთ საწყისი გვერდის კონტენტი
        ' მაგალითად, დღევანდელი განრიგი, შეხსენებები და ა.შ.
        Dim contentPanel As Panel = DirectCast(Me.Controls("pnlContent"), Panel)
        contentPanel.Controls.Clear()

        ' მაგალითად, დავამატოთ მისალმების ლეიბლი
        Dim lblWelcome As New Label()
        lblWelcome.Text = $"მოგესალმებით, {currentUserName}!"
        lblWelcome.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        lblWelcome.AutoSize = True
        lblWelcome.Location = New Point(20, 20)
        contentPanel.Controls.Add(lblWelcome)

        ' აქ შეგიძლიათ დაამატოთ ნებისმიერი კონტენტი, მაგალითად,
        ' დაგეგმილი სეანსების ცხრილი, შეხსენებები და ა.შ.

        ' ცხრილის მაგალითი
        Dim dgvAppointments As New DataGridView()
        dgvAppointments.Name = "dgvAppointments"
        dgvAppointments.Location = New Point(20, 60)
        dgvAppointments.Size = New Size(contentPanel.Width - 40, 300)
        dgvAppointments.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        dgvAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvAppointments.AllowUserToAddRows = False
        dgvAppointments.AllowUserToDeleteRows = False
        dgvAppointments.AllowUserToOrderColumns = True
        dgvAppointments.ReadOnly = True
        dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvAppointments.RowHeadersVisible = False
        dgvAppointments.MultiSelect = False

        ' ცხრილის სვეტების დამატება
        dgvAppointments.Columns.Add("AppointmentID", "ID")
        dgvAppointments.Columns.Add("Date", "თარიღი")
        dgvAppointments.Columns.Add("Time", "დრო")
        dgvAppointments.Columns.Add("Client", "კლიენტი")
        dgvAppointments.Columns.Add("Therapist", "თერაპევტი")
        dgvAppointments.Columns.Add("Status", "სტატუსი")

        ' ნიმუშის მონაცემების დამატება
        dgvAppointments.Rows.Add("1", "06/05/2025", "10:00", "გიორგი გიორგაძე", "ნინო ნინიძე", "დაგეგმილი")
        dgvAppointments.Rows.Add("2", "06/05/2025", "11:00", "ანა ანანიძე", "მერაბ მერაბიშვილი", "დაგეგმილი")
        dgvAppointments.Rows.Add("3", "06/05/2025", "14:00", "დავით დავითაძე", "ნინო ნინიძე", "დაგეგმილი")
        dgvAppointments.Rows.Add("4", "07/05/2025", "09:30", "ლელა ლელაშვილი", "მერაბ მერაბიშვილი", "დაგეგმილი")
        dgvAppointments.Rows.Add("5", "07/05/2025", "16:45", "სალომე სალომონიძე", "ნინო ნინიძე", "დაგეგმილი")

        contentPanel.Controls.Add(dgvAppointments)

        ' შეხსენებების პანელი
        Dim pnlReminders As New Panel()
        pnlReminders.BorderStyle = BorderStyle.FixedSingle
        pnlReminders.Location = New Point(20, 380)
        pnlReminders.Size = New Size(contentPanel.Width - 40, 150)
        pnlReminders.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right

        ' შეხსენებების სათაური
        Dim lblReminders As New Label()
        lblReminders.Text = "შეხსენებები"
        lblReminders.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        lblReminders.AutoSize = True
        lblReminders.Location = New Point(10, 10)
        pnlReminders.Controls.Add(lblReminders)

        ' შეხსენებების სია
        Dim lstReminders As New ListBox()
        lstReminders.Location = New Point(10, 40)
        lstReminders.Size = New Size(pnlReminders.Width - 20, 100)
        lstReminders.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        lstReminders.Items.Add("დღეს 15:00-ზე საერთო შეხვედრა")
        lstReminders.Items.Add("ხვალ 10:00-ზე ბენეფიციარების განხილვა")
        lstReminders.Items.Add("07/05/2025 შეავსეთ კვირის ანგარიშები")

        pnlReminders.Controls.Add(lstReminders)
        contentPanel.Controls.Add(pnlReminders)
    End Sub

    ''' <summary>
    ''' ფანჯრის ზომის შეცვლის ივენთ ჰენდლერი
    ''' </summary>
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ' აქ შეგიძლიათ დაამატოთ ლოგიკა, თუ საჭიროა კონტროლების ზომის/პოზიციის შეცვლა
        ' მაგალითად, თუ გსურთ ცხრილების ზომების ავტომატური მორგება ფანჯრის ზომაზე
    End Sub

    ''' <summary>
    ''' ფორმის დახურვის ივენთ ჰენდლერი
    ''' </summary>
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' დახურვის დადასტურება
        If MessageBox.Show("ნამდვილად გსურთ აპლიკაციის დახურვა?", "დახურვა",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            e.Cancel = True
        End If
    End Sub
End Class