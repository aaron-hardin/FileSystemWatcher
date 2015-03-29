﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FileWatcherPluginLibrary;
using Newtonsoft.Json;

namespace SampleEmailPlugin
{
	public class EmailPluginConfiguration : IPluginConfiguration
	{
		public List<WatchedFolderConfiguration> watchedFolders { get; set; }
		public string EmailAddress { get; set; }

		public EmailPluginConfiguration()
		{
			watchedFolders = new List<WatchedFolderConfiguration>();
		}

		// TODO: abstract specific tags to interface if possible
		// that way the developer building a plugin does not need to worry about this.
		[Browsable(false)]
		[JsonIgnore]
		public List<IFolderConfiguration> WatchedFolders
		{
			get { return watchedFolders.Cast<IFolderConfiguration>().ToList(); }
		}

		public bool WatchConfiguration
		{
			get { return false; }
		}
	}
}
