Public Class ManifestTool

    Private Sub DriverBrowseButton_Click(sender As Object, e As EventArgs) Handles DriverBrowseButton.Click
        Dim fd As OpenFileDialog = New OpenFileDialog()

        fd.Title = "Select driver file"
        fd.Filter = "Manifest file (Manifest)|Manifest|csv files (*.csv)|*.csv|All files (*.*)|*.*"
        fd.FilterIndex = 2
        fd.RestoreDirectory = True

        If fd.ShowDialog() = DialogResult.OK Then
            DriverTextBox.Text = fd.FileName
        End If
    End Sub

    Private Sub ChecksumDirecoryBrowseButton_Click(sender As Object, e As EventArgs) Handles ChecksumDirecoryBrowseButton.Click
        Dim fd As New FolderBrowserDialog

        ' Set the Help text description for the FolderBrowserDialog.
        fd.Description =
            "Select where to find the files to do the checksum on"
        ' Turn off the create new folder button
        fd.ShowNewFolderButton = False
        ' Initialize with recommended location

        If (fd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            ChecksumDirectoryText.Text = fd.SelectedPath & "\"
        End If
    End Sub

    Private Sub OutputBrowseButton_Click(sender As Object, e As EventArgs) Handles OutputBrowseButton.Click
        Dim fd As OpenFileDialog = New OpenFileDialog()

        fd.Title = "Select driver file"
        fd.Filter = "Manifest file (Manifest)|Manifest|csv files (*.csv)|*.csv|All files (*.*)|*.*"
        fd.FilterIndex = 2
        fd.RestoreDirectory = True

        If fd.ShowDialog() = DialogResult.OK Then
            OutputTextBox.Text = fd.FileName
        End If
    End Sub

    Private Sub ManifestTool_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
