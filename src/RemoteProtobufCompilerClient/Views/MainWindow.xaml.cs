using Microsoft.Win32;
using RemoteProtobufCompilerClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RemoteProtobufCompilerClient.Views
{
	public partial class MainWindow : Window
	{
		private IDictionary<string, string> serverFriendlyOutputTypeLookup;

		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private OpenFileDialog openFileDialog;

		private Settings settings;

		private MainWindowModel model;

		private HttpClient httpClient;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void WindowOnInitialised(object sender, EventArgs e)
		{
			serverFriendlyOutputTypeLookup = new Dictionary<string, string>
			{
				["C#"] = "csharp",
				["C++"] = "cpp",
				["Java"] = "java",
				["JavaScript"] = "javascript",
				["ObjectiveC"] = "objectivec",
				["PHP"] = "php",
				["Python"] = "python",
				["Ruby"] = "ruby"
			};

			folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog
			{
				Description = "Where to put compiled files"
			};

			openFileDialog = new OpenFileDialog
			{
				AddExtension = true,
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = true,
				Filter = "Protocol Buffer Files|*.proto|All Files|*.*",
				Title = "Select protocol buffer files"
			};

			settings = new Settings("settings.json");
			settings.Load();
			model = settings.Data;
			DataContext = model;

			httpClient = new HttpClient();
		}

		private void BtnSelectOutputPathOnClick(object sender, RoutedEventArgs e)
		{
			var dialogResult = folderBrowserDialog.ShowDialog();
			if (dialogResult == System.Windows.Forms.DialogResult.OK)
				model.OutputPath = folderBrowserDialog.SelectedPath;
		}

		private void BtnAddFilesOnClick(object sender, RoutedEventArgs e)
		{
			var dialogResult = openFileDialog.ShowDialog();
			if (dialogResult.HasValue && dialogResult.Value)
			{
				foreach (var fileName in openFileDialog.FileNames)
				{
					// We don't want duplicates!
					if (!model.FilePaths.Contains(fileName))
						model.FilePaths.Add(fileName);
				}
			}
		}

		private void BtnRemoveSelectedFileOnClick(object sender, RoutedEventArgs e)
		{
			var selectedPath = (string)lstBxInputFiles.SelectedItem;

			model.FilePaths.Remove(selectedPath);
		}

		private void BtnRemoveAllFilesOnClick(object sender, RoutedEventArgs e)
		{
			model.FilePaths.Clear();
		}

		private void LstBxInputFilesOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// Allowing users to open the protocol buffer files form within the app.
			if (lstBxInputFiles.SelectedItem is string filePath)
				Process.Start(filePath);
		}

		private void HyperlinkOnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(e.Uri.AbsoluteUri);
		}

		private void Log(string message)
		{
			var logText = $"{DateTime.Now.ToLongTimeString()} - {message}{Environment.NewLine}";

			txtLog.AppendText(logText);
			txtLog.ScrollToEnd();
		}
	}
}