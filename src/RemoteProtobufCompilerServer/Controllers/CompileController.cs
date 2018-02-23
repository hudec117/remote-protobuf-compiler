using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemoteProtobufCompilingServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RemoteProtobufCompilingServer.Controllers
{
	[Route("api/[controller]")]
	public class CompileController : Controller
	{
		private readonly ProtobufCompilerWrapper compiler;

		private readonly IList<string> outputTypes = new List<string>
		{
			"cpp",
			"csharp",
			"java",
			"js",
			"objc",
			"php",
			"python",
			"ruby"
		};

		private readonly string toCompilePath;
		private readonly string compiledPath;
		private readonly string toDownloadPath;

		private const string compileToHeaderKey = "Compile-To";

		public CompileController(IHostingEnvironment hostingEnvironment)
		{
			compiledPath = Path.Combine(hostingEnvironment.ContentRootPath, "proto-compiled/");
			toCompilePath = Path.Combine(hostingEnvironment.ContentRootPath, "proto-to-compile/");
			toDownloadPath = Path.Combine(hostingEnvironment.ContentRootPath, "proto-to-download/");

			SetupProtoFolders();

			compiler = new ProtobufCompilerWrapper(compiledPath, toCompilePath);
		}

		[HttpPost]
		public IActionResult Index(IFormFileCollection files)
		{
			// Make sure that there are files.
			var totalLength = files.Sum(x => x.Length);
			if (totalLength == 0)
				return BadRequest("No file(s) or empty file(s).");

			// Make sure that the Compile-To header is present.
			if (!Request.Headers.ContainsKey(compileToHeaderKey))
				return BadRequest($"\"{compileToHeaderKey}\" header missing.");

			// Make sure that the output type is valid.
			var outputType = Request.Headers[compileToHeaderKey];
			if (!IsValidOutputType(outputType))
				return BadRequest("Invalid Compile-To value.");

			SaveUploadedFiles(files);

			var compilerOptions = new ProtobufCompilerWrapperOptions
			{
				OutputType = outputType
			};

			var output = compiler.Compile(compilerOptions);
			if (!string.IsNullOrWhiteSpace(output))
				return StatusCode(422, output);

			var compiledZipPath = ZipUpCompiled(outputType);

			var compiledZipFileStream = System.IO.File.OpenRead(compiledZipPath);
			return File(compiledZipFileStream, "application/zip");
		}

		private bool IsValidOutputType(string outputType)
		{
			return outputTypes.Contains(outputType);
		}

		private void SaveUploadedFiles(IFormFileCollection files)
		{
			foreach (var file in files)
			{
				var savePath = Path.Combine(toCompilePath, file.FileName);

				using (var fileStream = System.IO.File.OpenWrite(savePath))
				{
					file.CopyTo(fileStream);
				}
			}
		}

		private string ZipUpCompiled(string outputType)
		{
			var zipFileName = $"{outputType}-compiled.zip";
			var zipFilePath = Path.Combine(toDownloadPath, zipFileName);

			ZipFile.CreateFromDirectory(compiledPath, zipFilePath);

			return zipFilePath;
		}

		private void SetupProtoFolders()
		{
			SetupFolder(compiledPath);
			SetupFolder(toCompilePath);
			SetupFolder(toDownloadPath);
		}

		private void SetupFolder(string path)
		{
			if (Directory.Exists(path))
				EmptyFolder(path);
			else
				Directory.CreateDirectory(path);
		}

		private void EmptyFolder(string path)
		{
			var directoryInfo = new DirectoryInfo(path);

			// Delete all files.
			foreach (var file in directoryInfo.GetFiles())
				file.Delete();

			// Delete all folders.
			foreach (var directory in directoryInfo.GetDirectories())
				directory.Delete(true);
		}
	}
}