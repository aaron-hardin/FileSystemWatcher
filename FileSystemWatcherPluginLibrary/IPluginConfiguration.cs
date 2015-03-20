using System.Collections.Generic;

namespace FileWatcherPluginLibrary
{
	public interface IPluginConfiguration
	{
		List<IFolderConfiguration> WatchedFolders { get; }
		bool WatchConfiguration { get; }
	}
}
