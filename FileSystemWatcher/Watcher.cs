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
			SysUtils.ReportToEventLog("DEBUG: Creating watcher, loading plugins...");
		    
			_plugins = LoadPlugins(PluginDirectory);

		    SysUtils.ReportToEventLog("DEBUG: Creating watcher, setting up plugins...\n\t" +
		                              string.Join("\n\t", _plugins.Select(pl => pl.ConfigurationType.Name)));

		    SetupPlugins();

			SysUtils.ReportToEventLog("DEBUG: Creating watcher, started");
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
					SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::SetupPlugins initializing " + plugin.ConfigurationType.Name);

					plugin.Initialize();
					
					if(plugin.Configuration.WatchedFolders == null || plugin.Configuration.WatchedFolders.Count == 0)
					{
						SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::SetupPlugins skipping because there are not folders to watch " + plugin.ConfigurationType.Name);

						continue;
					}

					SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::SetupPlugins setting up watchers for\n\t" +
					                          string.Join("\n\t", plugin.Configuration.WatchedFolders.Select(wf => wf.Path)));

					foreach (IFolderConfiguration folderConfiguration in plugin.Configuration.WatchedFolders)
					{
						// TODO: get settings from folder configuration
						FileSystemWatcher watcher = new FileSystemWatcher(folderConfiguration.Path);
						watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Security;
						watcher.EnableRaisingEvents = true;

						IFolderConfiguration configuration = folderConfiguration;
						IPlugin plugin1 = plugin;
						watcher.Created += (sender, args) =>
						{
							try
							{
								// Don't trust plugins
								plugin1.Trigger(configuration, args);
							}
							catch(Exception e)
							{
								SysUtils.ReportErrorMessageToEventLog("Error triggering plugin.", e);
							}
						};

						SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::SetupPlugins watching " + folderConfiguration.Path);
					}
				}
				catch (Exception e)
				{
					SysUtils.ReportErrorMessageToEventLog(string.Format("Failed to start plugin {0}", plugin.ConfigurationType.Name), e);

					// TODO: cleanup any failed plugins and filewatchers
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

				// TODO: refactor interfaces, really you should be able to have multiple plugins in one dll and have a plugin and configuration in different dlls
			    List<Type> pluginTypes = assembly.GetPluginTypes();
				List<Type> configTypes = assembly.GetConfigurationTypes();

			    if(pluginTypes.Count == 1)
			    {
				    if(pluginTypes.Count > 1)
				    {
					    throw new Exception(string.Format("Expected only one plugin type in assembly {0}.", assembly.GetName()));
				    }

				    if(configTypes.Count > 1)
				    {
					    throw new Exception(string.Format("Expected only one configuration type in assembly {0}.", assembly.GetName()));
				    }

				    if(configTypes.Count == 0)
				    {
					    throw new Exception(string.Format("Expected one configuration type in assembly {0}.", assembly.GetName()));
				    }

				    loadedPlugins.Add(CreatePlugin(pluginTypes[0], configTypes[0]));
			    }
		    }
			return loadedPlugins;
	    }

	    private IPlugin CreatePlugin(Type pluginType, Type configurationType)
	    {
			// TODO: print debug statments, want to make sure we are loading from the correct place.
			// TODO: load things correctly
			SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::CreatePlugin " + pluginType.Name);

		    IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);

			SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::CreatePlugin " + pluginType.Name+" created plugin");

			string pluginConfigPath = Path.Combine(PluginDirectory, configurationType.Name + ".json");

		    SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::CreatePlugin " + pluginType.Name + " looking for " +
		                              Path.Combine(Directory.GetCurrentDirectory(), pluginConfigPath));

			// Load Configuration
			if (File.Exists(pluginConfigPath))
			{
				SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::CreatePlugin " + pluginType.Name + " found config path");
				plugin.Configuration = (IPluginConfiguration)JsonConvert.DeserializeObject(File.ReadAllText(pluginConfigPath), plugin.ConfigurationType);
			}
			else
			{
				SysUtils.ReportToEventLog("DEBUG: FileWatcher::Watcher::CreatePlugin " + pluginType.Name + " did not find config path");
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
