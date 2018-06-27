Imports System.Threading
Imports System.IO

Public Class RealsunSynDataServerGuard
    Private _predate As Date

    Private Sub RealsunMiniServerGuard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
        Me.ShowInTaskbar = False
        Me.NotifyIcon1.Visible = True
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Hide()
        Me.ShowInTaskbar = False
        Me.NotifyIcon1.Visible = True
        _predate = DateTime.Now.Date
        Timer1.Start()
    End Sub
    Private Sub killprocess()
        Dim process As Process() = System.Diagnostics.Process.GetProcesses()
        Try
            For Each prc As Process In process
                If prc.ProcessName = "RealsunSynData" Then
                    prc.Kill()

                End If
            Next
        Catch ex As Exception

        End Try



    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim H As Integer = DateAndTime.Now.Hour
        Dim create As Boolean = False
        Try
            If System.Diagnostics.Process.GetProcessesByName("RealsunSynData").Length <= 0 Then
                System.Diagnostics.Process.Start("RealsunSynData.exe")
            Else
                If DateDiff(DateInterval.Day, _predate, DateAndTime.Now.Date) <> 0 Then
                    killprocess()
                    System.Diagnostics.Process.Start("RealsunSynData.exe")
                    _predate = DateAndTime.Now.Date
                End If
            End If


        Catch ex As Exception

        End Try


    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "realsun" Then
            System.Environment.Exit(0)
        Else
            MessageBox.Show("Error Code!")
        End If
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick
        Me.Show()
        Me.ShowInTaskbar = True
    End Sub
End Class
