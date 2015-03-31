using System.ServiceProcess;

namespace FileWatcherService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] servicesToRun = { 
				new Service1() 
			};
			ServiceBase.Run(servicesToRun);
		}
	}
}
