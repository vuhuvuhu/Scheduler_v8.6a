Imports System.Threading.Tasks
Imports System.Windows.Forms

Public Class GoogleAuthForm
    ' ივენთის განსაზღვრა ავტორიზაციის დასრულებისთვის
    Public Event AuthenticationComplete(ByVal sender As Object, ByVal e As AuthCompletedEventArgs)

    ' ივენთ არგუმენტების კლასი
    Public Class AuthCompletedEventArgs
        Inherits EventArgs

        Public Property IsAuthenticated As Boolean
        Public Property UserEmail As String
        Public Property UserName As String
        Public Property UserRole As String

        Public Sub New(isAuth As Boolean, email As String, name As String, role As String)
            IsAuthenticated = isAuth
            UserEmail = email
            UserName = name
            UserRole = role
        End Sub
    End Class

    ' კონტროლების განცხადება
    Private WithEvents btnLogin As Button
    Private WithEvents btnCancel As Button
    Private lblTitle As Label
    Private lblDescription As Label
    Private lblStatus As Label
    Private progressBar As ProgressBar
    Private pnlHeader As Panel
    Private pnlFooter As Panel
    Private pbGoogleLogo As PictureBox

    ' ავტორიზაციის სერვისი
    Private authService As New GoogleAuthService()

    ' ფორმის კონსტრუქტორი
    Public Sub New()
        ' ეს გამოძახება მოითხოვება დიზაინერის კომპონენტისთვის
        InitializeComponent()

        ' სხვა ინიციალიზაცია
        SetupForm()
    End Sub

    ' ფორმის დამატებითი კონფიგურაცია
    Private Sub SetupForm()
        Me.Text = "Google-ით ავტორიზაცია"
        Me.Size = New Size(450, 300)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        ' ჰედერის პანელი
        pnlHeader = New Panel()
        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Height = 90
        Me.Controls.Add(pnlHeader)

        ' Google-ის ლოგო
        pbGoogleLogo = New PictureBox()
        pbGoogleLogo.Size = New Size(50, 50)
        pbGoogleLogo.Location = New Point(20, 20)
        pbGoogleLogo.SizeMode = PictureBoxSizeMode.StretchImage
        ' აქ უნდა ჩაიტვირთოს Google-ის ლოგო თუ გაქვთ
        pnlHeader.Controls.Add(pbGoogleLogo)

        ' სათაური
        lblTitle = New Label()
        lblTitle.Text = "შედით სისტემაში Google-ის ანგარიშით"
        lblTitle.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        lblTitle.AutoSize = True
        lblTitle.Location = New Point(80, 20)
        pnlHeader.Controls.Add(lblTitle)

        ' აღწერა
        lblDescription = New Label()
        lblDescription.Text = "დააჭირეთ ღილაკს Google-ის ავტორიზაციისთვის"
        lblDescription.AutoSize = True
        lblDescription.Location = New Point(80, 50)
        pnlHeader.Controls.Add(lblDescription)

        ' სტატუსის ლეიბლი
        lblStatus = New Label()
        lblStatus.Text = "მზადაა ავტორიზაციისთვის"
        lblStatus.AutoSize = True
        lblStatus.Location = New Point(150, 120)
        Me.Controls.Add(lblStatus)

        ' პროგრეს ბარი
        progressBar = New ProgressBar()
        progressBar.Location = New Point(80, 150)
        progressBar.Width = 300
        progressBar.Visible = False
        Me.Controls.Add(progressBar)

        ' ფუტერის პანელი
        pnlFooter = New Panel()
        pnlFooter.Dock = DockStyle.Bottom
        pnlFooter.Height = 70
        Me.Controls.Add(pnlFooter)

        ' ღილაკები
        btnLogin = New Button()
        btnLogin.Text = "Google-ით შესვლა"
        btnLogin.Size = New Size(150, 35)
        btnLogin.Location = New Point(125, 20)
        btnLogin.BackColor = Color.FromArgb(66, 133, 244)
        btnLogin.ForeColor = Color.White
        btnLogin.FlatStyle = FlatStyle.Flat
        pnlFooter.Controls.Add(btnLogin)

        btnCancel = New Button()
        btnCancel.Text = "გაუქმება"
        btnCancel.Size = New Size(100, 35)
        btnCancel.Location = New Point(295, 20)
        btnCancel.Visible = False
        pnlFooter.Controls.Add(btnCancel)
    End Sub

    ' ავტორიზაციის ღილაკზე დაჭერის ივენთ ჰენდლერი
    Private Async Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Try
            ' UI განახლება
            lblStatus.Text = "მიმდინარეობს ავტორიზაცია..."
            progressBar.Visible = True
            btnLogin.Enabled = False
            btnCancel.Visible = True

            ' ავტორიზაციის პროცესის გაშვება ასინქრონულად
            Dim userInfo As Dictionary(Of String, String) = Await authService.AuthenticateUserAsync()

            If userInfo IsNot Nothing AndAlso userInfo.ContainsKey("isAuthenticated") AndAlso userInfo("isAuthenticated") = "true" Then
                ' ივენთის გამოძახება
                RaiseEvent AuthenticationComplete(Me, New AuthCompletedEventArgs(
                    True,
                    userInfo("email"),
                    userInfo("name"),
                    userInfo("role")))

                ' დიალოგის დახურვა
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                ' ავტორიზაცია ვერ მოხერხდა
                lblStatus.Text = "ავტორიზაცია ვერ მოხერხდა. გთხოვთ სცადოთ თავიდან."
                progressBar.Visible = False
                btnLogin.Enabled = True
                btnCancel.Visible = False
            End If
        Catch ex As Exception
            MessageBox.Show("ავტორიზაციის შეცდომა: " & ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)

            ' UI განახლება
            lblStatus.Text = "ავტორიზაცია ვერ მოხერხდა: " & ex.Message
            progressBar.Visible = False
            btnLogin.Enabled = True
            btnCancel.Visible = False
        End Try
    End Sub

    ' გაუქმების ღილაკზე დაჭერის ივენთ ჰენდლერი
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class