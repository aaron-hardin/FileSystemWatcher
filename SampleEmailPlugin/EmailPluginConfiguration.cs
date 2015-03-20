using System.Collections.Generic;
using FileWatcherPluginLibrary;

namespace SampleEmailPlugin
{
	public class EmailPluginConfiguration : IPluginConfiguration
	{
		private List<IFolderConfiguration> watchedFolders;
		public string EmailAddress { get; set; }

		public EmailPluginConfiguration()
		{
			watchedFolders = new List<IFolderConfiguration>();
		}

		public List<IFolderConfiguration> WatchedFolders
		{
			get { return watchedFolders; }
		}

		public bool WatchConfiguration
		{
			get { return false; }
		}
	}
}
