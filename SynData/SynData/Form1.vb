Imports Newtonsoft.Json
Imports System.IO
Imports HS.Platform
Imports System.Windows.Forms

Public Class Form1
    Public pst As CmsPassport
    Public sysData As SynDefine
    Dim ds As New DataSet
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

        Try
            ds = CmsTable.GetDatasetForHostTable(pst, sysData.SynDatas(0).source_resid, False, sysData.SynDatas(0).cmswhere)

        Catch ex As Exception

        End Try



    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        BaseService.PlatformWwwUrl = sysData.baseUrl
        BaseService.saveMethod = sysData.saveMethod
        BaseService.getMethod = sysData.getMethod
        BaseService.PlatformUser = sysData.user
        BaseService.PlatformPassword = sysData.upass
        BaseService.PlatformApiToken = sysData.LoginToken

        Dim bs As New BaseService()
        Dim param As Hashtable = New Hashtable()
        Dim listofdata As New ArrayList()
        Dim cmscolumns As New ArrayList
        Dim rt As PlatformResultModel
        listofdata = bs.ShowHostTableDatas_Ajax_GetDATA(ds.Tables(0), sysData.SynDatas(0)._state, 0, 0, sysData.SynDatas(0).sourcefields, sysData.SynDatas(0).targetfields)
        If listofdata IsNot Nothing Then
            param.Add("resid", sysData.SynDatas(0).target_resid)
            param.Add("data", JsonConvert.SerializeObject(listofdata))
            param.Add("uniquecolumns", sysData.SynDatas(0).uniquecolumns)
            param.Add("withoutdata", sysData.SynDatas(0).withoutdata)
            Try
                rt = bs.Post(BaseService.saveMethod2, param)

            Catch ex As Exception

            End Try

        End If

    End Sub
End Class