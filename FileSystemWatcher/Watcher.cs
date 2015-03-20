using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileWatcherPluginLibrary;

namespace FileWatcher
{
    public class Watcher
    {
	    public const string PluginDirectory = "plugins";
	    private readonly List<IPlugin> plugins;

	    public Watcher()
	    {
			plugins = GenericPluginLoader<IPlugin>.LoadPlugins(PluginDirectory).ToList();

		    SetupPlugins();
	    }

	    public Watcher(List<IPlugin> plugins)
	    {
		    this.plugins = plugins;

			SetupPlugins();
	    }

	    private void SetupPlugins()
	    {
			foreach (IPlugin plugin in plugins)
			{
				try
				{
					plugin.Initialize();

					foreach (IFolderConfiguration folderConfiguration in plugin.WatchedFolders)
					{
						// TODO: get settings from folder configuration
						FileSystemWatcher watcher = new FileSystemWatcher();
						watcher.EnableRaisingEvents = true;
						watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Security;

						IFolderConfiguration configuration = folderConfiguration;
						IPlugin plugin1 = plugin;
						watcher.Created += (sender, args) => { plugin1.Trigger(configuration, args); };
					}
				}
				catch (Exception e)
				{
					// TODO: log to event log?
				}
			}
	    }
    }
}
