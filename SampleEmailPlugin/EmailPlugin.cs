using System;
using System.Collections.Generic;
using System.IO;
using FileWatcherPluginLibrary;

namespace SampleEmailPlugin
{
    public class EmailPlugin : IPlugin
    {
	    private EmailPluginConfiguration configuration;
	    private List<IFolderConfiguration> watchedFolders;

	    public EmailPlugin()
	    {
		    configuration = new EmailPluginConfiguration();
			watchedFolders = new List<IFolderConfiguration>();
	    }

	    public void Initialize()
	    {
		    
	    }

		public IPluginConfiguration Configuration
	    {
		    get { return configuration; }
	    }

		public List<IFolderConfiguration> WatchedFolders
	    {
		    get { return watchedFolders; }
	    }
		
		public void Trigger(IFolderConfiguration triggeredFolder, FileSystemEventArgs args)
		{
			// TODO: send email
			Console.WriteLine("{0} has triggered {1} with change type {2}", args.Name, triggeredFolder.Path, args.ChangeType);
		}
	}
}
