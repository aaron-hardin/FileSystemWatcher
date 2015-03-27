using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ManagementConsole;

namespace FileWatcherSnapIn
{
	public partial class ConfigurationControl : UserControl, IFormViewControl
	{
		public ConfigurationControl()
		{
			InitializeComponent();
		}

		public void Initialize(FormView view)
		{
			throw new NotImplementedException();
		}
	}
}
