' Services/GoogleAuthService.vb
Imports System.Windows.Forms
Imports System.IO
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Sheets.v4
Imports Google.Apis.Sheets.v4.Data
Imports Google.Apis.Services
Imports Google.Apis.Util.Store
Imports System.Threading

''' <summary>
''' Google Authentication სერვისი OAuth 2.0-ის გამოყენებით
''' </summary>
Public Class GoogleAuthService
    ' აპლიკაციის ID და საიდუმლო (მიიღებთ Google Cloud Console-დან)
    Private Shared ReadOnly ClientId As String = "თქვენი_კლიენტის_ID.apps.googleusercontent.com"
    Private Shared ReadOnly ClientSecret As String = "თქვენი_კლიენტის_საიდუმლო"

    ' აპლიკაციის სახელი
    Private Shared ReadOnly ApplicationName As String = "Scheduler_v8.6a"

    ' სფეროები რომელიც აუცილებელია აპლიკაციისთვის
    Private Shared ReadOnly Scopes() As String = {
        SheetsService.Scope.SpreadsheetsReadonly,  ' მხოლოდ კითხვა Google Sheets-დან
        "https://www.googleapis.com/auth/userinfo.email", ' მომხმარებლის ელფოსტაზე წვდომა
        "https://www.googleapis.com/auth/userinfo.profile" ' მომხმარებლის პროფილზე წვდომა
    }

    ' Sheets მონაცემთა ცხრილის ID (მომხმარებლების სიისთვის)
    Private Shared ReadOnly UserSpreadsheetId As String = "1SrBc4vLKPui6467aNmF5Hw-WZEd7dfGhkeFjfcnUqog"

    ' ტოკენების შესანახი საქაღალდე
    Private Shared ReadOnly TokenFolderPath As String = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Scheduler_v8.6a", "GoogleTokens")

    ''' <summary>
    ''' Google-ით ავტორიზაციის გავლა და UserCredential-ის მიღება
    ''' </summary>
    Public Shared Function Authenticate() As UserCredential
        Try
            ' ტოკენების საქაღალდის შექმნა თუ არ არსებობს
            If Not Directory.Exists(TokenFolderPath) Then
                Directory.CreateDirectory(TokenFolderPath)
            End If

            ' ClientSecrets ობიექტის შექმნა
            Dim clientSecrets = New ClientSecrets() With {
                .ClientId = ClientId,
                .ClientSecret = ClientSecret
            }

            ' აპლიკაციის ლოკალური პორტის მოსმენა OAuth callback-ისთვის
            Const LocalhostPort As Integer = 8888

            ' ავთენტიფიკაციის ნაკადის შექმნა
            Dim credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                Scopes,
                "user", ' მომხმარებლის ID
                CancellationToken.None,
                New FileDataStore(TokenFolderPath, True)).Result

            Return credential
        Catch ex As Exception
            MessageBox.Show($"ავტორიზაციის შეცდომა: {ex.Message}", "შეცდომა",
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' მომხმარებლის ელფოსტის მიღება UserCredential-დან
    ''' </summary>
    Public Shared Function GetUserEmail(credential As UserCredential) As String
        Try
            ' Google OAuth v2 სერვისის შექმნა
            Dim oauthService = New Google.Apis.Oauth2.v2.Oauth2Service(
                New BaseClientService.Initializer() With {
                    .HttpClientInitializer = credential,
                    .ApplicationName = ApplicationName
                })

            ' მომხმარებლის ინფორმაციის მიღება
            Dim userInfo = oauthService.Userinfo.Get().Execute()

            ' ელფოსტის დაბრუნება
            Return userInfo.Email
        Catch ex As Exception
            MessageBox.Show($"ელფოსტის მიღების შეცდომა: {ex.Message}", "შეცდომა",
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' მომხმარებლის სახელის მიღება UserCredential-დან
    ''' </summary>
    Public Shared Function GetUserName(credential As UserCredential) As String
        Try
            ' Google OAuth v2 სერვისის შექმნა
            Dim oauthService = New Google.Apis.Oauth2.v2.Oauth2Service(
                New BaseClientService.Initializer() With {
                    .HttpClientInitializer = credential,
                    .ApplicationName = ApplicationName
                })

            ' მომხმარებლის ინფორმაციის მიღება
            Dim userInfo = oauthService.Userinfo.Get().Execute()

            ' სახელის დაბრუნება
            Return userInfo.Name
        Catch ex As Exception
            MessageBox.Show($"სახელის მიღების შეცდომა: {ex.Message}", "შეცდომა",
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' მომხმარებლის როლის მიღება ცხრილიდან ელფოსტის მიხედვით
    ''' </summary>
    Public Shared Function GetUserRole(credential As UserCredential, email As String) As String
        Try
            ' Google Sheets სერვისის შექმნა
            Dim sheetsService = New SheetsService(New BaseClientService.Initializer() With {
                .HttpClientInitializer = credential,
                .ApplicationName = ApplicationName
            })

            ' მომხმარებლების ფურცლის წაკითხვა
            Dim range = "მომხმარებლები!A2:C100" ' ვიწყებთ მე-2 მწკრივიდან, რადგან პირველი სათაურია
            Dim request = sheetsService.Spreadsheets.Values.Get(UserSpreadsheetId, range)

            Dim response = request.Execute()
            Dim values = response.Values

            ' მომხმარებლის ძებნა ელფოსტის მიხედვით
            If values IsNot Nothing AndAlso values.Count > 0 Then
                For Each row In values
                    ' შევამოწმოთ, რომ მწკრივში საკმარისი ელემენტებია და პირველი ელემენტი (ელფოსტა) ემთხვევა
                    If row.Count >= 2 AndAlso
                       row(0).ToString().Trim().Equals(email, StringComparison.OrdinalIgnoreCase) Then
                        ' დავაბრუნოთ როლი (მე-2 ელემენტი)
                        Return row(1).ToString().Trim()
                    End If
                Next
            End If

            Return String.Empty ' მომხმარებელი ვერ მოიძებნა

        Catch ex As Exception
            MessageBox.Show($"როლის მიღების შეცდომა: {ex.Message}", "შეცდომა",
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return String.Empty
        End Try
    End Function
End Class