using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FileWatcherPluginLibrary;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SampleEmailPlugin
{
	public class EmailPluginConfiguration : IPluginConfiguration
	{
		/// <summary>
		/// Used by the Configuration Manager to edit the configurations
		/// </summary>
		[UsedImplicitly]
		public List<WatchedFolderConfiguration> WatchedFolderConfigurations { get; set; }
		
		public string EmailAddress { get; set; }

		/// <summary>
		/// Simple encryption just to show the idea, don't want anyone reading our password on the screen.
		/// </summary>
		private string password;
		public string Password
		{
			get { return password ?? string.Empty; }
			set { password = StringCipher.Encrypt(value); }
		}

		[Browsable(false)]
		[JsonIgnore]
		internal string DecryptedPassword
		{
			get { return StringCipher.Decrypt(Password); }
		}

		public EmailPluginConfiguration()
		{
			WatchedFolderConfigurations = new List<WatchedFolderConfiguration>();
		}

		// TODO: abstract specific tags to interface if possible
		// that way the developer building a plugin does not need to worry about this.
		[Browsable(false)]
		[JsonIgnore]
		public List<IFolderConfiguration> WatchedFolders
		{
			get { return WatchedFolderConfigurations.Cast<IFolderConfiguration>().ToList(); }
		}

		public bool WatchConfiguration
		{
			get { return false; }
		}
	}
}
