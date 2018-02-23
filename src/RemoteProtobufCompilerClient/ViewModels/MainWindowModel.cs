using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace RemoteProtobufCompilerClient.ViewModels
{
	public class MainWindowModel : NotifyPropertyChangedModel, IDataErrorInfo
	{
		[JsonProperty("filePaths")]
		public ObservableCollection<string> FilePaths { get; set; } = new ObservableCollection<string>();

		[JsonIgnore]
		public bool IsSendingRequest
		{
			get => isSendingRequest;
			set
			{
				if (value != isSendingRequest)
				{
					isSendingRequest = value;
					NotifyPropertyChanged(nameof(IsSendingRequest));
				}
			}
		}

		[JsonProperty(nameof(outputPath))]
		public string OutputPath
		{
			get => outputPath;
			set
			{
				if (value != outputPath)
				{
					outputPath = value;
					NotifyPropertyChanged(nameof(OutputPath));
				}
			}
		}

		[JsonProperty(nameof(apiHost))]
		public string APIHost
		{
			get => apiHost;
			set
			{
				if (value != apiHost)
				{
					apiHost = value;
					NotifyPropertyChanged(nameof(APIHost));
				}
			}
		}

		[JsonProperty(nameof(outputType))]
		public string OutputType
		{
			get => outputType;
			set
			{
				if (value != outputType)
				{
					outputType = value;
					NotifyPropertyChanged(nameof(OutputType));
				}
			}
		}

		[JsonIgnore]
		public string Error => string.Empty;

		public string this[string columnName]
		{
			get
			{
				switch (columnName)
				{
					case nameof(OutputPath):
						if (!Directory.Exists(OutputPath))
							return "Path is not valid.";
						break;
					case nameof(APIHost):
						if (string.IsNullOrWhiteSpace(APIHost))
							return "Invalid API host.";
						break;
				}

				return null;
			}
		}

		private bool isSendingRequest;

		private string outputPath;
		private string apiHost;
		private string outputType;
	}
}