using System.Collections.Generic;
using System.IO;

namespace FileWatcherPluginLibrary
{
	public interface IPlugin
	{
		void Initialize();

		IPluginConfiguration Configuration { get; }
		List<IFolderConfiguration> WatchedFolders { get; }

		void Trigger(IFolderConfiguration triggeredFolder, FileSystemEventArgs args);
	}
}
