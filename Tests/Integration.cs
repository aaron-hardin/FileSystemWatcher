using System;
using System.Collections.Generic;
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
    }
}
