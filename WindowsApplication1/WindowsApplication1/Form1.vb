Imports System.IO
Imports System.Threading
Imports HS.Platform
Imports Newtonsoft.Json
Imports SynData
Imports LumiSoft.Net.Mail
Imports LumiSoft.Net
Imports LumiSoft.Net.Mime
Imports MiniUiAppCode.Platform
Imports MiniUiAppCode.Platform.Utils



Public Class Form1
    Public pst As CmsPassport
    Dim sysData As SynDefine
    Public Delegate Sub setText(ByVal msg As String)
    Public setTextHandler As setText = New setText(AddressOf Me.setRichTextBox)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Enabled = False
        Try
            CmsEnvironment.InitForClientApplication(Application.StartupPath)
            pst = CmsPassport.GenerateCmsPassportBySysuser()
            Dim filepath As String = "dataconfig.json"
            Dim stream As FileStream = System.IO.File.OpenRead(filepath)
            Dim txtrd As TextReader = New System.IO.StringReader((System.IO.File.ReadAllText(filepath)))

            Dim jsrd As JsonTextReader = New JsonTextReader(txtrd)

            Dim js As JsonSerializer = New JsonSerializer()


            sysData = js.Deserialize(Of SynDefine)(jsrd)
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Button2.Enabled = False
        For i As Integer = 0 To sysData.SynDatas.Count - 1

            Try
                Dim Process As SynProcessor = New SynProcessor(pst, sysData, sysData.SynDatas(i))
                Process.printMessageHandler = New SynProcessor.printMessage(AddressOf printMessage)
                Process.includedatalog = sysData.includedatalog


                Dim Thread As New Thread(New ThreadStart(AddressOf Process.DealSynThread))
                Thread.IsBackground = True
                Thread.Start()
            Catch ex As Exception
                SLog.Err("同步数据配置失败-" + sysData.SynDatas(i).uniquenameofsynname + ",错误信息:" + ex.Message)
            End Try

        Next
    End Sub
    Public Sub setRichTextBox(ByVal msg As String)
        If (Me.RichTextBox1.TextLength > 5000) Then
            Me.RichTextBox1.Text = msg
        Else
            Me.RichTextBox1.Text = msg + vbCrLf + Me.RichTextBox1.Text
        End If

    End Sub
    Public Sub printMessage(ByVal msg As String)

        Me.BeginInvoke(setTextHandler, New Object() {msg})

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button1.PerformClick()
        Button2.PerformClick()

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        '  txt_cmsfunction.Text = CmsFunction.FilterSystemFunction(pst, txt_cmsfunction.Text)
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        Dim popServer As String = Text_server.Text
        Dim popPort As String = Text_port.Text
        Dim user As String = Text_user.Text
        Dim pass As String = Text_pass.Text
        Dim isDelete As Boolean = Check_delete.Checked
        '''''''''''''''''''''''''''''''''''''''''''''''''
        '''''''''''''''''''''''''''''''''''''''''''''''''
        Dim resid As Long = 579794693603
        Dim uIDCol As String = "uid"
        Dim senderCol As String = "address"
        Dim bodyTextCol As String = "bodytext"
        Dim bodyHtmlTextCol As String = "bodyhtmltext"
        Dim subjectCol As String = "subject"
        MiniUiAppCode.Platform.Utils.LumiSoft.password = pass
        MiniUiAppCode.Platform.Utils.LumiSoft.pop3Port = popPort
        MiniUiAppCode.Platform.Utils.LumiSoft.pop3Server = popServer
        MiniUiAppCode.Platform.Utils.LumiSoft.username = user
        Dim result As List(Of LumiSoft.Net.Mail.Mail_Message) = New List(Of Mail_Message)()
        Try
            result = MiniUiAppCode.Platform.Utils.LumiSoft.GetEmails(pst, resid, uIDCol, senderCol, bodyTextCol, bodyHtmlTextCol, subjectCol, popServer, popPort, user, pass, Check_delete.Checked, RichTextBox1)
        Catch ex As Exception
            Me.RichTextBox1.Text = "GetEmails Error:" + ex.Message.ToString()
        End Try

    End Sub

    Private Sub Text_server_TextChanged(sender As Object, e As EventArgs) Handles Text_server.TextChanged

    End Sub
End Class
