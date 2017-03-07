Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json

Public Class PlatformResultModel
    ' Properties
    <JsonProperty("data")>
    Public Property Data As Object



    <JsonProperty("error")>
    Public Property [Error] As Integer



    <JsonProperty("message")>
    Public Property Message As String


    <JsonProperty("token")>
    Public Property Token As String

    <JsonProperty("Total")>
    Public Property Total As Integer


End Class



