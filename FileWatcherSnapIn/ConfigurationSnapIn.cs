using FileWatcherPluginLibrary;
using Microsoft.ManagementConsole;

namespace FileWatcherSnapIn
{
	[SnapInSettings(Constants.SnapInGuid, DisplayName = Constants.DisplayName, Description = Constants.Description, Vendor = Constants.Vendor)]
    public class ConfigurationSnapIn : SnapIn
	{
		public ConfigurationSnapIn()
		{
			RootNode = new ScopeNode();
			RootNode.DisplayName = Constants.DisplayName;

			FormViewDescription fvd = new FormViewDescription();
			fvd.DisplayName = "Configuration";
			fvd.ViewType = typeof(ConfigurationFormView);
			fvd.ControlType = typeof(ConfigurationControl);

			RootNode.ViewDescriptions.Add(fvd);
			RootNode.ViewDescriptions.DefaultIndex = 0;
		}
	}
}
