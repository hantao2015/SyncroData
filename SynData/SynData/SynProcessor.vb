Imports HS.Platform
Imports MiniUiAppCode.Platform
Imports Newtonsoft.Json

Public Class SynProcessor
    Private m_pst As CmsPassport
    Private m_syndata As OneSynDefine
    Private listofhash2Save As List(Of Hashtable)
    Public Property SyncroRows As Long = 0
    Public Property intTotal As Long = 0


    Public Property Pst As CmsPassport
        Get
            Return m_pst
        End Get
        Set(value As CmsPassport)
            m_pst = value

        End Set
    End Property

    Public Property OneSyndata As OneSynDefine
        Get
            Return m_syndata
        End Get
        Set(value As OneSynDefine)
            m_syndata = value

        End Set
    End Property
    Public Sub New(ByVal pst As CmsPassport, ByVal syndata As OneSynDefine)
        Dim errmsg As String = ""
        m_pst = pst
        m_syndata = syndata
        pst = CmsPassport.GenerateCmsPassportBySysuser()
        If Not CheckSyndata(errmsg) Then
            Throw New Exception(errmsg)
        End If

    End Sub
    Public Function CheckSyndata(ByRef errmsg As String) As Boolean
        If OneSyndata.sourcefields = "" Then
            errmsg = "missing field of sourcefields"
            Return False
        End If
        If OneSyndata.pushtype = "" Then
            errmsg = "missing field of pushtype"
            Return False
        End If
        If OneSyndata.target_synmonitorcolumnofid = "" Then
            errmsg = "missing field of target_synmonitorcolumnofid"
            Return False
        End If

        If OneSyndata.fetchtype = "" Then
            errmsg = "missing field of fetchtype"
            Return False
        End If
        If OneSyndata.monitoridcolumn = "" Then
            errmsg = "missing field of monitoridcolumn"
            Return False
        End If
        If OneSyndata.uniquenameofsynname = "" Then
            errmsg = "missing field of uniquenameofsynname"
            Return False
        End If
        If OneSyndata.monitor_resid = "" Then
            errmsg = "missing field of monitor_resid"
            Return False
        End If
        If OneSyndata.targetfields = "" Then
            errmsg = "missing field of targetfields"
            Return False

        End If
        If OneSyndata.source_resid = "" Then
            errmsg = "missing field of source_resid"
            Return False
        End If
        If OneSyndata.target_resid = "" Then
            errmsg = "missing field of target_resid"
            Return False
        End If



        Return True
    End Function
    Public Function GetSourceData(ByRef errmesage As String, ByVal intIndex As Integer) As DataSet

        Try
            Pst.Dbc = OneSyndata.fetchdbc

            Return CmsTable.GetDatasetForHostTable(Pst, OneSyndata.source_resid, False, OneSyndata.cmswhere, "", "", intIndex * OneSyndata.pagesize, OneSyndata.pagesize)
        Catch ex As Exception
            errmesage = ex.Message.ToString()
            Return Nothing
        End Try
    End Function
    Public Function GetSourceDataByPage(ByRef rows As ArrayList, ByRef errmesage As String, ByVal index As Integer, ByVal monitorbatchid As Long) As Boolean
        Dim ds As DataSet
        Dim dt As DataTable = Nothing

        If OneSyndata.fetchtype = "web" Then
            Dim webrows As List(Of Hashtable) = FetchWebSourceData(index, OneSyndata.pagesize, errmesage)
            If errmesage <> "" Then
                Return False
            End If
            Try
                dt = DBUtil.Convert2DataTable(webrows)
            Catch ex As Exception
                ds = Nothing
                errmesage = ex.Message.ToString()
                Return False
            End Try
        Else
            dt = GetSourceData(errmesage, index).Tables(0)
        End If

        If dt IsNot Nothing Then

            rows = BaseService.ShowHostTableDatas_Ajax_GetDATA(dt, OneSyndata._state, OneSyndata.target_synmonitorcolumnofid, Convert.ToString(monitorbatchid), OneSyndata.sourcefields, OneSyndata.targetfields)

        Else
            Return False
        End If
        errmesage = ""
        Return True
    End Function

    Public Function SaveData2Web(ByVal data As Hashtable, ByVal url As String, ByVal user As String, ByVal upass As String) As PlatformResultModel
        Dim bs As New BaseService()
        bs.PlatformWwwUrl = url
        bs.PlatformUser = user
        bs.PlatformPassword = upass
        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            rt = bs.Post(bs.saveMethod2, data)

        Catch ex As Exception
            rt.Error = -1
            rt.Message = ex.Message.ToString()
        End Try
        Return rt
    End Function
    Public Function SaveData2Client(ByVal rows As ArrayList, ByVal dbc As DbConfig, ByRef strErrorMessage As String, ByRef listofdataReturn As List(Of Hashtable)) As Boolean
        Pst.Dbc = dbc
        Dim dbs As DbStatement = New DbStatement(dbc)

        Try
            dbs.TransactionBegin()
            Dim alistofRowstate As ArrayList = Nothing

            Dim rp As CmsTableParam = Nothing

            Dim strRightsErrorMessage As String = Nothing

            If CmsTable.SaveDataBytran(Pst, dbs, OneSyndata.target_resid, rows, OneSyndata.uniquecolumns, alistofRowstate, listofdataReturn, listofhash2Save, strRightsErrorMessage, strErrorMessage) = False Then
                dbs.TransactionRollback()
            End If


        Catch ex As Exception
            dbs.TransactionRollback()
            strErrorMessage = ex.Message.ToString()
            Return False
        End Try

        Return True

    End Function
    Public Function PushData(ByVal rows As ArrayList, ByRef strErrorMessage As String) As Boolean
        Dim rt As PlatformResultModel
        Dim blrt As Boolean

        If OneSyndata.pushtype = "web" Then
            rt = PushData2Web(rows)
            If rt.Error = "0" Then
                blrt = True
            Else
                blrt = False
                strErrorMessage = rt.Message
            End If
        Else

                blrt = PushData2Client(rows, strErrorMessage)
        End If
        Return blrt
    End Function
    Public Function PushData2Web(ByVal rows As ArrayList) As PlatformResultModel

        Dim param As Hashtable = New Hashtable()
        param.Add("resid", OneSyndata.target_resid)
        param.Add("data", JsonConvert.SerializeObject(rows))
        param.Add("uniquecolumns", OneSyndata.uniquecolumns)
        param.Add("withoutdata", OneSyndata.withoutdata)
        Return SaveData2Web(param, OneSyndata.pushurl, OneSyndata.pushuser, OneSyndata.pushupass)

    End Function
    Public Function PushData2Client(ByVal rows As ArrayList, ByRef strErrorMessage As String) As Boolean
        Pst.Dbc = OneSyndata.pushdbc
        Dim alistofRowstate As ArrayList = Nothing

        Dim listofdataReturn As List(Of Hashtable) = Nothing

        Return SaveData2Client(rows, OneSyndata.pushdbc, strErrorMessage, listofdataReturn)


    End Function

    Public Function FetchWebSourceData(ByVal intIndex As Integer, ByVal intSize As Integer, ByRef errmsg As String) As List(Of Hashtable)


        Dim bs As New BaseService()
        bs.PlatformWwwUrl = OneSyndata.fetchurl
        bs.PlatformUser = OneSyndata.fetchuser
        bs.PlatformPassword = OneSyndata.fetchupass
        Dim param As Hashtable = New Hashtable()
        Dim listofdata As New ArrayList()
        Dim cmscolumns As New ArrayList
        Dim rows As New List(Of Hashtable)

        param.Add("resid", OneSyndata.source_resid)
        param.Add("cmswhere", OneSyndata.cmswhere)
        param.Add("pagesize", intSize)
        param.Add("pageindex", intIndex)
        param.Add("cmscolumns", OneSyndata.sourcefields)
        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            rt = bs.Post(bs.getMethod, param)
            If (rt.Error = 0) Then
                rows = JsonConvert.DeserializeObject(Of List(Of Hashtable))(rt.Data.ToString())
                errmsg = ""
            Else
                errmsg = rt.Message
            End If
        Catch ex As Exception
            errmsg = ex.Message.ToString()
        End Try
        Return rows

    End Function
    Public Function getWebSourceDataCount(ByRef errmsg As String, ByVal resid As String, ByVal cmswhere As String, ByVal fetchurl As String, ByVal fetchuser As String, ByVal fetchupass As String) As Long
        Dim total As Long = 0
        Dim param As Hashtable = New Hashtable()
        param.Add("resid", resid)
        param.Add("cmswhere", cmswhere)
        Dim bs As New BaseService()
        bs.PlatformWwwUrl = fetchurl
        bs.PlatformUser = fetchuser
        bs.PlatformPassword = fetchupass
        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            rt = bs.Post("Ajax_CountByWhere", param)
            If (rt.Error = 0) Then
                total = rt.Total
                errmsg = ""
            Else
                errmsg = rt.Message
            End If
        Catch ex As Exception
            errmsg = ex.Message.ToString()
        End Try
        Return total
    End Function

    Public Function getClientSourceDataCount(ByRef errmsg As String, ByVal dbc As DbConfig, ByVal resid As Long, ByVal cmswhere As String) As Long
        Dim total As Long = 0



        Pst.Dbc = dbc
        Return CmsTable.CountByWhere(Pst, resid, "REC_ID", cmswhere)
    End Function

    Public Function getDataCount(ByRef errmsg As String, ByVal dbc As DbConfig, ByVal resid As Long, ByVal cmswhere As String, ByVal sourcetype As String, ByVal url As String, ByVal user As String, ByVal upass As String) As Long
        If sourcetype = "web" Then
            Return getWebSourceDataCount(errmsg, resid, cmswhere, url, user, upass)
        Else
            Return getClientSourceDataCount(errmsg, dbc, resid, cmswhere)

        End If
    End Function
    Public Sub DealSynThread()
        Try

            If (OneSyndata.Active = "Y") Then
                SLog.Crucial("开始同步:" + OneSyndata.uniquenameofsynname)
                DealSyn()
                SLog.Crucial("同步结束:" + OneSyndata.uniquenameofsynname + "，源记录数:" + intTotal.ToString() + "，已同步的记录数:" + SyncroRows.ToString())
            End If
        Catch ex As Exception
            SLog.Err("处理同步失败-" + OneSyndata.uniquenameofsynname + ex.Message)
        End Try

    End Sub
    Public Sub DealSyn()
        Dim param As Hashtable = New Hashtable()
        Dim row As New Hashtable
        Dim rows As New ArrayList
        Dim monitorbatchid As Long
        Dim strErrorMessage As String = ""

        Dim intPagecount As Long = 0
        Dim intIndex As Long = 0
        param.Add("resid", OneSyndata.monitor_resid)
        row.Add(OneSyndata.monitoridcolumn, OneSyndata.uniquenameofsynname)
        row.Add("_id", 1)
        row.Add("_state", "added")
        rows.Add(row)
        param.Add("data", JsonConvert.SerializeObject(rows))
        '生成同步监控记录
        '同步监控记录编号
        If OneSyndata.pushtype = "web" Then
            Dim rt As PlatformResultModel = SaveData2Web(param, OneSyndata.pushurl, OneSyndata.pushuser, OneSyndata.pushupass)
            If rt.Error = 0 Then
                monitorbatchid = JsonConvert.DeserializeObject(Of ArrayList)(rt.Data.ToString())(0)("REC_ID").ToString()
            Else
                SLog.Err("web 添加监控记录失败:" + rt.Message)
            End If
        Else

            Dim listofdataReturn As New List(Of Hashtable)

            If SaveData2Client(rows, OneSyndata.pushdbc, strErrorMessage, listofdataReturn) Then
                Dim record As Hashtable = DirectCast(listofdataReturn.Item(0), Hashtable)
                monitorbatchid = Convert.ToInt64(record("REC_ID"))
            Else
                SLog.Err("client 添加监控记录失败:" + strErrorMessage)

            End If

        End If


        If monitorbatchid > 0 Then
            '先fetch rowscount ,根据pagesize分页获取rows
            intTotal = getDataCount(strErrorMessage, OneSyndata.fetchdbc, OneSyndata.source_resid, OneSyndata.cmswhere, OneSyndata.fetchtype, OneSyndata.fetchurl, OneSyndata.fetchuser, OneSyndata.fetchupass)
            If strErrorMessage <> "" Then
                SLog.Err(strErrorMessage)
                Return
            End If
            If intTotal > 0 Then
                intPagecount = intTotal / OneSyndata.pagesize
                For i As Int32 = 0 To intPagecount
                    '根据pagesize分页获取rows
                    Dim aRows As New ArrayList
                    Dim strErr As String = ""
                    If GetSourceDataByPage(aRows, strErrorMessage, i, monitorbatchid) Then
                        If Not PushData(aRows, strErrorMessage) Then
                            If strErrorMessage <> "" Then
                                SLog.Err(strErrorMessage)
                                Exit For
                            End If
                        Else

                            SyncroRows = SyncroRows + aRows.Count
                            SLog.Info("同步任务:" + OneSyndata.uniquenameofsynname + ",当前同步完成记录数:" + SyncroRows.ToString())
                        End If
                    Else
                        If strErrorMessage <> "" Then
                            SLog.Err(strErrorMessage)
                            Exit For
                        End If
                    End If

                    '将rows转成可提交的数据
                    '提交数据
                Next
            End If

        End If


    End Sub
End Class
