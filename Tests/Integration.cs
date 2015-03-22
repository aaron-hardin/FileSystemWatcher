using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileWatcher;
using FileWatcherPluginLibrary;
using NUnit.Framework;
using SampleEmailPlugin;

namespace Tests
{
	[TestFixture]
    public class Integration
    {
		[Test]
		public void Startup()
		{
			List<IPlugin> plugins = new List<IPlugin>();
			EmailPlugin testPlugin = new EmailPlugin();
			((EmailPluginConfiguration) testPlugin.Configuration).EmailAddress = "asdf";
			plugins.Add(testPlugin);
			Watcher watcher = new Watcher(plugins);
		}

		[Test]
		public void StartupNoPlugin()
		{
			string currentDir = Directory.GetCurrentDirectory();
			Console.WriteLine("Current Directory: "+currentDir);
			string pluginDir = Path.Combine(currentDir, Watcher.PluginDirectory);
			if(!Directory.Exists(pluginDir))
			{
				Console.WriteLine("Creating Plugin Directory: "+pluginDir);
				Directory.CreateDirectory(pluginDir);
			}
			else
			{
				foreach(string file in Directory.GetFiles(pluginDir))
				{
					File.Delete(file);
				}
				foreach(string directory in Directory.GetDirectories(pluginDir))
				{
					Directory.Delete(directory, true);
				}
			}
			string pluginAssemblyName = typeof(EmailPlugin).Assembly.GetName().Name + ".dll";
			string pluginPath = Path.Combine(currentDir, pluginAssemblyName);
			Console.WriteLine("Asserting that plugin exists at: "+pluginPath);
			Assert.True(File.Exists(pluginPath));
			File.Copy(pluginPath, Path.Combine(pluginDir, pluginAssemblyName));

			File.Copy(
				Path.Combine(currentDir, "FileWatcherPluginLibrary.dll"),
				Path.Combine(pluginDir, "FileWatcherPluginLibrary.dll"));

			Watcher watcher = new Watcher();
		}
    }
}
