using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace FileWatcherSnapIn
{
	[RunInstaller(true)]
	public partial class SnapInInstaller : System.Configuration.Install.Installer
	{
		public SnapInInstaller()
		{
			InitializeComponent();
		}
	}
}
