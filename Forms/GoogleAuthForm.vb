' Forms/GoogleAuthForm.vb
Imports System.Windows.Forms
Imports Google.Apis.Auth.OAuth2

''' <summary>
''' Google ავტორიზაციის ფორმა ბრაუზერის კომპონენტით
''' </summary>
Public Class GoogleAuthForm
    ' მომხმარებლის ავტორიზაციის კოლბეკი
    Public Event AuthenticationComplete(email As String, name As String, role As String)

    ' ბრაუზერის კომპონენტი
    Private browser As WebBrowser

    ' ავტორიზებული მომხმარებლის კრედენციალები
    Private credential As UserCredential = Nothing

    ''' <summary>
    ''' კონსტრუქტორი
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' კომპონენტების ინიციალიზაცია
    ''' </summary>
    Private Sub InitializeComponent()
        ' ფორმის თვისებები
        Me.Text = "Google-ით ავტორიზაცია"
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MinimizeBox = False

        ' ბრაუზერის კომპონენტის შექმნა
        browser = New WebBrowser()
        browser.Dock = DockStyle.Fill
        browser.ScriptErrorsSuppressed = True
        AddHandler browser.Navigated, AddressOf Browser_Navigated
        Me.Controls.Add(browser)

        ' ფორმის ჩატვირთვის ივენთის დამატება
        AddHandler Me.Load, AddressOf GoogleAuthForm_Load
    End Sub

    ''' <summary>
    ''' ფორმის ჩატვირთვის ივენთი
    ''' </summary>
    Private Sub GoogleAuthForm_Load(sender As Object, e As EventArgs)
        Try
            ' ავტორიზაციის პროცესის დაწყება
            Me.Cursor = Cursors.WaitCursor

            ' ავტორიზაციის ასინქრონულად გაშვება
            Dim authThread As New Threading.Thread(AddressOf StartAuthentication)
            authThread.SetApartmentState(Threading.ApartmentState.STA)
            authThread.Start()
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show($"ავტორიზაციის შეცდომა: {ex.Message}", "შეცდომა",
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' ავტორიზაციის პროცესის დაწყება ცალკე ნაკადში
    ''' </summary>
    Private Sub StartAuthentication()
        Try
            Me.Invoke(Sub()
                          Me.Cursor = Cursors.WaitCursor
                      End Sub)

            ' Google ავტორიზაციის სერვისის გამოძახება
            credential = GoogleAuthService.Authenticate()

            If credential IsNot Nothing Then
                ' ავტორიზაცია წარმატებულია, მივიღოთ ელფოსტა
                Dim email As String = GoogleAuthService.GetUserEmail(credential)

                If Not String.IsNullOrEmpty(email) Then
                    ' მივიღოთ მომხმარებლის სახელი
                    Dim name As String = GoogleAuthService.GetUserName(credential)

                    ' მივიღოთ მომხმარებლის როლი
                    Dim role As String = GoogleAuthService.GetUserRole(credential, email)

                    ' თუ როლი ვერ მოიძებნა, ე.ი. მომხმარებელი არ არის რეგისტრირებული
                    If String.IsNullOrEmpty(role) Then
                        Me.Invoke(Sub()
                                      Me.Cursor = Cursors.Default
                                      MessageBox.Show("თქვენი ანგარიში არ არის რეგისტრირებული სისტემაში. " &
                                                     "გთხოვთ დაუკავშირდეთ ადმინისტრატორს რეგისტრაციისთვის.",
                                                     "ავტორიზაციის შეცდომა",
                                                     MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                      Me.DialogResult = DialogResult.Cancel
                                      Me.Close()
                                  End Sub)
                        Return
                    End If

                    ' ყველაფერი წარმატებულია, გამოვიძახოთ კოლბეკი და დავხუროთ ფორმა
                    Me.Invoke(Sub()
                                  Me.Cursor = Cursors.Default
                                  RaiseEvent AuthenticationComplete(email, name, role)
                                  Me.DialogResult = DialogResult.OK
                                  Me.Close()
                              End Sub)
                Else
                    ' ვერ მოხერხდა ელფოსტის მიღება
                    Me.Invoke(Sub()
                                  Me.Cursor = Cursors.Default
                                  MessageBox.Show("ვერ მოხერხდა მომხმარებლის ელფოსტის მიღება. " &
                                                 "გთხოვთ სცადოთ თავიდან.", "ავტორიზაციის შეცდომა",
                                                 MessageBoxButtons.OK, MessageBoxIcon.Error)
                                  Me.DialogResult = DialogResult.Cancel
                                  Me.Close()
                              End Sub)
                End If
            Else
                ' ავტორიზაცია არ მოხერხდა
                Me.Invoke(Sub()
                              Me.Cursor = Cursors.Default
                              MessageBox.Show("ავტორიზაცია ვერ მოხერხდა. გთხოვთ სცადოთ თავიდან.",
                                             "ავტორიზაციის შეცდომა",
                                             MessageBoxButtons.OK, MessageBoxIcon.Error)
                              Me.DialogResult = DialogResult.Cancel
                              Me.Close()
                          End Sub)
            End If
        Catch ex As Exception
            Me.Invoke(Sub()
                          Me.Cursor = Cursors.Default
                          MessageBox.Show($"ავტორიზაციის შეცდომა: {ex.Message}", "შეცდომა",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error)
                          Me.DialogResult = DialogResult.Cancel
                          Me.Close()
                      End Sub)
        End Try
    End Sub

    ''' <summary>
    ''' ბრაუზერში URL-ის ცვლილების ივენთი
    ''' </summary>
    Private Sub Browser_Navigated(sender As Object, e As WebBrowserNavigatedEventArgs)
        ' აქ შეგვიძლია დავამატოთ ლოგიკა, რომელიც გააკონტროლებს ბრაუზერის ნავიგაციას
        ' მაგალითად, შეგვიძლია გავაჩეროთ ავტორიზაციის პროცესი ხელით, თუ მომხმარებელი
        ' გადავა კონკრეტულ URL-ზე
    End Sub
End Class