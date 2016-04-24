Imports System
Imports System.IO
Imports System.Net
Imports System.Object
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Public Class ManifestTool
    Dim outFile As System.IO.StreamWriter

    ' Allocate the shared Manifest memory
    Dim manifest_file As String = ""
    Dim localLines As Object
    Dim num_rows As Long = 0
    Dim one_line As Object
    Dim num_cols As Long = 0

    Dim mRecord As Integer = 0

    Private Function PrintByteArray(ByVal array() As Byte) As String
        ' Return the checksum converted in a string 
        Dim i As Integer
        Dim pcksum As String = ""
        For i = 0 To array.Length - 1
            pcksum = pcksum & Format(String.Format("{0:X2}", array(i)))
        Next i
        Return pcksum
    End Function 'PrintByteArray

    Private Sub getManifest(ByRef array As String, ByRef rows As Long, ByRef cols As Long, ByVal fileName As String)
        Dim localLines As Object
        Dim oneLine As Object

        ' Load the Live manifest file into memory
        Dim inFile As StreamReader = My.Computer.FileSystem.OpenTextFileReader(fileName)

        array = inFile.ReadToEnd()

        ' split it into individual lines
        localLines = Split(array, vbCrLf)
        ' determine the number of files
        ' num_rows becoming non-zero indicates that the manifest has been downloaded and placed into memory
        rows = UBound(localLines)
        ' break the first line into fields 
        oneLine = Split(localLines(0), ",")
        ' determine the number of fields
        cols = UBound(oneLine)

    End Sub 'getManifest

    Private Function getSelectedAction() As String
        ' Finds and returns the first selected box.
        ' Box properties are to allow only one entry to be selected so returning
        '  first selection should present no surprises

        Dim i As Integer
        ' Loop through all items the ListBox.
        For i = 0 To FileActionBox.Items.Count - 1

            ' Determine if the item is selected.
            If FileActionBox.GetSelected(i) = True Then
                Return Format(i, "G")
            End If
        Next i

        Return Nothing ' Returning Nothing is an error condition
    End Function 'getSelectedAction

    Private Sub DriverBrowseButton_Click(sender As Object, e As EventArgs) Handles DriverBrowseButton.Click
        Dim fd As OpenFileDialog = New OpenFileDialog()

        fd.Title = "Select driver file"
        fd.Filter = "Manifest file (Manifest)|Manifest|csv files (*.csv)|*.csv|All files (*.*)|*.*"
        fd.FilterIndex = 2
        fd.RestoreDirectory = True

        If fd.ShowDialog() = DialogResult.OK Then
            DriverTextBox.Text = fd.FileName
        End If
    End Sub 'DriverBrowseButton_Click

    Private Sub ChecksumDirecoryBrowseButton_Click(sender As Object, e As EventArgs) Handles ChecksumDirecoryBrowseButton.Click
        Dim fd As New FolderBrowserDialog

        ' Set the Help text description for the FolderBrowserDialog.
        fd.Description =
            "Select where to find the files to do the checksum on"
        ' Turn off the create new folder button
        fd.ShowNewFolderButton = False
        ' Initialize with current directory
        fd.SelectedPath = My.Computer.FileSystem.CurrentDirectory

        If (fd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            ChecksumDirectoryText.Text = fd.SelectedPath & "\"
        End If
    End Sub 'ChecksumDirecoryBrowseButton_Click

    Private Sub OutputBrowseButton_Click(sender As Object, e As EventArgs) Handles OutputBrowseButton.Click
        Dim fd As New FolderBrowserDialog

        ' Set the Help text description for the FolderBrowserDialog.
        fd.Description =
            "Select where to write the new manifest file"
        ' Turn off the create new folder button
        fd.ShowNewFolderButton = True
        ' Initialize with current directory
        fd.SelectedPath = My.Computer.FileSystem.CurrentDirectory

        If (fd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            OutputTextBox.Text = fd.SelectedPath & "\"
        End If
    End Sub 'OutputBrowseButton_Click

    Private Sub ManifestTool_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub fill_record(ByVal index As Integer)

        Dim md5Hash As MD5 = MD5.Create()
        Dim hashValue() As Byte

        If index < num_rows Then
            one_line = Split(localLines(index), ",")
        End If

        'Fill in the filename
        FileNameText.Text = one_line(0)
        ' Initialize Checksum/comment from Driver File
        ChecksumText.Text = one_line(2)

        ' Fill in the action and checksum
        If one_line(1) = "0" Then '0 - Delete File Or Directory
            FileActionBox.SetSelected(0, True)
            Label5.Text = "Directory"
            Label6.Text = "Comment"
        End If
        If one_line(1) = "1" Then '1 - Create Directory
            FileActionBox.SetSelected(1, True)
            Label5.Text = "Directory"
            Label6.Text = "Comment"
        End If
        If (one_line(1) = "2" Or one_line(1) = "3" Or
            one_line(1) = "4" Or one_line(1) = "5" Or
            one_line(1) = "6" Or one_line(1) = "7") Then '2 through 7
            FileActionBox.SetSelected(2, True)
            If one_line(1) = "2" Then '2 - Required for Legal copy - can copy
                FileActionBox.SetSelected(2, True)
            End If
            If one_line(1) = "3" Then '3 - Required for Legal copy - do Not copy
                FileActionBox.SetSelected(3, True)
            End If
            If one_line(1) = "4" Then '4 - SOE files
                FileActionBox.SetSelected(4, True)
            End If
            If one_line(1) = "5" Then '5 - SOE files - user editable
                FileActionBox.SetSelected(5, True)
            End If
            If one_line(1) = "6" Then '6 - Server Specific Files
                FileActionBox.SetSelected(6, True)
            End If
            If one_line(1) = "7" Then '7 - End User License Agreement
                FileActionBox.SetSelected(7, True)
            End If
            Label5.Text = "Filename"

            If File.Exists(ChecksumDirectoryText.Text & FileNameText.Text) Then
                ' Create a fileStream for the file. 
                Dim fileStream As FileStream = File.Open(ChecksumDirectoryText.Text & FileNameText.Text,
                                                         FileMode.Open, FileAccess.Read, FileShare.None)
                ' Be sure it's positioned to the beginning of the stream.
                fileStream.Position = 0
                ' Compute the hash of the fileStream.
                hashValue = md5Hash.ComputeHash(fileStream)

                ' Close the file.
                fileStream.Close()

                ChecksumText.Text = PrintByteArray(hashValue)
            End If
            Label6.Text = "Checksum"
        End If

        If one_line(1) = "8" Then '8 - Create Or Change Registry Entry
            FileActionBox.SetSelected(8, True)
            Label5.Text = "Registry Entry"
            Label6.Text = "Value"
        End If

        If one_line(1) = "9" Then '9 - Delete Registry Entry
            FileActionBox.SetSelected(9, True)
            Label5.Text = "Registry Entry"
            Label6.Text = "Comment"
        End If

        InputLineText.Text = "Processing Entry " & Format(index, "G")

        Me.Refresh()
    End Sub ' fill_record

    Private Sub ManualButton_Click(sender As Object, e As EventArgs) Handles ManualButton.Click
        ' First open the input file
        If Not My.Computer.FileSystem.FileExists(DriverTextBox.Text) Then
            Label1.Text = "Input files does not exist"
            Return
        End If

        ' Read the driver file into memory
        getManifest(manifest_file, num_rows, num_cols, DriverTextBox.Text)
        ' split the manifestinto record lines
        localLines = Split(manifest_file, vbCrLf)

        'For Debug purposes display Manifest information
        'MsgBox("Minifest file contains" & vbCrLf &
        'Format(num_rows + 1, "G") & " Lines of " &
        'Format(num_cols + 1, "G") & " Elements")
        'Return

        ' Set up screen to start output
        OutputTextBox.Text = OutputTextBox.Text & "Manifest"
        Label3.Text = "Output Manifest File"

        If My.Computer.FileSystem.FileExists(OutputTextBox.Text) Then
            ' Deleting file
            My.Computer.FileSystem.DeleteFile(OutputTextBox.Text,
                                              FileIO.UIOption.OnlyErrorDialogs,
                                              FileIO.RecycleOption.SendToRecycleBin,
                                              FileIO.UICancelOption.DoNothing)
        End If

        ' Create empty manifest file
        File.Create(OutputTextBox.Text).Dispose()
        ' Open output stream for manifest file
        outFile = My.Computer.FileSystem.OpenTextFileWriter(OutputTextBox.Text, False)

        ' Read First File Entry
        mRecord = 0
        fill_record(mRecord)
    End Sub 'ManualButton_Click

    Private Sub BatchButton_Click(sender As Object, e As EventArgs) Handles BatchButton.Click
        ' First open the input file
        If Not My.Computer.FileSystem.FileExists(DriverTextBox.Text) Then
            Label1.Text = "Input files does not exist"
            Return
        End If

        ' Read the driver file into memory
        getManifest(manifest_file, num_rows, num_cols, DriverTextBox.Text)
        ' split the manifestinto record lines
        localLines = Split(manifest_file, vbCrLf)

        'For Debug purposes display Manifest information
        'MsgBox("Minifest file contains" & vbCrLf &
        'Format(num_rows + 1, "G") & " Lines of " &
        'Format(num_cols + 1, "G") & " Elements")
        'Return

        ' Set up screen to start output
        OutputTextBox.Text = OutputTextBox.Text & "Manifest"
        Label3.Text = "Output Manifest File"

        If My.Computer.FileSystem.FileExists(OutputTextBox.Text) Then
            ' Deleting file
            My.Computer.FileSystem.DeleteFile(OutputTextBox.Text,
                                              FileIO.UIOption.OnlyErrorDialogs,
                                              FileIO.RecycleOption.SendToRecycleBin,
                                              FileIO.UICancelOption.DoNothing)
        End If

        ' Create empty manifest file
        File.Create(OutputTextBox.Text).Dispose()
        ' Open output stream for manifest file
        outFile = My.Computer.FileSystem.OpenTextFileWriter(OutputTextBox.Text, False)

        ' Read First File Entry
        mRecord = 0

        Do While mRecord < num_rows
            fill_record(mRecord)

            ' Write current entry
            ' If the file exists and a checksum is needed for the type
            ' fill_record() will automatically update the checksum

            If Not FileNameText.Text = Nothing Then
                ' Write the record
                outFile.WriteLine(FileNameText.Text & "," & getSelectedAction() & "," & ChecksumText.Text)
            End If

            mRecord = mRecord + 1

            ' For small files give the user a chance to see that they are being processed
            ' Sleep for .25 seconds
            Thread.Sleep(250)
        Loop

        ' Let user know that the batch processing is done
        MsgBox("Batch Processing complete" & vbCrLf & "Enter new entry and Next Entry to add more files" & vbCrLf & "or Done if complete")

        ' Setup for adding additional records
        FileActionBox.SetSelected(6, True)
        FileNameText.Text = Nothing
        Label5.Text = "Filename"
        ChecksumText.Text = Nothing
        Label6.Text = "Checksum"

    End Sub ' BatchButton_Click

    Private Sub DoneButton_Click(sender As Object, e As EventArgs) Handles DoneButton.Click
        Try
            ' Write current entry
            If Not FileNameText.Text = Nothing Then
                ' Write the record
                outFile.WriteLine(FileNameText.Text & "," & getSelectedAction() & "," & ChecksumText.Text)
            End If

            outFile.Close()
        Catch ex As Exception
            ' Do Nothing
        End Try
        Environment.Exit(0)
    End Sub 'DoneButton_Click

    Private Sub ChecksumButton_Click(sender As Object, e As EventArgs) Handles ChecksumButton.Click
        If File.Exists(ChecksumDirectoryText.Text & FileNameText.Text) Then
            Dim md5Hash As MD5 = MD5.Create()
            Dim hashValue() As Byte

            ' Create a fileStream for the file. 
            Dim fileStream As FileStream = File.Open(ChecksumDirectoryText.Text & FileNameText.Text,
                                                     FileMode.Open, FileAccess.Read, FileShare.None)
            ' Be sure it's positioned to the beginning of the stream.
            fileStream.Position = 0
            ' Compute the hash of the fileStream.
            hashValue = md5Hash.ComputeHash(fileStream)

            ' Close the file.
            fileStream.Close()

            ChecksumText.Text = PrintByteArray(hashValue)
        End If
    End Sub 'ChecksumButton_Click

    Private Sub NextButton_Click(sender As Object, e As EventArgs) Handles NextButton.Click

        ' Write current entry
        If Not FileNameText.Text = Nothing Then
            ' Write the record
            outFile.WriteLine(FileNameText.Text & "," & getSelectedAction() & "," & ChecksumText.Text)
        End If

        If mRecord < num_rows Then
            ' Read Next File Entry
            mRecord = mRecord + 1
            fill_record(mRecord)
        Else
            FileActionBox.SetSelected(6, True)
            FileNameText.Text = Nothing
            Label5.Text = "Filename"
            ChecksumText.Text = Nothing
            Label6.Text = "Checksum"
        End If

    End Sub 'NextButton_Click

    Private Sub FileActionBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles FileActionBox.SelectedIndexChanged

        Dim actionType As String = getSelectedAction()

        If (actionType = "0" Or actionType = "1") Then
            Label5.Text = "Directory"
            Label6.Text = "Comment"
        End If

        If (actionType = "2" Or actionType = "3" Or
            actionType = "4" Or actionType = "5" Or
            actionType = "6" Or actionType = "7") Then
            Label5.Text = "Filename"
            Label6.Text = "Checksum"
        End If

        If (actionType = "8" Or actionType = "9") Then
            Label5.Text = "Registry Entry"
            Label6.Text = "Value"
        End If

    End Sub

End Class
