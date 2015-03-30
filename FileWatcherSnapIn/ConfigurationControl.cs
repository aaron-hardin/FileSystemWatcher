using System;
using System.Collections.Generic;
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
		private ConfigurationFormView formView;
		public const string ActionSave = "Save";

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

			Dock = DockStyle.Fill;
			ConfigurationTabControl.TabPages.Clear();

			try
			{
				List<Type> configurationTypes = GetPluginConfigurationTypes(PluginPath);

				foreach (Type configurationType in configurationTypes)
				{
					TabPage tp = GetTabPage(configurationType);
					ConfigurationTabControl.TabPages.Add(tp);
				}
			}
			catch(Exception e)
			{
				SysUtils.ReportErrorMessageToEventLog("Failed to load plugin types", e);
			}
		}

		public void DelegateAction(string commandName)
		{
			switch(commandName)
			{
				case ActionSave:
					try
					{
						Save();
					}
					catch(Exception e)
					{
						MessageBox.Show("Failed to save configuration\n" + e);
					}
					break;
			}
		}

		private void Save()
		{
			TabPage tp = ConfigurationTabControl.SelectedTab;
			PropertyGrid pg = (PropertyGrid)tp.Controls[0];
			object selectedObject = pg.SelectedObject;
			string path = Path.Combine(PluginPath, tp.Name + ".json");
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.WriteLine(JsonConvert.SerializeObject(selectedObject, Formatting.Indented));
			}
		}

		private TabPage GetTabPage(Type configurationType)
		{
			TabPage tp = new TabPage(configurationType.Name);
			tp.Name = configurationType.Name;
			PropertyGrid pg = new PropertyGrid();
			pg.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			pg.Dock = DockStyle.Fill;
			string path = Path.Combine(PluginPath, configurationType.Name + ".json");
			if (File.Exists(path))
			{
				pg.SelectedObject = JsonConvert.DeserializeObject(File.ReadAllText(path), configurationType);
			}
			else
			{
				object newObject = Activator.CreateInstance(configurationType);
				pg.SelectedObject = newObject;
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.WriteLine(JsonConvert.SerializeObject(newObject, Formatting.Indented));
				}
			}
			tp.Controls.Add(pg);

			return tp;
		}

		public void Initialize(FormView view)
		{
			formView = (ConfigurationFormView) view;

			formView.SelectionData.ActionsPaneItems.Clear();
			Microsoft.ManagementConsole.Action action = new Microsoft.ManagementConsole.Action("Save Configuration", "Save the current configuration.", -1, ActionSave);
			formView.ActionsPaneItems.Add(action);
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
				List<Type> types = assembly.GetConfigurationTypes();

				if(types.Count > 1)
				{
					throw new Exception(string.Format("Expected only one configuration type in assembly {0}.", assembly.GetName()));
				}

				if(types.Count == 1)
				{
					configurationTypes.Add(types[0]);
				}
			}
			return configurationTypes;
		}
	}
}
