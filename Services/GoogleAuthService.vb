Imports System
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Windows.Forms
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Sheets.v4
Imports Google.Apis.Sheets.v4.Data
Imports Google.Apis.Services
Imports Google.Apis.Util.Store

Public Class GoogleAuthService
    ' OAuth 2.0 კლიენტის კონფიგურაცია
    Private Const CLIENT_ID As String = "433406353051u43b28977gmpr7h58be326g3tgcsne.apps.googleusercontent.com"
    Private Const CLIENT_SECRET As String = "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"
    Private ReadOnly SCOPES As String() = {SheetsService.Scope.Spreadsheets, SheetsService.Scope.Drive}

    ' სერვის აკაუნტის კონფიგურაცია
    Private Const SERVICE_ACCOUNT_EMAIL As String = "exceltosheetsservice@sinuous-pact-454212-m3.iam.gserviceaccount.com"

    ' კონფიგურაციის ფაილების მდებარეობა
    Private Const TOKEN_PATH As String = "token.json"

    ' Google Sheets სერვისი
    Private sheetsService As SheetsService = Nothing

    ' მომხმარებლის ინფორმაცია
    Private userInfo As Dictionary(Of String, String) = Nothing

    ' კონსტრუქტორი
    Public Sub New()
        ' ინიციალიზაცია
    End Sub

    ' მომხმარებლის ავტორიზაცია OAuth2-ით
    Public Async Function AuthenticateUserAsync() As Task(Of Dictionary(Of String, String))
        Try
            ' OAuth 2.0 Flow ინიციალიზაცია
            Dim clientSecrets = New ClientSecrets() With {
                .ClientId = CLIENT_ID,
                .ClientSecret = CLIENT_SECRET
            }

            ' შევამოწმოთ ტოკენის ფაილი
            Dim tokenExists As Boolean = File.Exists(TOKEN_PATH)

            ' ვცადოთ არსებული ტოკენის გამოყენება
            Dim credential As UserCredential

            If tokenExists Then
                Try
                    ' არსებული ტოკენის გამოყენება
                    credential = Await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        clientSecrets,
                        SCOPES,
                        "user",
                        CancellationToken.None,
                        New FileDataStore(TOKEN_PATH, True))

                    ' შევქმნათ Sheets სერვისი
                    sheetsService = New SheetsService(New BaseClientService.Initializer() With {
                        .HttpClientInitializer = credential,
                        .ApplicationName = "Scheduler v8.6a"
                    })

                    ' მომხმარებლის ინფორმაციის ამოღება
                    userInfo = GetUserInfoFromCredential(credential)
                    Return userInfo
                Catch ex As Exception
                    ' ტოკენთან დაკავშირებული პრობლემაა, წავშალოთ და თავიდან დავიწყოთ
                    If File.Exists(TOKEN_PATH) Then
                        File.Delete(TOKEN_PATH)
                    End If
                End Try
            End If

            ' ახალი ავტორიზაციის დაწყება
            Dim result As DialogResult = MessageBox.Show(
                "ავტორიზაციისთვის გაიხსნება ბრაუზერი Google-ის ავტორიზაციის გვერდით. გსურთ გაგრძელება?",
                "Google-ით ავტორიზაცია",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

            If result = DialogResult.No Then
                Return Nothing
            End If

            ' ახალი ავტორიზაციის ფლოუ
            credential = Await GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                SCOPES,
                "user",
                CancellationToken.None,
                New FileDataStore(TOKEN_PATH, True))

            ' Sheets სერვისის შექმნა
            sheetsService = New SheetsService(New BaseClientService.Initializer() With {
                .HttpClientInitializer = credential,
                .ApplicationName = "Scheduler v8.6a"
            })

            ' მომხმარებლის ინფორმაციის ამოღება
            userInfo = GetUserInfoFromCredential(credential)
            Return userInfo
        Catch ex As Exception
            MessageBox.Show("ავტორიზაციის შეცდომა: " & ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    ' მომხმარებლის ინფორმაციის ამოღება ავტორიზაციის კრედენციალებიდან
    Private Function GetUserInfoFromCredential(credential As UserCredential) As Dictionary(Of String, String)
        Try
            ' სიმულაცია - რეალურ აპლიკაციაში აქ გამოიყენებთ Google People API-ს
            ' მომხმარებლის ინფორმაციის მისაღებად
            Dim info As New Dictionary(Of String, String)()
            info.Add("email", "sakhokia@gmail.com") ' თქვენი რეალური ელ-ფოსტა
            info.Add("name", "ვახტანგ სახოკია") ' თქვენი რეალური სახელი
            info.Add("role", "admin") ' დეფოლტ როლი - თქვენს აპლიკაციაში ეს შეიძლება განისაზღვროს ლოგიკით
            info.Add("isAuthenticated", "true")

            Return info
        Catch ex As Exception
            MessageBox.Show("მომხმარებლის ინფორმაციის მიღების შეცდომა: " & ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    ' Google Sheets-დან მონაცემების წამოღება
    Public Function GetSheetData(spreadsheetId As String, range As String) As List(Of List(Of String))
        Try
            If sheetsService Is Nothing Then
                Throw New Exception("ჯერ უნდა გაიაროთ ავტორიზაცია")
            End If

            ' მოთხოვნის მომზადება
            Dim request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range)

            ' მოთხოვნის გაგზავნა
            Dim response = request.Execute()
            Dim values = response.Values

            ' შედეგების გარდაქმნა
            Dim result As New List(Of List(Of String))

            If values IsNot Nothing AndAlso values.Count > 0 Then
                For Each row In values
                    Dim rowData As New List(Of String)
                    For Each cell In row
                        rowData.Add(cell.ToString())
                    Next
                    result.Add(rowData)
                Next
            End If

            Return result
        Catch ex As Exception
            MessageBox.Show("Google Sheets-დან მონაცემების მიღების შეცდომა: " & ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return New List(Of List(Of String))
        End Try
    End Function

    ' Google Sheets-ში მონაცემების ჩაწერა
    Public Function UpdateSheetData(spreadsheetId As String, range As String, values As List(Of List(Of Object))) As Boolean
        Try
            If sheetsService Is Nothing Then
                Throw New Exception("ჯერ უნდა გაიაროთ ავტორიზაცია")
            End If

            ' მონაცემების მომზადება
            Dim valueRange As New ValueRange()
            valueRange.Values = New List(Of IList(Of Object))()

            For Each row In values
                Dim rowData As IList(Of Object) = New List(Of Object)()
                For Each cell In row
                    rowData.Add(cell)
                Next
                valueRange.Values.Add(rowData)
            Next

            ' მოთხოვნის მომზადება
            Dim updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range)
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED

            ' მოთხოვნის გაგზავნა
            Dim response = updateRequest.Execute()

            Return response.UpdatedCells.HasValue AndAlso response.UpdatedCells.Value > 0
        Catch ex As Exception
            MessageBox.Show("Google Sheets-ში მონაცემების განახლების შეცდომა: " & ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' სისტემიდან გამოსვლა - ტოკენის წაშლა
    Public Function Logout() As Boolean
        Try
            If File.Exists(TOKEN_PATH) Then
                ' შევეცადოთ ფაილის/ფოლდერის წაშლას
                Try
                    If Directory.Exists(TOKEN_PATH) Then
                        Directory.Delete(TOKEN_PATH, True)
                    Else
                        File.Delete(TOKEN_PATH)
                    End If
                Catch ex As Exception
                    MessageBox.Show("ტოკენის წაშლის შეცდომა: " & ex.Message, "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Try
            End If

            ' სერვისის წაშლა
            sheetsService = Nothing
            userInfo = Nothing

            Return True
        Catch ex As Exception
            MessageBox.Show("გამოსვლის შეცდომა: " & ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function
End Class