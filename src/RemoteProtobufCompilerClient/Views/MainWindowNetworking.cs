using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace RemoteProtobufCompilerClient.Views
{
	public partial class MainWindow
	{
		private async void BtnCompileOnClick(object sender, RoutedEventArgs e)
		{
			model.IsSendingRequest = true;
			try
			{
				settings.Save();

				using (var postContent = GetPostRequestContent())
				{
					var apiUrl = $"http://{model.APIHost}/api/compile";

					HttpResponseMessage response;
					try
					{
						Log("Sending request to server...");

						// Send POST request to server.
						response = await httpClient.PostAsync(apiUrl, postContent);
					}
					catch (HttpRequestException exception)
					{
						var innerException = exception.InnerException;

						Log(innerException.Message);
						return;
					}

					var responseContent = response.Content;

					if (response.IsSuccessStatusCode)
						await OnSuccessResponse(responseContent);
					else
						await OnFailedResopnse(responseContent);
				}
			}
			finally
			{
				model.IsSendingRequest = false;
			}
		}

		private async Task OnSuccessResponse(HttpContent responseContent)
		{
			var archivePath = await DownloadArchive(responseContent);

			ExtractArchive(archivePath);

			Log("Finished successfully");
		}

		private async Task OnFailedResopnse(HttpContent responseContent)
		{
			var responseText = await responseContent.ReadAsStringAsync();

			txtLog.Text = responseText;
		}

		private async Task<string> DownloadArchive(HttpContent responseContent)
		{
			Log("Saving response archive...");

			var tempPath = Path.GetTempFileName();
			using (var fileStream = File.OpenWrite(tempPath))
			{
				await responseContent.CopyToAsync(fileStream);
			}

			Log("Response archive saved");

			return tempPath;
		}

		private void ExtractArchive(string tempPath)
		{
			Log("Extracting archive...");

			try
			{
				using (var archive = ZipFile.OpenRead(tempPath))
				{
					foreach (var entry in archive.Entries)
					{
						var linuxEntryFileName = entry.FullName;
						var windowsEntryFileName = linuxEntryFileName.Replace("/", "\\");

						// Create the absolute path where the file is going to be extracted.
						var fileOutputPath = Path.Combine(model.OutputPath, windowsEntryFileName);

						// Create the directory for the upcoming file extraction.
						var fileInfo = new FileInfo(fileOutputPath);
						fileInfo.Directory.Create();

						// Extract entry to file.
						entry.ExtractToFile(fileOutputPath, true);
					}
				}
			}
			catch (Exception e)
			{
				Log(e.Message);
			}

			Log("Archive extracted");
		}

		private MultipartContent GetPostRequestContent()
		{
			var filePaths = model.FilePaths;

			var multiFileContent = new MultipartFormDataContent();

			foreach (var filePath in filePaths)
			{
				var fileStream = File.OpenRead(filePath);

				var fileContent = new StreamContent(fileStream);

				var fileName = Path.GetFileName(filePath);

				multiFileContent.Add(fileContent, "files", fileName);
			}

			var friendlyOutputType = serverFriendlyOutputTypeLookup[model.OutputType];
			multiFileContent.Headers.Add("Compile-To", friendlyOutputType);

			return multiFileContent;
		}
	}
}