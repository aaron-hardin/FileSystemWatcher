using System.ComponentModel;
using System.Configuration.Install;
using FileWatcherPluginLibrary;

namespace FileWatcherService
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();

			// TODO: update props from common location
			serviceInstaller1.Description = Constants.Description;
		}
	}
}
