using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileWatcherPluginLibrary;

namespace SampleEmailPlugin
{
	public class WatchedFolderConfiguration : IFolderConfiguration
	{
		public string Path { get; set; }
	}
}
