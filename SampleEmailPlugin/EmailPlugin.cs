using System;
using System.Collections.Generic;
using System.IO;
using FileWatcherPluginLibrary;

namespace SampleEmailPlugin
{
    public class EmailPlugin : IPlugin
    {
	    private EmailPluginConfiguration configuration;
	    
	    public EmailPlugin()
	    {
		    configuration = new EmailPluginConfiguration();
	    }

	    public void Initialize()
	    {
		    
	    }

		public IPluginConfiguration Configuration
	    {
		    get { return configuration; }
			set { configuration = (EmailPluginConfiguration) value; }
	    }

		public void Trigger(IFolderConfiguration triggeredFolder, FileSystemEventArgs args)
		{
			// TODO: send email
			Console.WriteLine("{0} has triggered {1} with change type {2}", args.Name, triggeredFolder.Path, args.ChangeType);
		}

	    public Type ConfigurationType
	    {
		    get { return typeof(EmailPluginConfiguration); }
	    }
    }
}
