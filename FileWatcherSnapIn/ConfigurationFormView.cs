﻿using Microsoft.ManagementConsole;
using Action = Microsoft.ManagementConsole.Action;

namespace FileWatcherSnapIn
{
	public class ConfigurationFormView : FormView
	{
		private ConfigurationControl control;

		protected override void OnInitialize(AsyncStatus status)
		{
			base.OnInitialize(status);

			control = (ConfigurationControl) Control;
		}

		protected override void OnAction(Action action, AsyncStatus status)
		{
			control.DelegateAction((string)action.Tag);
		}

		protected override void OnSelectionAction(Action action, AsyncStatus status)
		{
			control.DelegateAction((string)action.Tag);
		}
	}
}
