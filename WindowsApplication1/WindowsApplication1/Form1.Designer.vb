<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
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

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Text_server = New System.Windows.Forms.TextBox()
        Me.Text_port = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Text_user = New System.Windows.Forms.TextBox()
        Me.Text_pass = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Check_delete = New System.Windows.Forms.CheckBox()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(24, 24)
        Me.Button1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(150, 46)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "初始化"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(186, 24)
        Me.Button2.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(150, 46)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "开始同步"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'RichTextBox1
        '
        Me.RichTextBox1.Location = New System.Drawing.Point(24, 82)
        Me.RichTextBox1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.Size = New System.Drawing.Size(1554, 876)
        Me.RichTextBox1.TabIndex = 2
        Me.RichTextBox1.Text = ""
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(1972, 576)
        Me.Button3.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(150, 46)
        Me.Button3.TabIndex = 3
        Me.Button3.Text = "读取邮件"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Text_server
        '
        Me.Text_server.Location = New System.Drawing.Point(1792, 104)
        Me.Text_server.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Text_server.Name = "Text_server"
        Me.Text_server.Size = New System.Drawing.Size(478, 35)
        Me.Text_server.TabIndex = 4
        Me.Text_server.Text = "outlook.office365.com"
        '
        'Text_port
        '
        Me.Text_port.Location = New System.Drawing.Point(1792, 224)
        Me.Text_port.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Text_port.Name = "Text_port"
        Me.Text_port.Size = New System.Drawing.Size(478, 35)
        Me.Text_port.TabIndex = 5
        Me.Text_port.Text = "995"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(1650, 110)
        Me.Label1.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(130, 24)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "邮箱服务器"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(1650, 242)
        Me.Label2.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(58, 24)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "端口"
        '
        'Text_user
        '
        Me.Text_user.Location = New System.Drawing.Point(1792, 338)
        Me.Text_user.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Text_user.Name = "Text_user"
        Me.Text_user.Size = New System.Drawing.Size(478, 35)
        Me.Text_user.TabIndex = 8
        Me.Text_user.Text = "recruitinghris@finisar.com"
        '
        'Text_pass
        '
        Me.Text_pass.Location = New System.Drawing.Point(1792, 494)
        Me.Text_pass.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Text_pass.Name = "Text_pass"
        Me.Text_pass.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.Text_pass.Size = New System.Drawing.Size(478, 35)
        Me.Text_pass.TabIndex = 9
        Me.Text_pass.Text = "Password201805"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(1650, 356)
        Me.Label3.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(58, 24)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "账号"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(1650, 512)
        Me.Label4.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(58, 24)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "密码"
        '
        'Check_delete
        '
        Me.Check_delete.AutoSize = True
        Me.Check_delete.Location = New System.Drawing.Point(1792, 584)
        Me.Check_delete.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Check_delete.Name = "Check_delete"
        Me.Check_delete.Size = New System.Drawing.Size(138, 28)
        Me.Check_delete.TabIndex = 12
        Me.Check_delete.Text = "删除邮件"
        Me.Check_delete.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(809, 24)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(116, 46)
        Me.Button4.TabIndex = 13
        Me.Button4.Text = "Button4"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(2500, 986)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Check_delete)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Text_pass)
        Me.Controls.Add(Me.Text_user)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Text_port)
        Me.Controls.Add(Me.Text_server)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.RichTextBox1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Name = "Form1"
        Me.Text = "数据同步20180617a"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents RichTextBox1 As RichTextBox
    Friend WithEvents Button3 As Button
    Friend WithEvents Text_server As TextBox
    Friend WithEvents Text_port As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Text_user As TextBox
    Friend WithEvents Text_pass As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Check_delete As CheckBox
    Friend WithEvents Button4 As Button
End Class
