Public Class OneSynDefine
    Public source_resid As String
    Public target_resid As String
    Public cmswhere As String
    Public sourcefields As String
    Public targetfields As String
    Public pagesize As Integer = 0
    Public uniquecolumns As String
    Public withoutdata As String = "1"
    Public synchronizedat As String = "1"
    Public pushurl As String
    Public pushuser As String
    Public pushupass As String
    Public _state As String
    Public Property fetchupass As String
    Public Property fetchuser As String
    Public Property fetchurl As String
    Public Property syntype As Boolean
End Class
