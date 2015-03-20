using FileWatcherPluginLibrary;

namespace SampleEmailPlugin
{
	public class EmailPluginConfiguration : IPluginConfiguration
	{
		public string EmailAddress { get; set; }
	}
}
