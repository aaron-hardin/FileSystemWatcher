﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FileWatcherPluginLibrary;
using Newtonsoft.Json;

namespace FileWatcher
{
    public class Watcher
    {
	    public const string PluginDirectory = "plugins";
	    private readonly List<IPlugin> plugins;

	    public Watcher()
	    {
		    plugins = LoadPlugins(PluginDirectory);

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
					
					if(plugin.Configuration.WatchedFolders == null || plugin.Configuration.WatchedFolders.Count == 0)
					{
						continue;
					}

					foreach (IFolderConfiguration folderConfiguration in plugin.Configuration.WatchedFolders)
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

	    private List<IPlugin> LoadPlugins(string pluginDirectory)
	    {
			// Include subdirectories
		    string[] pluginAssemblies = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);

			return LoadPlugins(pluginAssemblies.ToList());
	    }

	    private List<IPlugin> LoadPlugins(List<string> assemblies)
	    {
			List<IPlugin> plugins = new List<IPlugin>();
		    foreach(string assemblyPath in assemblies)
		    {
				Assembly assembly = Assembly.Load(assemblyPath);
			    Type[] types = assembly.GetTypes();
			    foreach(Type type in types)
			    {
				    if(type.IsClass && !type.IsAbstract && type.GetInterface(typeof(IPlugin).Name) != null)
				    {
					    plugins.Add(CreatePlugin(type));
				    }
			    }
		    }
		    return plugins;
	    }

	    private IPlugin CreatePlugin(Type pluginType)
	    {
		    IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);

		    string pluginConfigPath = pluginType.Name + ".json";

			// Load Configuration
			if (File.Exists(pluginConfigPath))
			{
				plugin.Configuration = (IPluginConfiguration)JsonConvert.DeserializeObject(pluginConfigPath, plugin.ConfigurationType);
			}
			else
			{
				plugin.Configuration = (IPluginConfiguration)Activator.CreateInstance(plugin.ConfigurationType);
			}

		    return plugin;
	    }
    }
}
