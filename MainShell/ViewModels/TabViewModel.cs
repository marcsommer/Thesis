using Caliburn.Micro;

namespace MainShell.ViewModels
{
	public sealed class TabViewModel<T> : Screen where T : PropertyChangedBase
	{
		public T ContentViewModel { get; set; }

		public TabViewModel(string headerName)
		{
			DisplayName = headerName;
		}

		public TabViewModel(T data)
		{
			ContentViewModel = data;
		}

		public TabViewModel(string headerName, T data)
		{
			DisplayName = headerName;
			ContentViewModel = data;
		}
	}
}
