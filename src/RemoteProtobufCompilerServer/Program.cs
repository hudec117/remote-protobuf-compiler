using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace RemoteProtobufCompilingServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.AddCommandLine(args)
				.Build();

			var builder = WebHost.CreateDefaultBuilder(args)
				.UseKestrel()
				.UseConfiguration(configuration)
				.UseStartup<Startup>();

			return builder.Build();
		}
	}
}