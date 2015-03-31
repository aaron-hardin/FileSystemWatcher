using FileWatcherPluginLibrary;

namespace SampleEmailPlugin
{
	public class WatchedFolderConfiguration : IFolderConfiguration
	{
		public string Path { get; set; }
	}
}
