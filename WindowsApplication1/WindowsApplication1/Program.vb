Imports System.Threading

Public Class Program
    Private Shared frm As New Form1()
    Public Shared Sub Main()
        Try
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)


            AddHandler Application.ThreadException, AddressOf Application_ThreadException
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            System.Windows.Forms.Application.Run(frm)
        Catch ex As Exception
            MessageBox.Show("发生致命错误，请及时联系作者！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Shared Sub Application_ThreadException(sender As Object, e As ThreadExceptionEventArgs)

        System.Windows.Forms.Application.Run(frm)

    End Sub

    Private Shared Sub CurrentDomain_UnhandledException(sender As Object, ex As UnhandledExceptionEventArgs)
        System.Windows.Forms.Application.Run(frm)

    End Sub
End Class
