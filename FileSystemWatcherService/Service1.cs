using System.ServiceProcess;
using FileWatcher;

namespace FileWatcherService
{
	public partial class Service1 : ServiceBase
	{
		private Watcher watcher;

		public Service1()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			// TODO: error handling
			watcher = new Watcher();

			// TODO: move some functions to start method
		}

		protected override void OnStop()
		{
		}
	}
}
