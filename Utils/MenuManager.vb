' Utils/MenuManager.vb ფაილი
Imports System.Windows.Forms

Public Class MenuManager
    ' მენიუს ელემენტები
    Private mainMenu As MenuStrip
    Private mnuHome As ToolStripMenuItem
    Private mnuCalendar As ToolStripMenuItem
    Private mnuDatabases As ToolStripMenuItem
    Private mnuGraphs As ToolStripMenuItem
    Private mnuDocuments As ToolStripMenuItem
    Private mnuFinances As ToolStripMenuItem
    Private mnuAdmin As ToolStripMenuItem

    ' ქვემენიუს ელემენტები "ბაზების" ქვეშ
    Private mnuSchedule As ToolStripMenuItem
    Private mnuBeneficiaries As ToolStripMenuItem
    Private mnuTherapists As ToolStripMenuItem
    Private mnuTherapies As ToolStripMenuItem
    Private mnuFunding As ToolStripMenuItem

    ' ქვემენიუს ელემენტები "ადმინისტრირების" ქვეშ
    Private mnuUserRegistration As ToolStripMenuItem

    ' მშობელი ფორმა
    Private parentForm As Form

    ' კონსტრუქტორი
    Public Sub New(form As Form)
        parentForm = form
        InitializeMenu()
    End Sub

    ' მენიუს ინიციალიზაცია
    Private Sub InitializeMenu()
        ' მთავარი მენიუს შექმნა
        mainMenu = New MenuStrip()
        mainMenu.Dock = DockStyle.Top

        ' მენიუს ელემენტების შექმნა
        mnuHome = New ToolStripMenuItem("საწყისი")
        mnuCalendar = New ToolStripMenuItem("კალენდარი")
        mnuDatabases = New ToolStripMenuItem("ბაზები")
        mnuGraphs = New ToolStripMenuItem("გრაფიკები")
        mnuDocuments = New ToolStripMenuItem("დოკუმენტები")
        mnuFinances = New ToolStripMenuItem("ფინანსები")
        mnuAdmin = New ToolStripMenuItem("ადმინისტრირება")

        ' "ბაზების" ქვემენიუს შექმნა
        mnuSchedule = New ToolStripMenuItem("განრიგი")
        mnuBeneficiaries = New ToolStripMenuItem("ბენეფიციარები")
        mnuTherapists = New ToolStripMenuItem("თერაპევტები")
        mnuTherapies = New ToolStripMenuItem("თერაპიები")
        mnuFunding = New ToolStripMenuItem("დაფინანსება")

        ' "ადმინისტრირების" ქვემენიუს შექმნა
        mnuUserRegistration = New ToolStripMenuItem("მომხმარებელთა რეგისტრაცია")

        ' ქვემენიუს ელემენტების დამატება
        mnuDatabases.DropDownItems.Add(mnuSchedule)
        mnuDatabases.DropDownItems.Add(mnuBeneficiaries)
        mnuDatabases.DropDownItems.Add(mnuTherapists)
        mnuDatabases.DropDownItems.Add(mnuTherapies)
        mnuDatabases.DropDownItems.Add(mnuFunding)

        mnuAdmin.DropDownItems.Add(mnuUserRegistration)

        ' მთავარ მენიუში ყველა ელემენტის დამატება
        mainMenu.Items.Add(mnuHome)
        mainMenu.Items.Add(mnuCalendar)
        mainMenu.Items.Add(mnuDatabases)
        mainMenu.Items.Add(mnuGraphs)
        mainMenu.Items.Add(mnuDocuments)
        mainMenu.Items.Add(mnuFinances)
        mainMenu.Items.Add(mnuAdmin)

        ' ივენთ ჰენდლერების დამატება
        AddHandlers()

        ' მენიუს კონტროლის დამატება ფორმაზე
        parentForm.Controls.Add(mainMenu)
        parentForm.MainMenuStrip = mainMenu

        ' თავდაპირველად მხოლოდ "საწყისი" გამოჩნდება
        ShowOnlyHomeMenu()
    End Sub

    ' მხოლოდ "საწყისი" მენიუს გამოჩენა
    Public Sub ShowOnlyHomeMenu()
        mnuHome.Visible = True
        mnuCalendar.Visible = False
        mnuDatabases.Visible = False
        mnuGraphs.Visible = False
        mnuDocuments.Visible = False
        mnuFinances.Visible = False
        mnuAdmin.Visible = False
    End Sub

    ' ყველა მენიუს გამოჩენა (ადმინისტრატორისთვის)
    Public Sub ShowAdminMenu()
        mnuHome.Visible = True
        mnuCalendar.Visible = True
        mnuDatabases.Visible = True
        mnuGraphs.Visible = True
        mnuDocuments.Visible = True
        mnuFinances.Visible = True
        mnuAdmin.Visible = True
    End Sub

    ' მენეჯერის მენიუს გამოჩენა
    Public Sub ShowManagerMenu()
        mnuHome.Visible = True
        mnuCalendar.Visible = True
        mnuDatabases.Visible = True
        mnuGraphs.Visible = True
        mnuDocuments.Visible = True
        mnuFinances.Visible = True
        mnuAdmin.Visible = False
    End Sub

    ' თერაპევტის მენიუს გამოჩენა
    Public Sub ShowTherapistMenu()
        mnuHome.Visible = True
        mnuCalendar.Visible = True
        mnuDatabases.Visible = True
        mnuGraphs.Visible = False
        mnuDocuments.Visible = True
        mnuFinances.Visible = False
        mnuAdmin.Visible = False
    End Sub

    ' მიმღების მენიუს გამოჩენა
    Public Sub ShowReceptionistMenu()
        mnuHome.Visible = True
        mnuCalendar.Visible = True
        mnuDatabases.Visible = True
        mnuGraphs.Visible = False
        mnuDocuments.Visible = False
        mnuFinances.Visible = False
        mnuAdmin.Visible = False
    End Sub

    ' როლის მიხედვით მენიუს გამოჩენა
    Public Sub ShowMenuByRole(role As String)
        Select Case role.ToLower()
            Case "admin"
                ShowAdminMenu()
            Case "manager"
                ShowManagerMenu()
            Case "therapist"
                ShowTherapistMenu()
            Case "receptionist"
                ShowReceptionistMenu()
            Case Else
                ShowOnlyHomeMenu()
        End Select
    End Sub

    ' ივენთ ჰენდლერების დამატება
    Private Sub AddHandlers()
        AddHandler mnuHome.Click, AddressOf MenuItemClicked
        AddHandler mnuCalendar.Click, AddressOf MenuItemClicked
        AddHandler mnuDatabases.Click, AddressOf MenuItemClicked
        AddHandler mnuGraphs.Click, AddressOf MenuItemClicked
        AddHandler mnuDocuments.Click, AddressOf MenuItemClicked
        AddHandler mnuFinances.Click, AddressOf MenuItemClicked
        AddHandler mnuAdmin.Click, AddressOf MenuItemClicked

        AddHandler mnuSchedule.Click, AddressOf MenuItemClicked
        AddHandler mnuBeneficiaries.Click, AddressOf MenuItemClicked
        AddHandler mnuTherapists.Click, AddressOf MenuItemClicked
        AddHandler mnuTherapies.Click, AddressOf MenuItemClicked
        AddHandler mnuFunding.Click, AddressOf MenuItemClicked

        AddHandler mnuUserRegistration.Click, AddressOf MenuItemClicked
    End Sub

    ' მენიუს ელემენტზე დაჭერისას გამოძახებული მეთოდი
    Private Sub MenuItemClicked(sender As Object, e As EventArgs)
        Dim menuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        ' აქ შეგიძლიათ სხვადასხვა მენიუს ელემენტზე სხვადასხვა მოქმედება შეასრულოთ
        Select Case menuItem.Text
            Case "საწყისი"
                MessageBox.Show("საწყისი გვერდი")
            Case "კალენდარი"
                MessageBox.Show("კალენდარი")
            Case "განრიგი"
                MessageBox.Show("განრიგი")
            Case "ბენეფიციარები"
                MessageBox.Show("ბენეფიციარები")
            Case "თერაპევტები"
                MessageBox.Show("თერაპევტები")
            Case "თერაპიები"
                MessageBox.Show("თერაპიები")
            Case "დაფინანსება"
                MessageBox.Show("დაფინანსება")
            Case "გრაფიკები"
                MessageBox.Show("გრაფიკები")
            Case "დოკუმენტები"
                MessageBox.Show("დოკუმენტები")
            Case "ფინანსები"
                MessageBox.Show("ფინანსები")
            Case "მომხმარებელთა რეგისტრაცია"
                MessageBox.Show("მომხმარებელთა რეგისტრაცია")
        End Select
    End Sub
End Class