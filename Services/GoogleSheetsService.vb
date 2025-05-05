' Services/GoogleSheetsService.vb
Imports System.Windows.Forms
Imports System.IO
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Sheets.v4
Imports Google.Apis.Services
Imports Google.Apis.Sheets.v4.Data
Imports System.Configuration

''' <summary>
''' Google Sheets API-სთან ურთიერთობის სერვისი
''' </summary>
Public Class GoogleSheetsService
    ' აპლიკაციის სახელი
    Private Shared ReadOnly ApplicationName As String = "Scheduler_v8.6a"

    ' მომხმარებლის მონაცემების ცხრილის ID
    Private Shared ReadOnly UserSpreadsheetId As String =
        ConfigurationManager.AppSettings("UserSpreadsheetId")

    ' სერვის აკაუნტის გასაღების გზა
    Private Shared ReadOnly ServiceAccountKeyPath As String =
        Path.Combine(Application.StartupPath, ConfigurationManager.AppSettings("ServiceAccountKeyPath"))

    ' Sheets API სერვისის შექმნა
    Private Shared Function CreateSheetsService() As SheetsService
        Try
            ' სერვის აკაუნტის გასაღების წაკითხვა
            Dim credential As GoogleCredential = GoogleCredential.FromFile(ServiceAccountKeyPath) _
                .CreateScoped(New String() {SheetsService.Scope.Spreadsheets})

            ' Sheets სერვისის შექმნა
            Return New SheetsService(New BaseClientService.Initializer() With {
                .HttpClientInitializer = credential,
                .ApplicationName = ApplicationName
            })
        Catch ex As Exception
            MessageBox.Show($"Google Sheets სერვისთან დაკავშირების შეცდომა: {ex.Message}",
                           "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' მომხმარებლის როლის მიღება ელფოსტის მიხედვით
    ''' </summary>
    ''' <param name="email">მომხმარებლის ელფოსტა</param>
    ''' <returns>მომხმარებლის როლი ან ცარიელი სტრინგი თუ მომხმარებელი არ მოიძებნა</returns>
    Public Shared Function GetUserRole(email As String) As String
        Try
            ' Sheets სერვისის შექმნა
            Dim service As SheetsService = CreateSheetsService()
            If service Is Nothing Then
                Return String.Empty
            End If

            ' მომხმარებლების ცხრილის წაკითხვა
            Dim range As String = "მომხმარებლები!A:C" ' ცხრილის სახელი და სვეტების დიაპაზონი (ა:მეილი, ბ:როლი, გ:სახელი)
            Dim request As SpreadsheetsResource.ValuesResource.GetRequest =
                service.Spreadsheets.Values.Get(UserSpreadsheetId, range)

            Dim response = request.Execute()
            Dim values = response.Values

            If values IsNot Nothing AndAlso values.Count > 0 Then
                ' ცხრილში ვეძებთ მომხმარებლის ელფოსტას
                For Each row In values
                    ' შევამოწმოთ, რომ მწკრივში საკმარისი სვეტებია
                    If row.Count >= 2 AndAlso row(0).ToString().Equals(email, StringComparison.OrdinalIgnoreCase) Then
                        Return row(1).ToString() ' როლის დაბრუნება (B სვეტი)
                    End If
                Next
            End If

            ' თუ მომხმარებელი ვერ მოიძებნა, ვაბრუნებთ ცარიელ როლს
            Return String.Empty

        Catch ex As Exception
            MessageBox.Show($"შეცდომა მომხმარებლის როლის მიღებისას: {ex.Message}",
                           "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' მომხმარებლის სახელის მიღება ელფოსტის მიხედვით
    ''' </summary>
    ''' <param name="email">მომხმარებლის ელფოსტა</param>
    ''' <returns>მომხმარებლის სახელი ან ცარიელი სტრინგი თუ მომხმარებელი არ მოიძებნა</returns>
    Public Shared Function GetUserName(email As String) As String
        Try
            ' Sheets სერვისის შექმნა
            Dim service As SheetsService = CreateSheetsService()
            If service Is Nothing Then
                Return String.Empty
            End If

            ' მომხმარებლების ცხრილის წაკითხვა
            Dim range As String = "მომხმარებლები!A:C" ' ცხრილის სახელი და სვეტების დიაპაზონი (ა:მეილი, ბ:როლი, გ:სახელი)
            Dim request As SpreadsheetsResource.ValuesResource.GetRequest =
                service.Spreadsheets.Values.Get(UserSpreadsheetId, range)

            Dim response = request.Execute()
            Dim values = response.Values

            If values IsNot Nothing AndAlso values.Count > 0 Then
                ' ცხრილში ვეძებთ მომხმარებლის ელფოსტას
                For Each row In values
                    ' შევამოწმოთ, რომ მწკრივში საკმარისი სვეტებია
                    If row.Count >= 3 AndAlso row(0).ToString().Equals(email, StringComparison.OrdinalIgnoreCase) Then
                        Return row(2).ToString() ' სახელის დაბრუნება (C სვეტი)
                    End If
                Next
            End If

            ' თუ მომხმარებელი ვერ მოიძებნა, ვაბრუნებთ ცარიელ სახელს
            Return String.Empty

        Catch ex As Exception
            MessageBox.Show($"შეცდომა მომხმარებლის სახელის მიღებისას: {ex.Message}",
                           "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' მომხმარებლის ავტორიზაცია ელფოსტით
    ''' </summary>
    ''' <param name="email">მომხმარებლის ელფოსტა</param>
    ''' <returns>Tuple(სახელი, როლი) ან Nothing თუ მომხმარებელი არ მოიძებნა</returns>
    Public Shared Function AuthorizeUser(email As String) As Tuple(Of String, String)
        Try
            ' Sheets სერვისის შექმნა
            Dim service As SheetsService = CreateSheetsService()
            If service Is Nothing Then
                Return Nothing
            End If

            ' მომხმარებლების ცხრილის წაკითხვა
            Dim range As String = "მომხმარებლები!A:C" ' ცხრილის სახელი და სვეტების დიაპაზონი (ა:მეილი, ბ:როლი, გ:სახელი)
            Dim request As SpreadsheetsResource.ValuesResource.GetRequest =
                service.Spreadsheets.Values.Get(UserSpreadsheetId, range)

            Dim response = request.Execute()
            Dim values = response.Values

            If values IsNot Nothing AndAlso values.Count > 0 Then
                ' ცხრილში ვეძებთ მომხმარებლის ელფოსტას
                For Each row In values
                    ' შევამოწმოთ, რომ მწკრივში საკმარისი სვეტებია და პირველი მწკრივი არაა სათაური
                    If row.Count >= 3 AndAlso row(0).ToString().Equals(email, StringComparison.OrdinalIgnoreCase) Then
                        Dim role As String = row(1).ToString()
                        Dim name As String = row(2).ToString()
                        Return New Tuple(Of String, String)(name, role)
                    End If
                Next
            End If

            ' თუ მომხმარებელი ვერ მოიძებნა, ვაბრუნებთ Nothing
            Return Nothing

        Catch ex As Exception
            MessageBox.Show($"შეცდომა მომხმარებლის ავტორიზაციისას: {ex.Message}",
                           "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function
End Class