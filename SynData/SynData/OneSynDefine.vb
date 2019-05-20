Imports HS.Platform

Public Class OneSynDefine
    Public source_resid As String
    Public target_resid As String
    Public temptarget_resid As String

    Public cmswhere As String
    Public sourcefields As String
    Public targetfields As String
    Public pagesize As Integer = 0
    Public uniquecolumns As String
    Public withoutdata As String = "1"
    Public synchronizedat As String = "1"
    Public monitor_resid As String = ""
    Public monitor_colofendtime As String = ""
    Public monitor_colofrecords As String = ""
    Public monitorlog_resid As String = ""
    Public monitorlog_colofbatchid As String = ""
    Public monitorlog_coloftime As String = ""
    Public monitorlog_colofcontent As String = ""
    Public uniquenameofsynname As String = ""
    Public target_synmonitorcolumnofid As String = ""
    Public pushurl As String
    Public pushuser As String
    Public pushupass As String
    Public pushmethod As String = ""
    Public _state As String
    Public Property fetchupass As String
    Public Property fetchuser As String
    Public Property fetchurl As String

    Public Property monitoridcolumn As String
    Public Property pushdbc As DbConfig
    Public Property fetchdbc As DbConfig
    Public Property pushtype As String
    Public Property fetchtype As String
    Property Active As String
    Property sysActive As String = "Y"
    Property bytransvalue As String = "1"
    Property frequency As Int64 = 5

End Class
