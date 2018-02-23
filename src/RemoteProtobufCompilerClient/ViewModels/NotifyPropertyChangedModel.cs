using System.ComponentModel;

namespace RemoteProtobufCompilerClient.ViewModels
{
	public abstract class NotifyPropertyChangedModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			var eventArgs = new PropertyChangedEventArgs(propertyName);
			PropertyChanged?.Invoke(this, eventArgs);
		}
	}
}