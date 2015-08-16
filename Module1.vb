Imports System.IO
Imports System.ConsoleColor
Imports Microsoft.Win32

Module Module1

    Private _loc As String
    Private WithEvents _ps As Process

    Public Sub Main()

        Console.Title = "Cortana Disabler v1.0"

        Try

            CheckTaskbar()

            SetupProcess()

            If _ps.Id <> 0 Then

                _loc = _ps.MainModule.FileName

                Out("Found SearchUI!", Yellow)
                Out("Location: " & _loc & vbCrLf)
                Out("Would you like to disable this completely? y/N", Magenta)

                Dim resp As String = Console.ReadLine()

                If resp.ToLower.Trim() = "y" Then
                    Out("Killing SearchUI...")
                    _ps.Kill()
                    Out("Renaming folder to prevent further execution...")
                End If

            End If

        Catch ex As Exception
            Console.WriteLine("No Cartana detected!")
        End Try

        ExitCmd()

    End Sub

    Private Sub ExitCmd()

        Out(vbCrLf & "Press any key to exit..")
        Console.ReadKey()
        End

    End Sub
    Private Sub SetupProcess()

        _ps = New Process()
        _ps = Process.GetProcessesByName("SearchUI")(0)
        _ps.EnableRaisingEvents = True

        AddHandler _ps.Exited, AddressOf ps_Exited
        AddHandler _ps.ErrorDataReceived, AddressOf ps_Error

    End Sub

    Private Sub ps_Error(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        Out(vbCrLf & "An error was detected! " & vbCrLf & e.Data, Red)
    End Sub

    Private Sub Out(ByVal text As String, Optional ByVal color As ConsoleColor = White)
        Console.ForegroundColor = color
        Console.Write(text & vbCrLf)
    End Sub

    Private Sub ps_Exited(sender As Object, e As EventArgs)

        Out(vbCrLf & "* Successfully killed SearchUI.. Renaming source directory..", Green)

        Dim fiInfo As New FileInfo(_loc)

        My.Computer.FileSystem.RenameDirectory(fiInfo.Directory.FullName, fiInfo.Directory.Name & "1")

        If Directory.Exists(_loc) Then
            Out("Failed to rename... Has process restarted?", Red)
        Else
            Out("Successfully disabled Cortana!")
        End If

        ExitCmd()

    End Sub

    Public Sub CheckTaskbar()

        Try

            Dim regCortana As Integer = Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search", "SearchboxTaskbarMode", "")

            Select Case regCortana

                Case 0 'hidden
                    Out("Taskbar setting is disabled.", Green)
                Case 1 'search icon
                    Out("Taskbar setting is showing Cortana Icon. Please disabled this.", Red)
                Case 2 'search box
                    Out("Taskbar setting is showing Cortana search box. Please disabled this.", Red)

            End Select

        Catch ex As Exception

        End Try

    End Sub

End Module
