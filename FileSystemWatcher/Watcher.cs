using System;
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
	    public const string PluginDirectoryName = "plugins";

		public static string PluginDirectory { get { return Path.Combine(AssemblyDirectory, PluginDirectoryName); } }

	    public static string AssemblyDirectory
	    {
		    get
		    {
			    string codeBase = Assembly.GetAssembly(typeof(Watcher)).CodeBase;
			    UriBuilder uri = new UriBuilder(codeBase);
			    string path = Uri.UnescapeDataString(uri.Path);
			    return Path.GetDirectoryName(path);
		    }
	    }

	    private readonly List<IPlugin> _plugins;

	    public Watcher()
	    {
		    _plugins = LoadPlugins(PluginDirectory);

		    SetupPlugins();
	    }

	    public Watcher(List<IPlugin> plugins)
	    {
		    _plugins = plugins;

			SetupPlugins();
	    }

	    private void SetupPlugins()
	    {
			foreach (IPlugin plugin in _plugins)
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
			List<IPlugin> loadedPlugins = new List<IPlugin>();
		    foreach(string assemblyPath in assemblies)
		    {
			    AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
				Assembly assembly = Assembly.Load(assemblyName);
			    Type[] types = assembly.GetTypes();
			    foreach(Type type in types)
			    {
				    if(type.IsClass && !type.IsAbstract && type.GetInterface(typeof(IPlugin).Name) != null)
				    {
						loadedPlugins.Add(CreatePlugin(type));
				    }
			    }
		    }
			return loadedPlugins;
	    }

	    private IPlugin CreatePlugin(Type pluginType)
	    {
		    IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);

		    string pluginConfigPath = pluginType.Name + ".json";

			// Load Configuration
			if (File.Exists(pluginConfigPath))
			{
				plugin.Configuration = (IPluginConfiguration)JsonConvert.DeserializeObject(File.ReadAllText(pluginConfigPath), plugin.ConfigurationType);
			}
			else
			{
				plugin.Configuration = (IPluginConfiguration)Activator.CreateInstance(plugin.ConfigurationType);
				using(StreamWriter sw = new StreamWriter(pluginConfigPath))
				{
					sw.WriteLine(JsonConvert.SerializeObject(plugin.Configuration, Formatting.Indented));
				}
			}

		    return plugin;
	    }
    }
}
