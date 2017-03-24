Imports System.Collections
Public Class SynDefine
    Private m_baseUrl As String
    Property baseUrl As String
        Get
            Return m_baseUrl
        End Get
        Set(value As String)
            m_baseUrl = value

        End Set
    End Property

    Property getMethod As String
    Property saveMethod As String
    Property saveMethod2 As String
    Property getBysqlMethod As String
    Property user As String
    Property upass As String
    Property includedatalog As Boolean
    Property SynDatas As List(Of OneSynDefine)
    Property LoginToken As String

End Class
