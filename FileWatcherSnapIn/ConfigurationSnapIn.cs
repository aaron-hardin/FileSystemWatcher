using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ManagementConsole;

namespace FileWatcherSnapIn
{
	[SnapInSettings(SnapInGuid, DisplayName = DisplayName, Description = Description, Vendor = Vendor)]
    public class ConfigurationSnapIn : SnapIn
	{
		public const string SnapInGuid = "{CA6C88D2-DE81-4D72-A6F0-97970F9DAC51}";
		public const string DisplayName = "File Watcher";
		public const string Description = "File Watcher";
		public const string Vendor = "Aaron";

		public ConfigurationSnapIn()
		{
			
		}
	}
}
