' GoogleLoginForm.vb

Imports System.Windows.Forms

''' <summary>
''' Google-ით ავტორიზაციის ფორმა
''' </summary>
Public Class GoogleLoginForm
    ' კონტროლები
    Private lblTitle As Label
    Private lblEmail As Label
    Private txtEmail As TextBox
    Private btnLogin As Button
    Private btnCancel As Button

    ' კოლბეკი წარმატებული ავტორიზაციისთვის - გავასწორეთ კოლბეკის სიგნატურა
    Private authSuccessCallback As Action(Of String, String, String)

    ''' <summary>
    ''' კონსტრუქტორი
    ''' </summary>
    ''' <param name="callback">წარმატებული ავტორიზაციის შემთხვევაში გამოძახებული მეთოდი</param>
    Public Sub New(callback As Action(Of String, String, String))
        ' ფორმის ინიციალიზაცია
        Me.authSuccessCallback = callback
        Me.Text = "Google-ით ავტორიზაცია"
        Me.Size = New Size(400, 250)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.FormBorderStyle = FormBorderStyle.FixedDialog

        ' სათაურის ლეიბლი
        lblTitle = New Label()
        lblTitle.Text = "კლინიკის მართვის სისტემაში შესვლა"
        lblTitle.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        lblTitle.AutoSize = True
        lblTitle.Location = New Point(20, 20)
        Me.Controls.Add(lblTitle)

        ' ელფოსტის ლეიბლი
        lblEmail = New Label()
        lblEmail.Text = "შეიყვანეთ თქვენი Google-ის ელფოსტა:"
        lblEmail.Font = New Font("Segoe UI", 10)
        lblEmail.AutoSize = True
        lblEmail.Location = New Point(20, 70)
        Me.Controls.Add(lblEmail)

        ' ელფოსტის ტექსტბოქსი
        txtEmail = New TextBox()
        txtEmail.Size = New Size(340, 30)
        txtEmail.Location = New Point(20, 100)
        txtEmail.Font = New Font("Segoe UI", 10)
        Me.Controls.Add(txtEmail)

        ' შესვლის ღილაკი
        btnLogin = New Button()
        btnLogin.Text = "შესვლა"
        btnLogin.Size = New Size(120, 35)
        btnLogin.Location = New Point(130, 150)
        btnLogin.BackColor = Color.FromArgb(66, 133, 244) ' Google-ის ფერი
        btnLogin.ForeColor = Color.White
        btnLogin.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        btnLogin.FlatStyle = FlatStyle.Flat
        AddHandler btnLogin.Click, AddressOf BtnLogin_Click
        Me.Controls.Add(btnLogin)

        ' გაუქმების ღილაკი
        btnCancel = New Button()
        btnCancel.Text = "გაუქმება"
        btnCancel.Size = New Size(100, 35)
        btnCancel.Location = New Point(260, 150)
        btnCancel.Font = New Font("Segoe UI", 10)
        AddHandler btnCancel.Click, AddressOf BtnCancel_Click
        Me.Controls.Add(btnCancel)

        ' Enter ღილაკით ავტორიზაცია
        Me.AcceptButton = btnLogin
    End Sub

    ''' <summary>
    ''' შესვლის ღილაკის კლიკის ჰენდლერი
    ''' </summary>
    Private Sub BtnLogin_Click(sender As Object, e As EventArgs)
        ' ველების შემოწმება
        If String.IsNullOrWhiteSpace(txtEmail.Text) Then
            MessageBox.Show("გთხოვთ შეიყვანოთ ელფოსტა", "შეცდომა",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' ელფოსტის ფორმატის შემოწმება
        If Not IsValidEmail(txtEmail.Text) Then
            MessageBox.Show("გთხოვთ შეიყვანოთ სწორი ფორმატის Google-ის ელფოსტა",
                            "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' ავტორიზაციის მცდელობა
        Me.Cursor = Cursors.WaitCursor
        btnLogin.Enabled = False

        Try
            ' მომხმარებლის ავტორიზაცია
            Dim userInfo = GoogleSheetsService.AuthorizeUser(txtEmail.Text)

            If userInfo IsNot Nothing Then
                ' წარმატებული ავტორიზაცია
                Dim name As String = userInfo.Item1
                Dim role As String = userInfo.Item2

                Me.DialogResult = DialogResult.OK
                Me.Close()

                ' კოლბეკის გამოძახება
                authSuccessCallback(txtEmail.Text, name, role)
            Else
                ' მომხმარებელი ვერ მოიძებნა
                MessageBox.Show("მომხმარებელი ამ ელფოსტით ვერ მოიძებნა სისტემაში. " &
                                "გთხოვთ დარწმუნდეთ, რომ სწორად შეიყვანეთ ელფოსტა, " &
                                "ან დაუკავშირდეთ ადმინისტრატორს ანგარიშის შესაქმნელად.",
                                "ავტორიზაციის შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                btnLogin.Enabled = True
            End If
        Catch ex As Exception
            MessageBox.Show($"ავტორიზაციის შეცდომა: {ex.Message}", "შეცდომა",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            btnLogin.Enabled = True
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' გაუქმების ღილაკის კლიკის ჰენდლერი
    ''' </summary>
    Private Sub BtnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    ''' <summary>
    ''' ელფოსტის ფორმატის შემოწმება
    ''' </summary>
    Private Function IsValidEmail(email As String) As Boolean
        Try
            Dim addr = New System.Net.Mail.MailAddress(email)
            Return addr.Address = email
        Catch
            Return False
        End Try
    End Function
End Class