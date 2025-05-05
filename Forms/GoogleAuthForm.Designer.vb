' GoogleAuthForm.Designer.vb
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class GoogleAuthForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.btnLogin = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.pbGoogleLogo = New System.Windows.Forms.PictureBox()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.pnlFooter = New System.Windows.Forms.Panel()
        Me.browser = New System.Windows.Forms.WebBrowser()

        ' პიქჩერბოქსის კონტეინერის მითითება თუ არსებობს
        CType(Me.pbGoogleLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlHeader.SuspendLayout()
        Me.pnlFooter.SuspendLayout()
        Me.SuspendLayout()

        ' btnLogin
        Me.btnLogin.Location = New System.Drawing.Point(122, 20)
        Me.btnLogin.Name = "btnLogin"
        Me.btnLogin.Size = New System.Drawing.Size(120, 35)
        Me.btnLogin.TabIndex = 0
        Me.btnLogin.Text = "შესვლა"
        Me.btnLogin.UseVisualStyleBackColor = True

        ' btnCancel
        Me.btnCancel.Location = New System.Drawing.Point(248, 20)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(120, 35)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "გაუქმება"
        Me.btnCancel.UseVisualStyleBackColor = True

        ' lblTitle
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(80, 20)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(325, 25)
        Me.lblTitle.TabIndex = 2
        Me.lblTitle.Text = "შედით სისტემაში Google-ის ანგარიშით"

        ' lblDescription
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Location = New System.Drawing.Point(82, 55)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(325, 15)
        Me.lblDescription.TabIndex = 3
        Me.lblDescription.Text = "ავტორიზაციისთვის დააჭირეთ ღილაკს და მიჰყევით ინსტრუქციას"

        ' pbGoogleLogo
        Me.pbGoogleLogo.Location = New System.Drawing.Point(20, 20)
        Me.pbGoogleLogo.Name = "pbGoogleLogo"
        Me.pbGoogleLogo.Size = New System.Drawing.Size(50, 50)
        Me.pbGoogleLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbGoogleLogo.TabIndex = 4
        Me.pbGoogleLogo.TabStop = False

        ' pnlHeader
        Me.pnlHeader.Controls.Add(Me.lblTitle)
        Me.pnlHeader.Controls.Add(Me.lblDescription)
        Me.pnlHeader.Controls.Add(Me.pbGoogleLogo)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(500, 90)
        Me.pnlHeader.TabIndex = 5

        ' pnlFooter
        Me.pnlFooter.Controls.Add(Me.btnLogin)
        Me.pnlFooter.Controls.Add(Me.btnCancel)
        Me.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlFooter.Location = New System.Drawing.Point(0, 420)
        Me.pnlFooter.Name = "pnlFooter"
        Me.pnlFooter.Size = New System.Drawing.Size(500, 70)
        Me.pnlFooter.TabIndex = 6

        ' browser
        Me.browser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.browser.Location = New System.Drawing.Point(0, 90)
        Me.browser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.browser.Name = "browser"
        Me.browser.Size = New System.Drawing.Size(500, 330)
        Me.browser.TabIndex = 7
        ' დასაწყისში ბრაუზერი უნდა იყოს დამალული
        Me.browser.Visible = False

        ' GoogleAuthForm
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(500, 490)
        Me.Controls.Add(Me.browser)
        Me.Controls.Add(Me.pnlFooter)
        Me.Controls.Add(Me.pnlHeader)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "GoogleAuthForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Google-ით ავტორიზაცია"

        ' კონტროლების რელიზი
        CType(Me.pbGoogleLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.pnlFooter.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub
End Class
