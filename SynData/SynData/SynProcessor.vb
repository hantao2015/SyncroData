Imports HS.Platform
Imports MiniUiAppCode.Platform
Imports Newtonsoft.Json

Public Class SynProcessor
    Private m_pst As CmsPassport
    Private m_syndata As OneSynDefine

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
        If OneSyndata.fetchtype = "" Then
            errmsg = "missing field of fetchtype"
            Return False
        End If
        If OneSyndata.monitoridcolumn = "" Then
            errmsg = "missing field of monitoridcolumn"
            Return False
        End If
        If OneSyndata.uniquenamesynname = "" Then
            errmsg = "missing field of uniquenamesynname"
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
        If OneSyndata.syntype = "" Then
            errmsg = "missing field of syntype"
            Return False
        End If
        If OneSyndata.syntype <> "push" And OneSyndata.syntype <> "pull" Then
            errmsg = "invalid syntype"
            Return False
        End If

        If OneSyndata.syntype = "push" Then
            If OneSyndata.pushurl = "" Then
                errmsg = "missing field of pushurl"
                Return False
            End If
            If OneSyndata.pushuser = "" Then
                errmsg = "missing field of pushuser"
                Return False
            End If
            If OneSyndata.pushupass = "" Then
                errmsg = "missing field of pushupass"
                Return False
            End If
        End If
        If OneSyndata.syntype = "pull" Then
            If OneSyndata.fetchurl = "" Then
                errmsg = "missing field of fetchurl"
                Return False
            End If
            If OneSyndata.fetchuser = "" Then
                errmsg = "missing field of fetchuser"
                Return False
            End If
            If OneSyndata.fetchupass = "" Then
                errmsg = "missing field of fetchupass"
                Return False
            End If
        End If
        Return True
    End Function
    Public Function GetSourceData(ByRef errmesage As String, ByVal intIndex As Integer) As DataSet

        Try
            Return CmsTable.GetDatasetForHostTable(Pst, OneSyndata.source_resid, False, OneSyndata.cmswhere, "", "", intIndex, OneSyndata.pagesize)
        Catch ex As Exception
            errmesage = ex.Message.ToString()
            Return Nothing
        End Try
    End Function
    Public Function GetSourceDataByPage(ByRef rows As ArrayList, ByRef errmesage As String, ByVal index As Integer, ByRef sourcetype As String) As Boolean
        Dim ds As DataSet
        If sourcetype = "web" Then
            Dim webrows As ArrayList = FetchWebSourceData(index, OneSyndata.pagesize, errmesage)
            If errmesage <> "" Then
                Return False
            End If
            Try
                ds = DBUtil.ConvertToDataSet(Of ArrayList)(webrows)
            Catch ex As Exception
                ds = Nothing
                errmesage = ex.Message.ToString()
                Return False
            End Try
        Else
            ds = GetSourceData(errmesage, index)
        End If

        If ds IsNot Nothing Then

            rows = BaseService.ShowHostTableDatas_Ajax_GetDATA(ds.Tables(0), OneSyndata._state, index, OneSyndata.pagesize, OneSyndata.sourcefields, OneSyndata.targetfields)

        Else
            Return False
        End If
        errmesage = ""
        Return True
    End Function
    Public Function PushMonitorData2Web(ByVal uniquenamesynname As String, ByRef monitorid As String) As PlatformResultModel
        Dim bs As New BaseService()
        bs.PlatformWwwUrl = OneSyndata.pushurl
        bs.PlatformUser = OneSyndata.pushuser
        bs.PlatformPassword = OneSyndata.pushupass
        Dim param As Hashtable = New Hashtable()
        Dim row As Hashtable = New Hashtable()
        Dim rows As ArrayList = New ArrayList()
        row.Add("_state", "added")
        row.Add("_id", "1")
        row.Add("uniquenamesynname", uniquenamesynname)
        rows.Add(row)
        param.Add("resid", OneSyndata.monitor_resid)
        param.Add("data", JsonConvert.SerializeObject(rows))


        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            rt = bs.Post(bs.saveMethod2, param)
            If rt.Error = 0 Then
                monitorid = JsonConvert.DeserializeObject(Of ArrayList)(rt.Data.ToString())(0)("REC_ID").ToString()
            Else
                monitorid = 0
            End If
        Catch ex As Exception
            rt.Error = -1
            rt.Message = ex.Message.ToString()
        End Try
        Return rt

    End Function
    Public Function PushData2Web(ByVal rows As ArrayList) As PlatformResultModel
        Dim bs As New BaseService()
        bs.PlatformWwwUrl = OneSyndata.pushurl
        bs.PlatformUser = OneSyndata.pushuser
        bs.PlatformPassword = OneSyndata.pushupass
        Dim param As Hashtable = New Hashtable()
        param.Add("resid", OneSyndata.target_resid)
        param.Add("data", JsonConvert.SerializeObject(rows))
        param.Add("uniquecolumns", OneSyndata.uniquecolumns)
        param.Add("withoutdata", OneSyndata.withoutdata)
        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            rt = bs.Post(bs.saveMethod2, param)

        Catch ex As Exception
            rt.Error = -1
            rt.Message = ex.Message.ToString()
        End Try
        Return rt

    End Function
    Public Function PushData2Client(ByVal rows As ArrayList) As PlatformResultModel
        Dim bs As New BaseService()
        bs.PlatformWwwUrl = OneSyndata.pushurl
        bs.PlatformUser = OneSyndata.pushuser
        bs.PlatformPassword = OneSyndata.pushupass
        Dim param As Hashtable = New Hashtable()
        param.Add("resid", OneSyndata.target_resid)
        param.Add("data", JsonConvert.SerializeObject(rows))
        param.Add("uniquecolumns", OneSyndata.uniquecolumns)
        param.Add("withoutdata", OneSyndata.withoutdata)
        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            rt = bs.Post(bs.saveMethod2, param)

        Catch ex As Exception
            rt.Error = -1
            rt.Message = ex.Message.ToString()
        End Try
        Return rt

    End Function

    Public Function FetchWebSourceData(ByVal intIndex As Integer, ByVal intSize As Integer, ByRef errmsg As String) As ArrayList


        Dim bs As New BaseService()
        bs.PlatformWwwUrl = OneSyndata.fetchurl
        bs.PlatformUser = OneSyndata.fetchuser
        bs.PlatformPassword = OneSyndata.fetchupass
        Dim param As Hashtable = New Hashtable()
        Dim listofdata As New ArrayList()
        Dim cmscolumns As New ArrayList
        Dim rows As ArrayList = New ArrayList

        param.Add("resid", OneSyndata.source_resid)
        param.Add("cmswhere", OneSyndata.cmswhere)
        param.Add("pagesize", intSize)
        param.Add("pageindex", intIndex)
        Dim rt As PlatformResultModel = New PlatformResultModel()
        Try
            rt = bs.Post(bs.getMethod, param)
            If (rt.Error = 0) Then
                rows = JsonConvert.DeserializeObject(Of ArrayList)(rt.Data.ToString())
                errmsg = ""
            Else
                errmsg = rt.Message
            End If
        Catch ex As Exception
            errmsg = ex.Message.ToString()
        End Try
        Return rows

    End Function
    Public Function FetchWebSourceDataCount(ByRef errmsg As String) As Long
        Dim total As Long = 0
        Return total
    End Function
    Public Function FetchSourceDataCount(ByRef errmsg As String) As Long
        Dim total As Long = 0
        Return total
    End Function
End Class
