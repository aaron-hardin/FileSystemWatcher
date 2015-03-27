using System.Collections.Generic;
using System.Linq;
using FileWatcherPluginLibrary;

namespace SampleEmailPlugin
{
	public class EmailPluginConfiguration : IPluginConfiguration
	{
		private List<WatchedFolderConfiguration> watchedFolders;
		public string EmailAddress { get; set; }

		public EmailPluginConfiguration()
		{
			watchedFolders = new List<WatchedFolderConfiguration>();
		}

		public List<IFolderConfiguration> WatchedFolders
		{
			get { return watchedFolders.Cast<IFolderConfiguration>().ToList(); }
		}

		public bool WatchConfiguration
		{
			get { return false; }
		}
	}
}
