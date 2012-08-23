namespace Simulator
{
	public interface IFileDialogWrapper
	{
		bool? ShowDialog();
		string GetSelectedFileName();
	}
}
