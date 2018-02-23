using System;
using System.Diagnostics;
using System.Text;

namespace RemoteProtobufCompilingServer.Models
{
	public class ProtobufCompilerWrapper
	{
		public string OutputPath { get; set; }

		public string InputPath { get; set; }

		public ProtobufCompilerWrapper(string outputPath, string inputPath)
		{
			OutputPath = outputPath;
			InputPath = inputPath;
		}

		public string Compile(ProtobufCompilerWrapperOptions options)
		{
			var protocCommand = GenerateProtocCommand(options);

			var escapedProtocCommand = protocCommand.Replace("\"", "\\\"");

			var protocProcess = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "/bin/bash",
					Arguments = $"-c \"{protocCommand}\"",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			protocProcess.Start();
			protocProcess.WaitForExit();

			// Get output
			var outputStream = protocProcess.StandardError;

			return outputStream.ReadToEnd();
		}

		private string GenerateProtocCommand(ProtobufCompilerWrapperOptions options)
		{
			// Not ideal
			return $"cd {InputPath} && protoc --{options.OutputType}_out={OutputPath} * && cd {OutputPath} && find . -type f -print0 | xargs -0 todos";
		}
	}
}