Imports System.Threading
Imports HS.Platform
Imports MiniUiAppCode.Platform
Imports Newtonsoft.Json

Public Class SynProcessor
    Private m_pst As CmsPassport
    Private m_syndata As OneSynDefine
    Private m_syndefine As SynDefine
    Private listofhash2Save As New List(Of Hashtable)
    Public Property SyncroRows As Long = 0
    Public Property continueOnError As String = "N"
    Public Property includedatalog As Boolean = False
    Public Property intTotal As Long = 0
    Public Property monitorbatchid As Long = 0
    Public Property taskNotFinished As Hashtable
    Public Delegate Sub printMessage(ByVal msg As String)
    Public printMessageHandler As printMessage = Nothing






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

    Public Sub New(ByVal pst As CmsPassport, ByVal syndefine As SynDefine, ByVal syndata As OneSynDefine)
        Dim errmsg As String = ""
        m_pst = pst
        m_syndata = syndata
        m_syndefine = syndefine
        taskNotFinished = New Hashtable()
        pst = CmsPassport.GenerateCmsPassportBySysuser()
        If Not CheckSyndata(errmsg) Then
            Throw New Exception(errmsg)
        End If

    End Sub
    Public Function CheckSyndata(ByRef errmsg As String) As Boolean
        If OneSyndata.monitor_colofrecords = "" Then
            errmsg = "missing field of monitor_colofrecords"
            Return False
        End If
        If OneSyndata.monitor_colofendtime = "" Then
            errmsg = "missing field of monitor_colofendtime"
            Return False
        End If
        If OneSyndata.monitorlog_colofbatchid = "" Then
            errmsg = "missing field of monitorlog_colofbatchid"
            Return False
        End If
        If OneSyndata.monitorlog_resid = "" Then
            errmsg = "missing field of monitorlog_resid"
            Return False
        End If
        If OneSyndata.monitorlog_coloftime = "" Then
            errmsg = "missing field of monitorlog_coloftime"
            Return False
        End If
        If OneSyndata.monitorlog_colofcontent = "" Then
            errmsg = "missing field of monitorlog_colofcontent"
            Return False
        End If
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
            If OneSyndata.pushmethod = "ajax_InsertBatchData" Then
                rows = BaseService.ShowHostTableDatas_Ajax_GetDATA(dt, OneSyndata._state, OneSyndata.target_synmonitorcolumnofid, Convert.ToString(monitorbatchid), OneSyndata.sourcefields, OneSyndata.targetfields, "getrecid", OneSyndata.target_resid)
            Else
                rows = BaseService.ShowHostTableDatas_Ajax_GetDATA(dt, OneSyndata._state, OneSyndata.target_synmonitorcolumnofid, Convert.ToString(monitorbatchid), OneSyndata.sourcefields, OneSyndata.targetfields, "")
            End If


        Else
            Return False
        End If
        errmesage = ""
        If includedatalog Then
            SLog.Crucial(OneSyndata.uniquenameofsynname + ":data-----" + JsonConvert.SerializeObject(rows))
        End If
        Return True
    End Function

    Public Function SaveData2Web(ByVal data As Hashtable, ByVal url As String, ByVal user As String, ByVal upass As String, Optional ByVal method As String = "") As PlatformResultModel
        Dim bs As New BaseService()
        bs.PlatformWwwUrl = url
        bs.PlatformUser = user
        bs.PlatformPassword = upass
        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            If method = "" Then
                rt = bs.Post(bs.saveMethod2, data)
            Else
                rt = bs.Post(method, data)
            End If


        Catch ex As Exception
            rt.Error = -1
            rt.Message = ex.Message.ToString() + ex.InnerException.Source.ToString()
        End Try
        Return rt
    End Function
    Public Function SaveData2Client(ByVal rows As ArrayList, ByVal dbc As DbConfig, ByRef strErrorMessage As String, ByRef listofdataReturn As List(Of Hashtable)) As Boolean
        Pst.Dbc = dbc
        Dim dbs As DbStatement = New DbStatement(dbc)
        Dim strRightsErrorMessage As String = ""
        Try
            dbs.TransactionBegin()
            Dim alistofRowstate As New ArrayList

            Dim rp As New CmsTableParam



            If CmsTable.SaveDataBytran(Pst, dbs, OneSyndata.target_resid, rows, OneSyndata.uniquecolumns, alistofRowstate, listofdataReturn, listofhash2Save, strRightsErrorMessage, strErrorMessage, "", "", "", rp, False) = False Then
                dbs.TransactionRollback()
            End If


        Catch ex As Exception
            dbs.TransactionRollback()
            strErrorMessage = ex.Message.ToString() + "/strRightsErrorMessage=" + strRightsErrorMessage
            Return False
        End Try
        dbs.TransactionCommit()

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
        Dim str As String = JsonConvert.SerializeObject(rows)


        If includedatalog Then
            SLog.Crucial(OneSyndata.uniquenameofsynname + "--PushData2Web:str-----" + str)
        End If
        param.Add("data", str)
        param.Add("uniquecolumns", OneSyndata.uniquecolumns)
        param.Add("withoutdata", OneSyndata.withoutdata)
        param.Add("bytransvalue", OneSyndata.bytransvalue)

        Return SaveData2Web(param, OneSyndata.pushurl, OneSyndata.pushuser, OneSyndata.pushupass, OneSyndata.pushmethod)

    End Function
    Public Function PushData2Client(ByVal rows As ArrayList, ByRef strErrorMessage As String) As Boolean
        Pst.Dbc = OneSyndata.pushdbc

        Dim listofdataReturn As New List(Of Hashtable)
        Return SaveData2Client(rows, OneSyndata.pushdbc, strErrorMessage, listofdataReturn)
    End Function
    Public Function FetchTaskNotFinished(ByRef errmsg As String, ByVal url As String, ByVal user As String, ByVal upass As String, Optional ByVal method As String = "") As Hashtable

        Dim bs As New BaseService()

        bs.PlatformWwwUrl = url
        bs.PlatformUser = user
        bs.PlatformPassword = upass
        SLog.Crucial("FetchTaskNotFinished:PlatformWwwUrl:" + url)
        SLog.Crucial("FetchTaskNotFinished:PlatformUser:" + user)
        SLog.Crucial("FetchTaskNotFinished:PlatformPassword:" + upass)
        Dim param As Hashtable = New Hashtable()
        Dim listofdata As New ArrayList()
        Dim cmscolumns As New ArrayList
        Dim rows As New List(Of Hashtable)
        Dim row As New Hashtable

        Dim cmswhere As String = "uniquenamesynname='" + OneSyndata.uniquenameofsynname + "' and C3_547583460116<>'Y'"

        param.Add("resid", "542290864273")
        param.Add("cmswhere", cmswhere)


        Dim rt As PlatformResultModel = New PlatformResultModel()
        Thread.Sleep(500)
        Try
            rt = bs.Post(bs.getMethod, param)
            If (rt.Error = 0) Then
                rows = JsonConvert.DeserializeObject(Of List(Of Hashtable))(rt.Data.ToString())
                SLog.Crucial("FetchTaskNotFinished:rt.Data.ToString():" + rt.Data.ToString())
                If (rows.Count > 0) Then
                    row = DirectCast(rows.Item(0), Hashtable)
                    errmsg = ""
                End If

            Else
                SLog.Crucial("FetchTaskNotFinished: rt.Message:" + rt.Message)
                errmsg = rt.Message
            End If
        Catch ex As Exception
            errmsg = ex.Message.ToString()
        End Try
        SLog.Crucial("FetchTaskNotFinished:errmsg:" + errmsg)
        SLog.Crucial("FetchTaskNotFinished:cmswhere:" + cmswhere)
        SLog.Crucial("FetchTaskNotFinished:resid:" + OneSyndata.monitor_resid)
        Return row
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
    Public Sub doPrintMessage(ByVal msg As String)
        If Not printMessageHandler Is Nothing Then
            printMessageHandler.BeginInvoke(msg, New AsyncCallback(AddressOf endDoPrintMessage), New Object())
        End If
    End Sub
    Public Sub endDoPrintMessage()

    End Sub
    Public Sub getSyndataFromHost()
        Dim bs As New BaseService()
        bs.PlatformWwwUrl = m_syndefine.baseUrl
        bs.PlatformUser = m_syndefine.user
        bs.PlatformPassword = m_syndefine.upass
        Dim param As Hashtable = New Hashtable()
        Dim listofdata As New ArrayList()
        Dim cmscolumns As New ArrayList
        Dim rows As New List(Of Hashtable)
        Dim errmsg As String
        param.Add("resid", "543946705485")
        param.Add("cmswhere", "C3_543946721063='" + OneSyndata.uniquenameofsynname + "'")
        param.Add("saveandget", "1")
        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            rt = bs.Post(m_syndefine.getOneRowMethod, param)
            If (rt.Error = 0) Then
                rows = JsonConvert.DeserializeObject(Of List(Of Hashtable))(rt.Data.ToString())
                errmsg = ""
            Else
                errmsg = rt.Message
            End If
        Catch ex As Exception
            errmsg = ex.Message.ToString()
        End Try
        If rows.Count > 0 Then
            Try
                Dim oneRecord As Hashtable = DirectCast(rows.Item(0), Hashtable)
                OneSyndata.cmswhere = Convert.ToString(oneRecord("C3_544372094587"))
                OneSyndata.sysActive = Convert.ToString(oneRecord("C3_543946869297"))
                OneSyndata.frequency = Convert.ToInt64(oneRecord("C3_543946898531"))
                continueOnError = Convert.ToString(oneRecord("C3_544477369294"))
            Catch ex As Exception
                OneSyndata.cmswhere = ""
                OneSyndata.sysActive = "N"
                continueOnError = "N"
                OneSyndata.frequency = 5
            End Try

        Else
            OneSyndata.sysActive = "N"
        End If
    End Sub
    Public Sub DealSynThread()
        Dim strMsg As String = "开始同步:" + OneSyndata.uniquenameofsynname
        Try

            If (OneSyndata.Active = "Y") Then
                getSyndataFromHost()
                If OneSyndata.sysActive = "Y" Then
                    Dim batchid As Long = TimeId.CurrentMillisecondsThreadSafe()
                    taskNotFinished.Clear()
                    StartMonitor()
                    SLog.Crucial(strMsg)
                    doPrintMessage(strMsg)
                    MonitorLog(strMsg)
                    DealSyn()
                    strMsg = "同步结束:" + OneSyndata.uniquenameofsynname + "，源记录数:" + intTotal.ToString() + "，已同步的记录数:" + SyncroRows.ToString()
                    SLog.Crucial(strMsg)
                    MonitorLog(strMsg)
                    doPrintMessage(strMsg)
                    EndMonitor()
                End If
                Thread.Sleep(OneSyndata.frequency * 1000)
                DealSynThread()
            End If
        Catch ex As Exception
            taskNotFinished.Clear()
            strMsg = "处理同步失败-" + OneSyndata.uniquenameofsynname + ex.Message
            SLog.Err(strMsg)
            MonitorLog(strMsg)
            doPrintMessage(strMsg)
        End Try

    End Sub
    Public Sub UpdateProgress(ByVal curRecordCount As Long)
        Dim param As Hashtable = New Hashtable()
        Dim row As New Hashtable
        Dim rows As New ArrayList
        Dim strErrorMessage As String = ""
        Dim intPagecount As Long = 0
        Dim intIndex As Long = 0
        param.Add("resid", OneSyndata.monitor_resid)
        row.Add("REC_ID", monitorbatchid)
        row.Add(OneSyndata.monitor_colofendtime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"))
        row.Add(OneSyndata.monitor_colofrecords, intTotal)
        row.Add("C3_545046524620", curRecordCount)
        row.Add("_id", 1)
        row.Add("_state", "modified")
        rows.Add(row)
        param.Add("data", JsonConvert.SerializeObject(rows))
        'If OneSyndata.pushtype = "web" Then
        'Dim rt As PlatformResultModel = SaveData2Web(param, OneSyndata.pushurl, OneSyndata.pushuser, OneSyndata.pushupass)
        Dim rt As PlatformResultModel = SaveData2Web(param, m_syndefine.baseUrl, m_syndefine.user, m_syndefine.upass)
        If rt.Error = 0 Then
                monitorbatchid = JsonConvert.DeserializeObject(Of ArrayList)(rt.Data.ToString())(0)("REC_ID").ToString()
            Else
                Dim strMsg As String = "web 添加监控记录失败:" + rt.Message
                SLog.Err(strMsg)
                MonitorLog(strMsg)
                doPrintMessage(strMsg)
            End If
        'Else

        '    Dim listofdataReturn As New List(Of Hashtable)
        '    Pst.Dbc = OneSyndata.pushdbc
        '    Try
        '        Dim cr As CmsTableReturn = CmsTable.EditRecord(Pst, OneSyndata.monitor_resid, Convert.ToInt64(monitorbatchid), row)

        '    Catch ex As Exception


        '        Dim strMsg As String = "client 添加监控记录失败:" + ex.Message.ToString
        '        SLog.Err(strMsg)
        '        MonitorLog(strMsg)
        '        doPrintMessage(strMsg)
        '    End Try


        'End If

    End Sub
    Public Sub MonitorLog(ByVal content As String)
        Dim param As Hashtable = New Hashtable()
        Dim row As New Hashtable
        Dim rows As New ArrayList
        Dim strErrorMessage As String = ""
        Dim intPagecount As Long = 0
        Dim intIndex As Long = 0

        param.Add("resid", OneSyndata.monitorlog_resid)
        row.Add(OneSyndata.monitorlog_colofbatchid, monitorbatchid)
        row.Add(OneSyndata.monitorlog_colofcontent, content)
        row.Add(OneSyndata.monitorlog_coloftime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"))
        row.Add("_id", 1)
        row.Add("_state", "added")
        rows.Add(row)
        param.Add("data", JsonConvert.SerializeObject(rows))
        'If OneSyndata.pushtype = "web" Then
        'Dim rt As PlatformResultModel = SaveData2Web(param, OneSyndata.pushurl, OneSyndata.pushuser, OneSyndata.pushupass)
        Dim rt As PlatformResultModel = SaveData2Web(param, m_syndefine.baseUrl, m_syndefine.user, m_syndefine.upass)
        If rt.Error = 0 Then

            Else
                Dim strMsg As String = "web 添加监控日志记录失败:" + rt.Message
                SLog.Err(strMsg)
                doPrintMessage(strMsg)
            End If
        'Else

        'Dim listofdataReturn As New List(Of Hashtable)
        'Pst.Dbc = OneSyndata.pushdbc
        'Try
        '    CmsTable.AddRecord(Pst, OneSyndata.monitorlog_resid, row)
        'Catch ex As Exception

        '    Dim strMsg As String = "client 添加监控日志记录失败:" + ex.Message.ToString()
        '    SLog.Err(strMsg)
        '    doPrintMessage(strMsg)
        'End Try


        'End If
    End Sub
    Public Sub EndMonitor()
        Dim param As Hashtable = New Hashtable()
        Dim row As New Hashtable
        Dim rows As New ArrayList
        Dim strErrorMessage As String = ""
        Dim intPagecount As Long = 0
        Dim intIndex As Long = 0
        param.Add("resid", OneSyndata.monitor_resid)
        row.Add("REC_ID", monitorbatchid)
        row.Add(OneSyndata.monitor_colofendtime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"))
        row.Add(OneSyndata.monitor_colofrecords, intTotal)
        row.Add("_id", 1)
        row.Add("_state", "modified")
        rows.Add(row)
        param.Add("data", JsonConvert.SerializeObject(rows))
        'If OneSyndata.pushtype = "web" Then
        '  Dim rt As PlatformResultModel = SaveData2Web(param, OneSyndata.pushurl, OneSyndata.pushuser, OneSyndata.pushupass)
        Dim rt As PlatformResultModel = SaveData2Web(param, m_syndefine.baseUrl, m_syndefine.user, m_syndefine.upass)
            If rt.Error = 0 Then
                monitorbatchid = JsonConvert.DeserializeObject(Of ArrayList)(rt.Data.ToString())(0)("REC_ID").ToString()
            Else
                Dim strMsg As String = "web 添加监控记录失败:" + rt.Message
                SLog.Err(strMsg)
                MonitorLog(strMsg)
                doPrintMessage(strMsg)
            End If
        'Else

        'Dim listofdataReturn As New List(Of Hashtable)
        '    Pst.Dbc = OneSyndata.pushdbc
        '    Try
        '        Dim cr As CmsTableReturn = CmsTable.EditRecord(Pst, OneSyndata.monitor_resid, Convert.ToInt64(monitorbatchid), row)

        '    Catch ex As Exception


        '        Dim strMsg As String = "client 添加监控记录失败:" + ex.Message.ToString
        '        SLog.Err(strMsg)
        '        MonitorLog(strMsg)
        '        doPrintMessage(strMsg)
        '    End Try


        'End If


    End Sub
    Public Sub StartMonitor()
        Dim param As Hashtable = New Hashtable()
        Dim row As New Hashtable
        Dim rows As New ArrayList
        Dim strErrorMessage As String = ""
        Dim intPagecount As Long = 0
        Dim intIndex As Long = 0
        Dim strMsg As String = ""
        SLog.Crucial("FetchTaskNotFinished")
        Dim taskRow As Hashtable = FetchTaskNotFinished(strMsg, m_syndefine.baseUrl, m_syndefine.user, m_syndefine.upass)

        If strMsg = "" And taskRow.Count > 0 Then
            monitorbatchid = Convert.ToInt64(taskRow("REC_ID"))
            taskNotFinished = taskRow
        Else
            param.Add("resid", OneSyndata.monitor_resid)
            row.Add(OneSyndata.monitoridcolumn, OneSyndata.uniquenameofsynname)
            row.Add("_id", 1)
            row.Add("_state", "added")
            rows.Add(row)
            param.Add("data", JsonConvert.SerializeObject(rows))
            Dim rt As PlatformResultModel = SaveData2Web(param, m_syndefine.baseUrl, m_syndefine.user, m_syndefine.upass)
            If rt.Error = 0 Then
                monitorbatchid = JsonConvert.DeserializeObject(Of ArrayList)(rt.Data.ToString())(0)("REC_ID").ToString()
            Else

                strMsg = "web 添加监控记录失败:" + rt.Message
                SLog.Err(strMsg)
                MonitorLog(strMsg)
                doPrintMessage(strMsg)
            End If
        End If



    End Sub
    Public Sub DealSyn()
        Dim strErrorMessage As String = ""
        Dim intPagecount As Long = 0
        Dim intIndex As Long = 0
        Dim finishedCount As Int32 = 0
        SyncroRows = 0
        intTotal = 0
        Dim startIndex As Long = 0
        If monitorbatchid > 0 Then
            '先fetch rowscount ,根据pagesize分页获取rows
            SLog.Crucial("taskNotFinished:" + JSON.Encode(taskNotFinished))
            SLog.Crucial("monitorbatchid:" + JSON.Encode(monitorbatchid))
            If (taskNotFinished.Count > 0) Then
                intTotal = Convert.ToInt32(taskNotFinished("C3_542630836091"))
                finishedCount = Convert.ToInt32(taskNotFinished("C3_545046524620"))
                SyncroRows = finishedCount
                startIndex = finishedCount / OneSyndata.pagesize
            Else
                intTotal = getDataCount(strErrorMessage, OneSyndata.fetchdbc, OneSyndata.source_resid, OneSyndata.cmswhere, OneSyndata.fetchtype, OneSyndata.fetchurl, OneSyndata.fetchuser, OneSyndata.fetchupass)
            End If

            If strErrorMessage <> "" Then
                MonitorLog(strErrorMessage)
                SLog.Err(strErrorMessage)
                Return
            End If
            SLog.Crucial("startIndex:" + startIndex.ToString())
            If intTotal > 0 Then
                intPagecount = intTotal / OneSyndata.pagesize
                For i As Int32 = startIndex To intPagecount
                    '根据pagesize分页获取rows
                    Dim aRows As New ArrayList
                    Dim strErr As String = ""
                    If GetSourceDataByPage(aRows, strErrorMessage, i, monitorbatchid) Then
                        If Not PushData(aRows, strErrorMessage) Then
                            If strErrorMessage <> "" Then
                                SLog.Err(strErrorMessage)
                                MonitorLog(strErrorMessage)
                                If continueOnError = "Y" Then
                                    Continue For
                                Else
                                    Exit For
                                End If

                            End If
                        Else

                            SyncroRows = SyncroRows + aRows.Count


                            Dim strMsg As String = "同步任务:" + OneSyndata.uniquenameofsynname + ",当前同步完成记录数:" + SyncroRows.ToString()
                            SLog.Err(strMsg)
                            UpdateProgress(SyncroRows)
                            MonitorLog(strMsg)
                            doPrintMessage(strMsg)
                        End If

                    Else
                        If strErrorMessage <> "" Then
                            SLog.Err(strErrorMessage)
                            MonitorLog(strErrorMessage)
                            If continueOnError = "Y" Then
                                Continue For
                            Else
                                Exit For
                            End If
                        End If
                    End If



                Next
            End If

        End If


    End Sub
End Class
