using System;
using System.IO;

namespace FileWatcherPluginLibrary
{
	public interface IPlugin
	{
		void Initialize();

		IPluginConfiguration Configuration { get; set; }

		void Trigger(IFolderConfiguration triggeredFolder, FileSystemEventArgs args);

		Type ConfigurationType { get; }
	}
}
