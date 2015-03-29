using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FileWatcher;
using FileWatcherPluginLibrary;
using FileWatcherSnapIn;
using Microsoft.Win32;

namespace CustomInstallHelper
{
	[RunInstaller(true)]
	public class InstallerAction : Installer
	{
		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);

			try
			{
				RegistryKey key = GetSnapInRegKey(false);
				if(!key.GetSubKeyNames().Contains(MmcSubkeyName))
				{
					SetMMCSnapIn();
					stateSaver.Add("addedMMC", true);
				}
				else
				{
					stateSaver.Add("addedMMC", false);
				}
			}
			catch(Exception e)
			{
				SysUtils.ReportErrorMessageToEventLog("Failed to install MMC Snap In", e);
			}
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);

			try
			{
				if((bool) savedState["addedMMC"])
				{
					RemoveMMCSnapIn();
				}
			}
			catch(Exception e)
			{
				SysUtils.ReportErrorMessageToEventLog("Failed to uninstall MMC Snap In", e);
			}
		}

		public override void Rollback(IDictionary savedState)
		{
			base.Rollback(savedState);

			try
			{
				if((bool) savedState["addedMMC"])
				{
					RemoveMMCSnapIn();
				}
			}
			catch(Exception e)
			{
				SysUtils.ReportErrorMessageToEventLog("Failed to roll back MMC Snap In", e);
			}
		}

		private static void RemoveMMCSnapIn()
		{
			using(RegistryKey reg = GetSnapInRegKey(true))
			{
				reg.DeleteSubKeyTree(MmcSubkeyName);
			}
		}

		private static RegistryKey GetSnapInRegKey(bool writable)
		{
			return
				RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
					.OpenSubKey(@"SOFTWARE\Microsoft\MMC\SnapIns", writable);
		}

		private static string MmcSubkeyName
		{
			get { return "FX:" + Constants.SnapInGuid; }
		}

		private static void SetMMCSnapIn()
		{
			using(RegistryKey reg = GetSnapInRegKey(true))
			{
				using(RegistryKey snapInKey = reg.CreateSubKey(MmcSubkeyName))
				{
					using(RegistryKey nodeTypesKey = snapInKey.CreateSubKey("NodeTypes"))
					{
					}
					using (RegistryKey standaloneKey = snapInKey.CreateSubKey("Standalone"))
					{
					}

					snapInKey.SetValue("UseCustomHelp", 00000000, RegistryValueKind.DWord);
					snapInKey.SetValue("Type", typeof(ConfigurationSnapIn).AssemblyQualifiedName, RegistryValueKind.String);
					snapInKey.SetValue("ApplicationBase", MyPath(), RegistryValueKind.String);
					snapInKey.SetValue("NameString", Constants.DisplayName, RegistryValueKind.String);
					snapInKey.SetValue("Description", Constants.Description, RegistryValueKind.String);
					snapInKey.SetValue("ModuleName", typeof(ConfigurationSnapIn).Assembly.GetName(), RegistryValueKind.String);
					snapInKey.SetValue("AssemblyName", typeof(ConfigurationSnapIn).Name, RegistryValueKind.String);
					snapInKey.SetValue("RuntimeVersion", "v4.0.30319", RegistryValueKind.String);
					snapInKey.SetValue("FxVersion", "3.0.0.0", RegistryValueKind.String);
					snapInKey.SetValue("About", "{00000000-0000-0000-0000-000000000000}", RegistryValueKind.String);
					snapInKey.SetValue("Provider", Constants.Vendor, RegistryValueKind.String);
				}
			}
		}

		private static string MyPath()
		{
			string myFile = Assembly.GetExecutingAssembly().Location;
			string myPath = Path.GetDirectoryName(myFile);
			return myPath;
		}

	}
}
