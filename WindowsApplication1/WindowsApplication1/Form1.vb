Imports System.IO
Imports System.Threading
Imports HS.Platform
Imports Newtonsoft.Json
Imports SynData

Public Class Form1
    Public pst As CmsPassport
    Dim sysData As SynDefine
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            CmsEnvironment.InitForClientApplication(Application.StartupPath)
            pst = CmsPassport.GenerateCmsPassportBySysuser()
            Dim filepath As String = "dataconfig.json"
            Dim stream As FileStream = File.OpenRead(filepath)
            Dim txtrd As TextReader = New StringReader((File.ReadAllText(filepath)))

            Dim jsrd As JsonTextReader = New JsonTextReader(txtrd)

            Dim js As JsonSerializer = New JsonSerializer()


            sysData = js.Deserialize(Of SynDefine)(jsrd)
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        For i As Integer = 0 To sysData.SynDatas.Count - 1

            Try
                Dim Process As SynProcessor = New SynProcessor(pst, sysData.SynDatas(i))
                Dim thread As New Thread(New ThreadStart(AddressOf Process.DealSynThread))
                thread.IsBackground = True
                thread.Start()
            Catch ex As Exception
                SLog.Err("同步数据配置失败-" + sysData.SynDatas(i).uniquenameofsynname + ",错误信息:" + ex.Message)
            End Try

        Next
    End Sub
End Class
