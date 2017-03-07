Imports MiniUiAppCode.Platform
Imports Newtonsoft.Json

Public Class BaseService
    ' Methods
    Private Function checkLogin() As Boolean
        Return BaseService.PlatformLogined
    End Function

    Private Function login() As Boolean
        BaseService.aRealsunClient = New RealsunClientNet(BaseService.PlatformWwwUrl)
        Dim resultModel As PlatformResultModel = JsonConvert.DeserializeObject(Of PlatformResultModel)(BaseService.aRealsunClient.LoginPlatform(BaseService.PlatformUser, BaseService.PlatformPassword, BaseService.PlatformApiToken))
        If (resultModel.Error = 0) Then
            BaseService.LoginToken = resultModel.Token
            BaseService.PlatformLogined = True
        Else
            BaseService.LoginToken = String.Empty
            BaseService.PlatformLogined = False
        End If
        Return BaseService.PlatformLogined
    End Function

    Public Function Post(ByVal method As String, ByVal param As Hashtable) As PlatformResultModel
        If (Not Me.checkLogin AndAlso Not Me.login) Then
            Dim model1 As New PlatformResultModel
            model1.Error = 1
            model1.Message = ChrW(24179) & ChrW(21488) & ChrW(30331) & ChrW(24405) & ChrW(22833) & ChrW(36133) & ChrW(65281)
            Return model1
        End If
        Dim requestid As Long = 0
        Dim str As String = BaseService.aRealsunClient.FlatformExecute(method, param, (requestid))
        Return JsonConvert.DeserializeObject(Of PlatformResultModel)(str)
    End Function
    Public Function ShowHostTableDatas_Ajax_GetDATA(dt As DataTable, state As String, Optional pageindex As Long = 0, Optional pagesize As Long = 0, Optional cmscolumns As String = "", Optional targetcmscolumns As String = "") As ArrayList
        Dim result As Hashtable = New Hashtable
        Dim alist As ArrayList
        Dim alistofcolumn As New List(Of String)
        Dim alistoftargetcolumn As New List(Of String)
        If cmscolumns <> "" Then
            Dim strColumns As String() = cmscolumns.Split(",")
            Dim strTargetColumns As String() = targetcmscolumns.Split(",")

            For Each str As String In strTargetColumns
                alistoftargetcolumn.Add(str)
            Next
            For Each str As String In strColumns
                alistofcolumn.Add(str)
            Next
            If alistofcolumn.Count <> alistoftargetcolumn.Count Then
                Return Nothing
            End If
            If (Not alistofcolumn.Contains("REC_ID")) And alistofcolumn.Count > 0 Then
                alistofcolumn.Add("REC_ID")
            End If
            If alistofcolumn.Count > 0 Then
                alist = DBUtil.DataTable2ArrayList(dt, alistofcolumn)
            Else
                alist = DBUtil.DataTable2ArrayList(dt)
            End If
        Else
            alist = DBUtil.DataTable2ArrayList(dt)
        End If


        Dim data As ArrayList = New ArrayList()

        Dim start As Integer = pageindex * pagesize ' index * size

        Dim ends As Integer = start + pagesize
        If pagesize = 0 Then
            ends = dt.Rows.Count
        End If
        For i As Integer = 0 To alist.Count - 1
            Dim record As Hashtable = DirectCast(alist(i), Hashtable)
            record.Add("_state", state)
            record.Add("_id", i)
            record.Remove("REC_ID")
            For k As Integer = 0 To alistoftargetcolumn.Count - 1
                record.Add(alistoftargetcolumn(k), record(alistofcolumn(k)))
                record.Remove(alistofcolumn(k))
            Next
            If record Is Nothing Then
                Continue For
            End If
            If start <= i And i < ends Then
                data.Add(record)
            End If
        Next
        Return data

    End Function

    ' Fields
    Public Shared aRealsunClient As RealsunClientNet = Nothing
    Public Shared deleteMethod As String = "Ajax_DeleteByColumn"
    Public Shared getMethod As String = "ShowHostTableDatas_Ajax"
    Public Shared LoginToken As String = String.Empty
    Public Shared PlatformApiToken As String
    Public Shared PlatformLogined As Boolean = False
    Public Shared PlatformPassword As String
    Public Shared PlatformUser As String
    Public Shared PlatformWwwUrl As String
    Public Shared saveMethod As String = "SaveData_Ajax"
    Public Shared saveMethod2 As String = "ajax_SaveDataBytrans"
    Public Shared saveMethodWithSub As String = "ajax_SaveDataWithSubTableBytrans"
End Class


