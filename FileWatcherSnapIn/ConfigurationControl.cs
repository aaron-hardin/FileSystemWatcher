using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FileWatcher;
using FileWatcherPluginLibrary;
using Microsoft.ManagementConsole;
using Newtonsoft.Json;

namespace FileWatcherSnapIn
{
	public partial class ConfigurationControl : UserControl, IFormViewControl
	{
		private ConfigurationFormView formView = null;

		public string PluginPath
		{
			get
			{
				return Watcher.PluginDirectory;
			}
		}

		public ConfigurationControl()
		{
			InitializeComponent();

			ConfigurationTabControl.TabPages.Clear();

			try
			{
				List<Type> configurationTypes = GetPluginConfigurationTypes(PluginPath);

				foreach (Type configurationType in configurationTypes)
				{
					TabPage tp = new TabPage(configurationType.Name);
					PropertyGrid pg = new PropertyGrid();
					pg.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
					string path = Path.Combine(PluginPath, configurationType.Name + ".json");
					if (File.Exists(path))
					{
						pg.SelectedObject = JsonConvert.DeserializeObject(File.ReadAllText(path), configurationType);
					}
					else
					{
						pg.SelectedObject = Activator.CreateInstance(configurationType);
					}
					tp.Controls.Add(pg);
					ConfigurationTabControl.TabPages.Add(tp);
				}
			}
			catch(Exception e)
			{
				SysUtils.ReportErrorMessageToEventLog("Failed to load plugin types", e);
			}
		}

		public void Initialize(FormView view)
		{
			formView = (ConfigurationFormView) view;

			formView.SelectionData.ActionsPaneItems.Clear();
		}

		private List<Type> GetPluginConfigurationTypes(string pluginDirectory)
		{
			// Include subdirectories
			string[] pluginAssemblies = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);

			return GetPluginConfigurationTypes(pluginAssemblies.ToList());
		}

		private List<Type> GetPluginConfigurationTypes(List<string> assemblies)
		{
			List<Type> configurationTypes = new List<Type>();
			foreach (string assemblyPath in assemblies)
			{
				AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
				Assembly assembly = Assembly.Load(assemblyName);
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (type.IsClass && !type.IsAbstract && type.GetInterface(typeof(IPluginConfiguration).Name) != null)
					{
						configurationTypes.Add(type);
					}
				}
			}
			return configurationTypes;
		}
	}
}
