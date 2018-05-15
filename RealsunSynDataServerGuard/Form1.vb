Imports System.Threading
Imports System.IO

Public Class RealsunSynDataServerGuard

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
        Timer1.Start()
    End Sub
    Private Sub killprocess()
        Dim process As Process() = System.Diagnostics.Process.GetProcesses()


        For Each prc As Process In process
            If prc.ProcessName = "RealsunSynData" Then
                prc.Kill()

            End If
        Next

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        Dim create As Boolean = False
        Try
            If System.Diagnostics.Process.GetProcessesByName("RealsunSynData").Length <= 0 Then
                System.Diagnostics.Process.Start("RealsunSynData.exe")
            End If
            Dim H As Integer = DateAndTime.Now.Hour
            If H = 2 Then
                killprocess()
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
