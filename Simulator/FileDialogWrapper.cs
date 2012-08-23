namespace Simulator
{
	public class FileDialogWrapper : IFileDialogWrapper
	{
		private readonly Microsoft.Win32.OpenFileDialog fileDialog;

		public FileDialogWrapper()
		{
			fileDialog = new Microsoft.Win32.OpenFileDialog
			{
				FileName = "input",
				DefaultExt = ".xml",
				Filter = "XML Documents (.xml)|*.xml"
			};

		}

		public bool? ShowDialog()
		{
			return fileDialog.ShowDialog();
		}

		public string GetSelectedFileName()
		{
			return fileDialog.FileName;
		}
	}
}
