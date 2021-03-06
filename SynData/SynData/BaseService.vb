﻿Imports System.IO
Imports MiniUiAppCode.Platform
Imports Newtonsoft.Json
Imports HS.Platform

Public Class BaseService
    ' Methods
    Private Function checkLogin() As Boolean
        Return Me.PlatformLogined
    End Function

    Private Function login() As Boolean
        Me.aRealsunClient = New MiniUiAppCode.Platform.RealsunClientNet(Me.PlatformWwwUrl)
        Dim resultModel As MiniUiAppCode.Platform.PlatformResultModel = JsonConvert.DeserializeObject(Of MiniUiAppCode.Platform.PlatformResultModel)(Me.aRealsunClient.LoginPlatform(Me.PlatformUser, Me.PlatformPassword, Me.PlatformApiToken))
        If (resultModel.Error = 0) Then
            Me.LoginToken = resultModel.Token
            Me.PlatformLogined = True
        Else
            Me.LoginToken = String.Empty
            Me.PlatformLogined = False
        End If
        Return Me.PlatformLogined
    End Function

    Public Function Post(ByVal method As String, ByVal param As Hashtable) As MiniUiAppCode.Platform.PlatformResultModel
        If (Not Me.checkLogin AndAlso Not Me.login) Then
            Dim model1 As New MiniUiAppCode.Platform.PlatformResultModel
            model1.Error = 1
            model1.Message = ChrW(24179) & ChrW(21488) & ChrW(30331) & ChrW(24405) & ChrW(22833) & ChrW(36133) & ChrW(65281)
            Return model1
        End If
        Dim requestid As Long = 0
        Dim str As String = Me.aRealsunClient.FlatformExecute(method, param, (requestid))

        Return JsonConvert.DeserializeObject(Of MiniUiAppCode.Platform.PlatformResultModel)(str)
    End Function
    Public Shared Function ShowHostTableDatas_Ajax_GetDATA(ByVal dt As DataTable, ByVal state As String, ByVal target_synmonitorcolumnofid As String, ByVal synmonitorid As String, Optional sourcecmscolumns As String = "", Optional targetcmscolumns As String = "", Optional method As String = "", Optional targetresid As String = "") As ArrayList
        Dim result As Hashtable = New Hashtable
        Dim alist As ArrayList
        Dim alistofcolumn As New List(Of String)
        Dim alistoftargetcolumn As New List(Of String)
        If sourcecmscolumns <> "" Then
            Dim strColumns As String() = sourcecmscolumns.Split(",")
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
                alist = MiniUiAppCode.Platform.DBUtil.DataTable2ArrayList(dt, alistofcolumn)
            Else
                alist = MiniUiAppCode.Platform.DBUtil.DataTable2ArrayList(dt)
            End If
        Else
            alist = MiniUiAppCode.Platform.DBUtil.DataTable2ArrayList(dt)
        End If


        Dim data As ArrayList = New ArrayList()


        For i As Integer = 0 To alist.Count - 1
            Dim record As Hashtable = DirectCast(alist(i), Hashtable)
            If method = "getrecid" Then
                If Not record.ContainsKey("REC_ID") Then
                    record.Add("REC_ID", TimeId.CurrentMillisecondsThreadSafe().ToString())
                Else
                    record("REC_ID") = TimeId.CurrentMillisecondsThreadSafe().ToString()
                End If
                If Not record.ContainsKey("REC_RESID") Then
                    record.Add("REC_RESID", targetresid)
                Else
                    record("REC_RESID") = targetresid
                End If
                If Not record.ContainsKey("REC_CRTTIME") Then
                    record.Add("REC_CRTTIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm"))
                Else
                    record("REC_CRTTIME") = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                End If
            Else

                record.Add("_state", state)
                record.Add("_id", i)
                If record.ContainsKey("REC_ID") Then
                    record.Remove("REC_ID")
                End If

            End If

            For k As Integer = 0 To alistoftargetcolumn.Count - 1

                Dim obj As Object = record(alistofcolumn(k))
                Dim str As String = Convert.ToString(obj)
                record.Remove(alistofcolumn(k))
                If str.IndexOf("&") > 0 Then
                    str = str.Replace("&", "-")
                    record.Add(alistoftargetcolumn(k), str)
                Else
                    record.Add(alistoftargetcolumn(k), obj)
                End If





            Next
            record.Add(target_synmonitorcolumnofid, synmonitorid)
            If record Is Nothing Then
                Continue For
            End If

            data.Add(record)

        Next
        Return data

    End Function
    Public Shared Function GetSyndefine(ByVal filepath As String, ByRef errmsg As String) As SynDefine
        Try

            Dim stream As FileStream = IO.File.OpenRead(filepath)
            Dim tr As TextReader = New StringReader((IO.File.ReadAllText(filepath)))

            Dim jtr As JsonTextReader = New JsonTextReader(tr)

            Dim js As JsonSerializer = New JsonSerializer()

            Return js.Deserialize(Of SynDefine)(jtr)
        Catch ex As Exception
            errmsg = ex.Message.ToString()
            Return Nothing

        End Try
    End Function

    ' Fields
    Public Property aRealsunClient As MiniUiAppCode.Platform.RealsunClientNet = Nothing
    Public Property deleteMethod As String = "Ajax_DeleteByColumn"
    Public Property getMethod As String = "ShowHostTableDatas_Ajax"
    Public Property LoginToken As String = String.Empty
    Public Property PlatformApiToken As String = "KingOfDinner123456789"
    Public Property PlatformLogined As Boolean = False
    Public Property PlatformPassword As String
    Public Property PlatformUser As String
    Public Property PlatformWwwUrl As String
    Public Property saveMethod As String = "SaveData_Ajax"
    Public Property saveMethod2 As String = "ajax_SaveDataBytrans"
    Public Property saveMethod3 As String = "ajax_InsertBatchData"
    Public Property saveMethodWithSub As String = "ajax_SaveDataWithSubTableBytrans"
End Class


